using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.MyTasksManager
{
    public interface IMyTaskManager
    {
        Task<ICollection<MyTask>> LoadAllTasksAsync();
        Task<ICollection<MyTask>> LoadWeeklyTimetable(int weekOfTheYear);
        Task<ICollection<MyTask>> LoadTasksAsync(Weekday day, int weekOfTheYear);
        Task<ICollection<MyTask>> LoadTasksAsync(Weekday day);
        Task<ICollection<MyTask>> LoadTasksAsync();

        Task<bool> CreateTaskAsync(MyTask myTask);
        Task<bool> UpdateTaskAsync(MyTask myTask);
    }
}