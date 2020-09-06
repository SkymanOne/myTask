using myTask.Models;

namespace myTask.Services.Database.Repositories
{
    public class DailyTimetableRepository : BaseRepository<DailyTimetable>
    {
        public DailyTimetableRepository(DbConnection connection) : base(connection)
        {
        }
    }
}