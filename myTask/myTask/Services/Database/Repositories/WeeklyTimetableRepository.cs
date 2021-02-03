using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Helpers;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Services.Database.Repositories
{
    public class WeeklyTimetableRepository : BaseRepository<WeeklyTimetable>
    {
        public WeeklyTimetableRepository(DbConnection connection) : base(connection)
        {
        }

        public override async Task<WeeklyTimetable> GetItemAsync(Expression<Func<WeeklyTimetable, bool>> expression)
        {
            try
            {
                return await Database.GetWithChildrenByQueryAsync(expression);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public override async Task<bool> UpdateItemAsync(WeeklyTimetable item)
        {
            if (item.Timetables.Count == 0)
            {
                return await base.DeleteItemAsync(item);
            }
            await Database.UpdateWithChildrenAsync(item);
            return true;
        }
    }
}