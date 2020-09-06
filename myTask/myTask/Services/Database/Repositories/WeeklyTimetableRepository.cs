using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.Database.Repositories
{
    public class WeeklyTimetableRepository : BaseRepository<WeeklyTimetable>
    {
        public WeeklyTimetableRepository(DbConnection connection) : base(connection)
        {
        }

        public override async Task<bool> UpdateItemAsync(WeeklyTimetable item)
        {
            if (item.Timetables.Count == 0)
            {
                return await base.DeleteItemAsync(item);
            }
            return await base.UpdateItemAsync(item);
        }
    }
}