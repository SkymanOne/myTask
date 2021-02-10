using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Domain.Models;

namespace myTask.Services.AssignmentsManager
{
    /// <summary>
    /// Interface describing the basic functionality of the core
    /// Contains algorithms and equations for task management
    /// </summary>
    public interface IAssignmentsManager
    {
        Task Init();
        Task<IEnumerable<Assignment>> LoadAllAssignmentsAsync();
        Task<WeeklyTimetable> LoadWeeklyTimetableAsync(int weekOfTheYear);
        Task<DailyTimetable> LoadAssignmentsAsync(DayOfWeek day, int weekOfTheYear = -1);
        Task<IEnumerable<Assignment>> LoadAssignmentsAsync();
        Task<bool> CreateAssigmentAsync(Assignment assignment);
        Task<bool> UpdateAssignmentAsync(Assignment assignment);
        Task<bool> DeleteAssignmentAsync(Assignment assignment);
        Task<List<Assignment>> SplitAssignmentAsync(Assignment assignment);
        Task<bool> MoveAssignmentForwardAsync(Assignment assignment);
        Task<bool> MoveAssignmentBackwardsAsync(Assignment assignment);
        Task<bool> UpdateTimetableAsync(DailyTimetable dailyTimetable);
    }
}