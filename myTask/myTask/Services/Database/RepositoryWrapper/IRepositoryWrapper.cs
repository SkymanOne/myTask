using myTask.Domain.Models;
using myTask.Services.Database.Repositories;

namespace myTask.Services.Database.RepositoryWrapper
{
    public interface IRepositoryWrapper
    {
        IRepository<Assignment> AssignmentRepo { get; }
        IRepository<Tag> TagRepo { get; }
        IRepository<DailyTimetable> DailyTimetableRepo { get; }
        IRepository<WeeklyTimetable> WeeklyTimetableRepo { get; }
    }
}