using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using myTask.Models;
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

        public AssignmentsManager(IRepositoryWrapper repositoryWrapper, IUserConfigManager userConfigManager)
        {
            _repWrapper = repositoryWrapper;
            _userConfigManager = userConfigManager;
        }

        public async Task Init()
        {
            Calendar calendar = new GregorianCalendar();
        }

        public async Task<IEnumerable<Assignment>> LoadAllAssignmentsAsync()
        {
            return await _repWrapper.AssignmentRepo.GetAllItemsAsync();
        }

        public async Task<WeeklyTimetable> LoadWeeklyTimetableAsync(int weekOfTheYear)
        {
            var weeklyTimetable = await _repWrapper.WeeklyTimetableRepo.GetItemAsync(x => x.Number == weekOfTheYear);
            if (weeklyTimetable == null)
            {
                //if we are trying to access the week before today which doesn't exist
                //no point of creating it, so just return null
                if (weekOfTheYear < DateTime.Now.DayOfYear / 7) return null;
                
                UserConfig userConfig = await _userConfigManager.GetConfig();
                DailyTimetable[] dailyTimetables = new DailyTimetable[7];
                Calendar calendar = new GregorianCalendar();
                for (int i = 0; i < 7; i++)
                {
                    int daysFromSunday = (int)DayOfWeek.Sunday - (int)DateTime.Now.DayOfWeek;
                    var dayNumber = calendar.GetDayOfYear(DateTime.Today.Subtract(new TimeSpan(daysFromSunday, 0, 0, 0)));
                    var dailyTimetable = new DailyTimetable()
                    {
                        Id = new Guid(),
                        DayNumber = dayNumber,
                        Day = (DayOfWeek) Enum.ToObject(typeof(DayOfWeek), i),
                        AvailableTimeInHours = userConfig.WeeklyAvailableTimeInHours[i],
                        Assignments = new List<Assignment>()
                    };
                    dailyTimetables[i] = dailyTimetable;
                }

                weeklyTimetable = new WeeklyTimetable()
                {
                    Id = new Guid(),
                    Number = weekOfTheYear,
                    Year = DateTime.Now.Year,
                    Timetables = dailyTimetables.ToList()
                };
                await _repWrapper.WeeklyTimetableRepo.CreateItemAsync(weeklyTimetable);
            }
            
            return weeklyTimetable;
        }

        public async Task<DailyTimetable> LoadAssignmentsAsync(DayOfWeek day, int weekOfTheYear = -1)
        {
            if (weekOfTheYear == -1) weekOfTheYear = DateTime.Now.DayOfYear / 7;
            var weeklyTimetable = await _repWrapper.WeeklyTimetableRepo.GetItemAsync(
                x => x.Number == weekOfTheYear);
            var dailyTimetable = weeklyTimetable?.Timetables.Find(x => x.Day == day);
            return dailyTimetable;
        }

        public async Task<IEnumerable<Assignment>> LoadAssignmentsAsync()
        {
            return await _repWrapper.AssignmentRepo.GetAllItemsAsync();
        }

        //TODO: change bool with complex parameter model
        public async Task<bool> CreateAssigmentAsync(Assignment assignment)
        {
            var result = await _repWrapper.AssignmentRepo.CreateItemAsync(assignment);
            if (result == false) return result;
            TimeSpan timeToDeadline = DateTime.Now - assignment.Deadline;
            if (timeToDeadline.TotalDays <= 3)
            {
                var currentDayTimetable =
                    await LoadAssignmentsAsync(DateTime.Now.DayOfWeek, DateTime.Today.DayOfYear / 7);
                if (currentDayTimetable.AvailableTimeInHours * 60 <= assignment.DurationMinutes)
                {
                    assignment.PriorityLevel = PriorityLevel.High;
                    CalculatePriorityCoefficient(ref assignment, currentDayTimetable);
                    currentDayTimetable.Assignments.Add(assignment);
                    result = await _repWrapper.DailyTimetableRepo.UpdateItemAsync(currentDayTimetable);
                }
                else
                {
                    result = await MoveAssignmentForwardAsync(assignment);
                }
            }

            return result;
        }

        private void CalculatePriorityCoefficient(ref Assignment assignment, DailyTimetable todayWeekday)
        {
            int t = assignment.DurationMinutes;
            double rho = assignment.Importance;
            int d = (int) Math.Round((DateTime.Now - assignment.Deadline).TotalMinutes);
            double w = todayWeekday.AvailableTimeInHours;
            decimal ct = decimal.Round(new decimal(t / w), 6, MidpointRounding.AwayFromZero);
            decimal cd = decimal.Round(new decimal(1 - (t / d)), 6, MidpointRounding.AwayFromZero);
            double cp = Math.Pow((double) ct, (double) cd) * rho;
            assignment.PriorityCoefficient = cp;
        }

        public async Task<bool> UpdateAssignmentAsync(Assignment assignment)
        {
            return await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);
        }

        public async Task<List<Assignment>> SplitAssignmentAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> MoveAssignmentForwardAsync(Assignment assignment)
        {
            //check if there are any days with enough free time for the assignment
            var vacantDaysBeforeDeadline = (List<DailyTimetable>) await GetVacantDaysBeforeDeadline(assignment);
            if (vacantDaysBeforeDeadline.Any())
            {
                //the assignments can then be easily completed and moved
                assignment.PriorityLevel = PriorityLevel.Low;
                //load today in order to calculate the priority coefficient
                var currentDayTimetable =
                    await LoadAssignmentsAsync(DateTime.Now.DayOfWeek, DateTime.Today.DayOfYear / 7);
                CalculatePriorityCoefficient(ref assignment, currentDayTimetable);
                await _repWrapper.AssignmentRepo.UpdateItemAsync(assignment);

                //get the first available day and add the assignment to it
                var availableDay = vacantDaysBeforeDeadline
                    .First(x => x.AvailableTimeInHours * 60 < assignment.DurationMinutes);
                availableDay.Assignments.Add(assignment);
                
                await RemoveAssignmentFromOldTimetable(assignment);
                
                await _repWrapper.DailyTimetableRepo.UpdateItemAsync(availableDay);
                return true;
            }
            
            //if there are no days with enough free time 
            //we need to move assignments with lower priority to the next 
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
                    //there may be a case when one of the assignments on the day can't be moved forward,
                    //then we should check the next day
                    //otherwise, no need to check other days
                    //so we quit the loop
                    if (beenMovedSuccessfully)
                    {
                        freedDayId = groupOfAssignments.Select(x => x.DayId).First();
                        break;
                    }
                    foreach (var assignmentToMove in groupOfAssignments)
                    {
                        beenMovedSuccessfully = await MoveAssignmentForwardAsync(assignmentToMove);
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


        private async Task<bool> RemoveAssignmentFromOldTimetable(Assignment assignment)
        {
            if (assignment.DayId != default)
            {
                var previousDay = await _repWrapper.DailyTimetableRepo.GetItemAsync(
                    x => x.Id == assignment.DayId);
                previousDay.Assignments.Remove(assignment);
                var result = await _repWrapper.DailyTimetableRepo.UpdateItemAsync(previousDay);
                return result;
            }

            return false;
        }


        private async Task<List<List<Assignment>>> GetLowPriorityAssignmentsOnDay(Assignment assignment, int numberOfTasks = 1)
        {
            //get daily timetables before deadline
            var daysBeforeDeadline = await _repWrapper.DailyTimetableRepo.GetItemsByQueryAsync(
                x => x.DayNumber <= assignment.Deadline.DayOfYear);
            
            //as we increase number of assignments we want to move on the day
            //we need to have to indicate that the number of assignments to move
            //is greater that the number of existing assignments on any day
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
                
                //iterate through assignments on the day
                foreach (var dailyTimetableAssignment in dailyTimetables[i].Assignments)
                {
                    //if we have enough assignments in list, then no need to check further
                    if (numberOfTasks == lowPriorityAssignments[i].Count) break;
                    
                    //check whether there is any point in moving adding the assignments
                    //to the list by comparing the freed time with the duration
                    //of the fresh assignments
                    double freedTime = dailyTimetables[i].AvailableTimeInHours * 60 -
                                       dailyTimetableAssignment.DurationMinutes;
                    if (freedTime <= assignment.DurationMinutes 
                        && dailyTimetableAssignment.PriorityLevel == PriorityLevel.Low)
                    {
                        lowPriorityAssignments[i].Add(dailyTimetableAssignment);
                    }
                }
            }

            //if none of the assignments can be moved forward, so we can free enough time
            //increase the number of assignments on the dat to move and repeat the process recursively
            if (!lowPriorityAssignments.SelectMany(x => x).Any() && !overflowedNumberOfAssignments)
            {
                numberOfTasks++;
                lowPriorityAssignments = await GetLowPriorityAssignmentsOnDay(assignment, numberOfTasks);
            }
            //if we reach the point when the number is too big, then none of the assignments can be moved
            else
            {
                lowPriorityAssignments = null;
            }

            return lowPriorityAssignments;
        }

        private async Task<IEnumerable<DailyTimetable>> GetVacantDaysBeforeDeadline(Assignment assignment)
        {
            int deadlineWeekNumber = assignment.Deadline.DayOfYear / 7;
            var weeksBeforeDeadline = from week in await _repWrapper.WeeklyTimetableRepo.GetAllItemsAsync()
                where
                    week.Year <= assignment.Deadline.Year &&
                    week.Number < deadlineWeekNumber
                select week.Timetables;
            var vacantDays = weeksBeforeDeadline
                .SelectMany(x => x).Where(x => x.Week.Number <= deadlineWeekNumber
                                               && x.Day <= assignment.Deadline.DayOfWeek);
            return vacantDays;
        }

        public async Task<bool> MoveAssignmentBackwardsAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateTimetableAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}