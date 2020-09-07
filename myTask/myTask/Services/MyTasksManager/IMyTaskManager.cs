using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.MyTasksManager
{
    /// <summary>
    /// Interface describing the basic functionality of the core
    /// Contains algorithms and equations for task management
    /// </summary>
    public interface IMyTaskManager
    {
        Task<ICollection<MyTask>> LoadAllTasksAsync();
        Task<ICollection<DailyTimetable>> LoadWeeklyTimetableAsync(int weekOfTheYear);
        Task<ICollection<MyTask>> LoadTasksAsync(Weekday day, int weekOfTheYear = -1);
        Task<ICollection<MyTask>> LoadTasksAsync();
        Task<bool> CreateTaskAsync(MyTask myTask);
        Task<bool> UpdateTaskAsync(MyTask myTask);
        Task<List<MyTask>> SplitTaskAsync(MyTask myTask);
        Task<bool> MoveTaskForwardAsync(MyTask myTask);
        Task<bool> MoveTaskBackwardsAsync(MyTask myTask);
        Task UpdateTimetableAsync();
    }
}