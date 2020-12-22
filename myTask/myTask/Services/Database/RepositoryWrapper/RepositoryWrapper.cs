using myTask.Models;
using myTask.Services.Database.Repositories;

namespace myTask.Services.Database.RepositoryWrapper
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IRepository<Assignment> _assignmentRepo;
        private IRepository<Tag> _tagRepo;
        private IRepository<DailyTimetable> _dailyTimetableRepo;
        private IRepository<WeeklyTimetable> _weeklyTimetableRepo;

        public RepositoryWrapper(IRepository<Assignment> assignmentRepo, IRepository<Tag> tagRepo,
            IRepository<DailyTimetable> dailyTimetableRepo, IRepository<WeeklyTimetable> weeklyTimetableRepo)
        {
            _assignmentRepo = assignmentRepo;
            _tagRepo = tagRepo;
            _dailyTimetableRepo = dailyTimetableRepo;
            _weeklyTimetableRepo = weeklyTimetableRepo;
        }

        public IRepository<Assignment> AssignmentRepo => _assignmentRepo;

        public IRepository<Tag> TagRepo => _tagRepo;

        public IRepository<DailyTimetable> DailyTimetableRepo => _dailyTimetableRepo;

        public IRepository<WeeklyTimetable> WeeklyTimetableRepo => _weeklyTimetableRepo;
    }
}