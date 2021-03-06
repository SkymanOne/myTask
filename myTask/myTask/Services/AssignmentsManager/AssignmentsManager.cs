#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Services.Database.RepositoryWrapper;
using myTask.Services.UserConfigManager;
using SQLitePCL;

namespace myTask.Services.AssignmentsManager
{
    public class AssignmentsManager : IAssignmentsManager
    {

        //access all repositories from a single place can easily be changed,
        //if more repositories are required to be added
        private readonly IRepositoryWrapper _repWrapper;
        private readonly IUserConfigManager _userConfigManager;
        private readonly Calendar _calendar;

        public AssignmentsManager(IRepositoryWrapper repositoryWrapper, IUserConfigManager userConfigManager)
        {
            _repWrapper = repositoryWrapper;
            _userConfigManager = userConfigManager;
            _calendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
        }

        public async Task Init()
        {
            //TODO: add web service init
        }

        public async Task<IEnumerable<Assignment>> LoadAllAssignmentsAsync()
        {
            return await _repWrapper.AssignmentRepo.GetAllItemsAsync();
        }

        public async Task<WeeklyTimetable?> LoadWeeklyTimetableAsync(int weekOfTheYear)
        {
            var weeklyTimetable = await _repWrapper
                        .WeeklyTimetableRepo
                        .GetItemAsync(x => x.Number == weekOfTheYear);
            if (weeklyTimetable == null || weeklyTimetable.Timetables.Count == 0) 
            {
                //if we are trying to access the week before today which doesn't exist
                //no point of creating it, so just return null
                if (weekOfTheYear < DateTime.Now.DayOfYear / 7) return null;
                
                UserConfig userConfig = await _userConfigManager.GetConfigAsync();
                Calendar calendar = new GregorianCalendar();
                if (weeklyTimetable == null)
                {
                    weeklyTimetable = new WeeklyTimetable()
                    {
                        Id = Guid.NewGuid(),
                        Number = weekOfTheYear,
                        Year = DateTime.Now.Year,
                        Timetables = new List<DailyTimetable>()
                    };
                    await _repWrapper.WeeklyTimetableRepo.CreateItemAsync(weeklyTimetable);
                }

                int c = (int) DateTime.Now.DayOfWeek;
                //populate with days between now and the end of working week
                for (int i = c; i < 7; i++)
                {
                    var dayNumber = calendar.GetDayOfYear(DateTime.Today.Add(new TimeSpan(i - c, 0, 0, 0)));

                    var dailyTimetable = new DailyTimetable()
                    {
                        Id = Guid.NewGuid(),
                        DayNumber = dayNumber,
                        Day = (DayOfWeek) Enum.ToObject(typeof(DayOfWeek), i),
                        AvailableTimeInHours = userConfig.WeeklyAvailableTimeInHours[i],
                        Assignments = new List<Assignment>(),
                        Week = weeklyTimetable
                    };
                    await _repWrapper.DailyTimetableRepo.CreateItemAsync(dailyTimetable);
                    weeklyTimetable.Timetables.Add(dailyTimetable);
                }

                await _repWrapper
                        .WeeklyTimetableRepo
                        .UpdateItemAsync(weeklyTimetable);
            }
            
            return weeklyTimetable;
        }

        public async Task<DailyTimetable> LoadAssignmentsAsync(DayOfWeek day, int weekOfTheYear = -1)
        {
            if (weekOfTheYear == -1)
            {
                weekOfTheYear = GetCurrentWeekNumber();
            }
            var weeklyTimetable = await LoadWeeklyTimetableAsync(weekOfTheYear);
            var dailyTimetable = weeklyTimetable?.Timetables.Find(x => x.Day == day);
            return dailyTimetable;
        }

        public async Task<IEnumerable<Assignment>> LoadAssignmentsAsync()
        {
            return await _repWrapper.AssignmentRepo.GetAllItemsAsync();
        }

        //TODO: change bool with complex parameter model
        public async Task<bool> CreateAssignmentAsync(Assignment assignment)
        {
            var result = await _repWrapper.AssignmentRepo.CreateItemAsync(assignment);
            if (result == false) return result;
            result = await AllocateTimeSlot(assignment);
            return result;
        }


        private async Task<bool> AllocateTimeSlot(Assignment assignment)
        {
            bool result;
            TimeSpan timeToDeadline = assignment.Deadline - DateTime.Now;
            var currentDayTimetable =
                await LoadAssignmentsAsync(DateTime.Now.DayOfWeek, GetCurrentWeekNumber());
            if (timeToDeadline.TotalDays <= 3 && currentDayTimetable != null)
            {
                if (currentDayTimetable.AvailableTimeInHours * 60 >= assignment.DurationMinutes)
                {
                    assignment.PriorityLevel = PriorityLevel.High;
                    CalculatePriorityCoefficient(ref assignment, currentDayTimetable);
                    CalculateKinbens(ref assignment, currentDayTimetable);
                    currentDayTimetable.Assignments ??= new List<Assignment>();
                    currentDayTimetable.Assignments.Add(assignment);
                    await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);
                    result = await _repWrapper.DailyTimetableRepo.UpdateItemAsync(currentDayTimetable);
                    if (result == false)
                    {
                        result = await MoveAssignmentBackwardsAsync(assignment);
                    }
                }
                else
                {
                    result = await MoveAssignmentForwardAsync(assignment);
                }
            }
            else
            {
                result = await MoveAssignmentForwardAsync(assignment);
            }

            return result;
        }

        private void CalculatePriorityCoefficient(ref Assignment assignment, DailyTimetable todayWeekday)
        {
            int t = assignment.DurationMinutes;
            double rho = assignment.Importance;
            int d = (int) Math.Round((assignment.Deadline - DateTime.Now).TotalMinutes);
            double w = todayWeekday.AvailableTimeInHours * 60;
            var ct = w != 0 ? decimal.Round(new decimal(t / w), 6, MidpointRounding.AwayFromZero) : 1;
            decimal cd = decimal.Round(new decimal(1 - (t / d)), 6, MidpointRounding.AwayFromZero);
            double cp = Math.Pow((double) ct, (double) cd) * rho;
            assignment.PriorityCoefficient = cp;
        }

        
        /// <summary>
        /// Calculates awarding points based on the required time to complete the assignment and available time
        /// </summary>
        /// <param name="assignment">reference to the assignment object</param>
        /// <param name="todayWeekday">today's timetable object</param>
        private void CalculateKinbens(ref Assignment assignment, DailyTimetable todayWeekday)
        {
            int t = assignment.DurationMinutes;
            double w = todayWeekday.AvailableTimeInHours * 60;
            var ct = w != 0 ? decimal.Round(new decimal(t / w), 6, MidpointRounding.AwayFromZero) : 1;
            int kinbens = (int) (200 * ct);
            assignment.Kinbens = kinbens;
        }


        private int GetCurrentWeekNumber()
        {
            return _calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
        }

        public async Task<bool> UpdateAssignmentAsync(Assignment assignment)
        {
            await RemoveAssignmentFromOldTimetable(assignment);
            await AllocateTimeSlot(assignment);
            return await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);
        }

        public async Task<List<Assignment>> SplitAssignmentAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }
        

        public async Task<bool> MoveAssignmentForwardAsync(Assignment assignment, bool nextDay = false)
        {
            //check if there are any days with enough free time for the assignment
            var vacantDaysBeforeDeadline = await GetVacantDaysBeforeDeadline(assignment);
            var daysBeforeDeadlineList = vacantDaysBeforeDeadline.ToList();
            if (daysBeforeDeadlineList.Any())
            {

                var oldDay = await RemoveAssignmentFromOldTimetable(assignment);
                
                //get the first available day and add the assignment to it
                DailyTimetable? availableDay;
                if (nextDay == false)
                {
                    availableDay = daysBeforeDeadlineList
                        .First(x => x.AvailableTimeInHours * 60 >= assignment.DurationMinutes);
                }
                else
                {
                    availableDay = daysBeforeDeadlineList
                        .First(x => x.AvailableTimeInHours * 60 >= assignment.DurationMinutes
                        && x.DayNumber != oldDay!.DayNumber);
                }
                
                //the assignments can then be easily completed and moved
                assignment.PriorityLevel = PriorityLevel.Low;
                CalculatePriorityCoefficient(ref assignment, availableDay);
                CalculateKinbens(ref assignment, availableDay);

                availableDay.Assignments.Add(assignment);
                
                await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);
                
                await _repWrapper.DailyTimetableRepo.UpdateItemAsync(availableDay);
                return true;
            }
            
            //if there are no days with enough free time 
            //we need to move assignments with lower priority to the next day
            else
            {
                #region CanbeUsed
                //TODO: 
                //check tasks with lower priority in each day
                /*
                var lessImportantAssignments = await _repWrapper.AssignmentRepo.GetItemsByQueryAsync(
                    x => x.PriorityCoefficient < assignment.PriorityCoefficient);
                if (lessImportantAssignments == null)
                {
                    return false;
                }
                
                

                foreach (var lessImportantAssignment in lessImportantAssignments)
                {
                    //TODO: get the day timetable of current assignment
                    var day = await _repWrapper.DailyTimetableRepo.GetItemAsync(
                        x => x.Id == lessImportantAssignment.DayId);
                    double freedTme = day.AvailableTimeInHours * 60 + lessImportantAssignment.DurationMinutes;
                    if (freedTme >= assignment.DurationMinutes)
                    {
                        var isSuccess = await MoveAssignmentForwardAsync(lessImportantAssignment);
                        if (isSuccess)
                        {
                            day.Assignments.Add(assignment);
                            await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);
                            await _repWrapper.DailyTimetableRepo.UpdateItemAsync(day);
                        }
                        else
                        {
                            
                        }
                    }
                }
                */
                #endregion
                //TODO: test
                //retrieve less important assignments that can potentially be moved forward
                var lessImportantAssignments = await GetLowPriorityAssignmentsOnDay(assignment);
                //if there are no any ones => the assignments can't be moved forward
                if (lessImportantAssignments == null)
                {
                    return false;
                }
                
                bool beenMovedSuccessfully = false;
                Guid freedDayId = default;
                //iterate through groups of assignments
                //each group represent assignments on the same day
                foreach (var groupOfAssignments in lessImportantAssignments)
                {
                    if (groupOfAssignments == null || !groupOfAssignments.Any())
                        continue;
                    
                    //there may be a case when one of the assignments on the day can't be moved forward,
                    //then we should check the next day
                    //otherwise, no need to check other days
                    //so we quit the loop
                    freedDayId = groupOfAssignments.Select(x => x.DayId).First();
                    foreach (var assignmentToMove in groupOfAssignments)
                    {
                        beenMovedSuccessfully = await MoveAssignmentForwardAsync(assignmentToMove, true);
                    }
                    if (beenMovedSuccessfully)
                    {
                        break;
                    }
                }
                
                //if none of the assignment have been moved successfully,
                //then the assignment can't be moved
                if (!beenMovedSuccessfully)
                {
                    return false;
                }
                
                //otherwise get the day where assignments have been moved forward
                //and add the fresh assignment there
                var day = await _repWrapper.DailyTimetableRepo.GetItemByIdAsync(freedDayId);
                if (day == null) throw new ArgumentNullException();

                assignment.PriorityLevel = PriorityLevel.Medium;
                day.Assignments.Add(assignment);
                await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);
                await RemoveAssignmentFromOldTimetable(assignment);
                await _repWrapper.DailyTimetableRepo.UpdateItemAsync(day);

                return true;
            }
        }


        private async Task<DailyTimetable?> RemoveAssignmentFromOldTimetable(Assignment assignment)
        {
            if (assignment.DayId != default)
            {
                var previousDay = await _repWrapper.DailyTimetableRepo.GetItemAsync(
                    x => x.Id == assignment.DayId);
                previousDay.Assignments.Remove(previousDay.Assignments.Find(x => x.Id == assignment.Id));
                await _repWrapper.DailyTimetableRepo.UpdateItemAsync(previousDay);
                return previousDay;
            }

            return null;
        }


        private async Task<List<List<Assignment>>?> GetLowPriorityAssignmentsOnDay(Assignment assignment, int numberOfTasks = 1)
        {
            //get daily timetables before deadline
            var currentDay = _calendar.GetDayOfYear(DateTime.Now);
            var daysBeforeDeadline = await _repWrapper.DailyTimetableRepo.GetItemsByQueryAsync(
                x => x.DayNumber <= assignment.Deadline.DayOfYear && x.DayNumber >= currentDay);
            
            //as we increase number of assignments we want to move on the day
            //we need to indicate that the number of assignments to move
            //is greater than the number of existing assignments on any day
            bool overflowedNumberOfAssignments = false;

            //convert complex IEnumerable to simple array so it can be easily iterated through
            //TODO: replace with custom Data structure with no memory allocation
            var dailyTimetables = daysBeforeDeadline as DailyTimetable[] ?? daysBeforeDeadline.ToArray();
            
            //here we have list of lists with assignments
            //so we can each individual list indicate assignments on the same day
            List<List<Assignment>> lowPriorityAssignments = new List<List<Assignment>>(dailyTimetables.Length);
            for (int i = 0; i < dailyTimetables.Length; i++)
            {
                //check whether the last day before deadline has enough assignments to move
                overflowedNumberOfAssignments = i == dailyTimetables.Length
                                                && dailyTimetables[i].Assignments.Count() > numberOfTasks;
                lowPriorityAssignments.Add(new List<Assignment>());
                //iterate through assignments on the day
                foreach (var dailyTimetableAssignment in dailyTimetables[i].Assignments)
                {
                    //if we have enough assignments in list, then no need to check further
                    if (numberOfTasks == lowPriorityAssignments[i].Count) break;
                    
                    //check whether there is any point in moving adding the assignments
                    //to the list by comparing the freed time with the duration
                    //of the fresh assignments
                    // double freedTime = dailyTimetables[i].AvailableTimeInHours * 60 -
                    //                    dailyTimetableAssignment.DurationMinutes;
                    
                    //check whether the time required by the assignment
                    //is within 20% boundaries
                    int boundaryDuration = assignment.DurationMinutes;
                    int freedTime = dailyTimetableAssignment.DurationMinutes;
                    
                    if ((freedTime >= boundaryDuration * 0.8 || freedTime <= boundaryDuration * 1.2)
                        && dailyTimetableAssignment.PriorityLevel == PriorityLevel.Low)
                    {
                        lowPriorityAssignments[i].Add(dailyTimetableAssignment);
                    }
                }

                //if total freed time on the day is less than time required by the assignment
                //empty the list
                if (assignment.DurationMinutes > lowPriorityAssignments[i].Select(x => x.DurationMinutes).Sum() + dailyTimetables[i].AvailableTimeInHours * 60)
                {
                    lowPriorityAssignments[i] = new List<Assignment>();
                }
            }
            //if we reach the point when the number is too big, then none of the assignments can be moved
            if (overflowedNumberOfAssignments)
            {
                return null;
            }
            //if none of the assignments can be moved forward, so we can free enough time
            //increase the number of assignments on the day to move and repeat the process recursively
            if (!lowPriorityAssignments.SelectMany(x => x).Any())
            {
                numberOfTasks++;
                return await GetLowPriorityAssignmentsOnDay(assignment, numberOfTasks);
            }
            //otherwise everything is fine => return the list
            return lowPriorityAssignments;
        }

        private async Task<IEnumerable<DailyTimetable>> GetVacantDaysBeforeDeadline(Assignment assignment)
        {
            int deadlineWeekNumber =
                _calendar.GetWeekOfYear(assignment.Deadline, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
            int currentWeekNumber = GetCurrentWeekNumber();
            var weeksBeforeDeadline = new List<WeeklyTimetable>();
            
            //populate list with week object before the deadline
            for (int i = 0; i <= deadlineWeekNumber-currentWeekNumber; i++)
            {
                var week = await LoadWeeklyTimetableAsync(currentWeekNumber + i);
                weeksBeforeDeadline.Add(week);
            }
            
            //search for vacant days in these weeks before the day of deadline
            var vacantDays = weeksBeforeDeadline
                .SelectMany(x => x.Timetables)
                .Select(x => x)
                .Where(x => x.DayNumber <= _calendar.GetDayOfYear(assignment.Deadline) 
                            && x.DayNumber >= _calendar.GetDayOfYear(DateTime.Now)
                            && x.AvailableTimeInHours >= assignment.DurationMinutes / 60.0);
            
            
            return vacantDays;
        }


        public async Task<bool> MoveAssignmentBackwardsAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> UpdateTimetableAsync(DailyTimetable dailyTimetable)
        {
            return await _repWrapper.DailyTimetableRepo.UpdateItemAsync(dailyTimetable);
        }

        public async Task<bool> DeleteAssignmentAsync(Assignment assignment)
        {
            var resultDelete = await _repWrapper.AssignmentRepo.DeleteItemAsync(assignment);
            var dailyTimetable = await _repWrapper.DailyTimetableRepo.GetItemByIdAsync(assignment.DayId);
            dailyTimetable.Assignments.Remove(assignment);
            var resultUpdate = await UpdateTimetableAsync(dailyTimetable);
            return resultDelete && resultUpdate;
        }
    }
}