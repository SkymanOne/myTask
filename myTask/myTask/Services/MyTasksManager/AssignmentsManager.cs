using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.MyTasksManager
{
    public class AssignmentsManager : IAssignmentsManager
    {
        public async Task<ICollection<Assignment>> LoadAllAssignmentsAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ICollection<DailyTimetable>> LoadWeeklyTimetableAsync(int weekOfTheYear)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ICollection<Assignment>> LoadAssignmentsAsync(Weekday day, int weekOfTheYear = -1)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ICollection<Assignment>> LoadAssignmentsAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> CreateAssigmentAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> UpdateAssignmentAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Assignment>> SplitAssignmentAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> MoveAssignmentForwardAsync(Assignment assignment)
        {
            throw new System.NotImplementedException();
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