using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.MyTasksManager
{
    /// <summary>
    /// Interface describing the basic functionality of the core
    /// Contains algorithms and equations for task management
    /// </summary>
    public interface IAssignmentsManager
    {
        Task<ICollection<Assignment>> LoadAllAssignmentsAsync();
        Task<ICollection<DailyTimetable>> LoadWeeklyTimetableAsync(int weekOfTheYear);
        Task<ICollection<Assignment>> LoadAssignmentsAsync(Weekday day, int weekOfTheYear = -1);
        Task<ICollection<Assignment>> LoadAssignmentsAsync();
        Task<bool> CreateAssigmentAsync(Assignment assignment);
        Task<bool> UpdateAssignmentAsync(Assignment assignment);
        Task<List<Assignment>> SplitAssignmentAsync(Assignment assignment);
        Task<bool> MoveAssignmentForwardAsync(Assignment assignment);
        Task<bool> MoveAssignmentBackwardsAsync(Assignment assignment);
        Task UpdateTimetableAsync();
    }
}