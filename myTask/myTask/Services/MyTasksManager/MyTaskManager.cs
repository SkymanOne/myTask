using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.MyTasksManager
{
    public class MyTaskManager : IMyTaskManager
    {
        public async Task<ICollection<MyTask>> LoadAllTasksAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ICollection<DailyTimetable>> LoadWeeklyTimetableAsync(int weekOfTheYear)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ICollection<MyTask>> LoadTasksAsync(Weekday day, int weekOfTheYear = -1)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ICollection<MyTask>> LoadTasksAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> CreateTaskAsync(MyTask myTask)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> UpdateTaskAsync(MyTask myTask)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<MyTask>> SplitTaskAsync(MyTask myTask)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> MoveTaskForwardAsync(MyTask myTask)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> MoveTaskBackwardsAsync(MyTask myTask)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateTimetableAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}