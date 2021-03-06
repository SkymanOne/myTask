using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Helpers;
using myTask.Services.UserConfigManager;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Services.Database.Repositories
{
    public class DailyTimetableRepository : BaseRepository<DailyTimetable>
    {
        private readonly IUserConfigManager _userConfigManager;
        public DailyTimetableRepository(DbConnection connection, IUserConfigManager userConfigManager) : base(connection)
        {
            _userConfigManager = userConfigManager;
        }

        public override async Task<DailyTimetable> GetItemAsync(Expression<Func<DailyTimetable, bool>> expression)
        {
            try
            {
                var item = await Database.GetWithChildrenByQueryAsync(expression);
                await LoadChildrenAsync(item);
                return item;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public override async Task<DailyTimetable> GetItemByIdAsync(object id)
        {
            try
            {
                return await Database.GetWithChildrenAsync<DailyTimetable>(id, true);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public override async Task<IEnumerable<DailyTimetable>> GetItemsByQueryAsync(Expression<Func<DailyTimetable, bool>> expression)
        {
            return await Database.GetAllWithChildrenAsync(expression);
        }

        public override async Task<bool> UpdateItemAsync(DailyTimetable item)
        {
            var config = await _userConfigManager.GetConfigAsync();
            var freeTimeLeft = 
                config.WeeklyAvailableTimeInHours.ElementAt((int)item.Day) - item.Assignments.Sum(x => x.DurationMinutes / 60.0);
            if (freeTimeLeft >= 0)
            {
                item.AvailableTimeInHours = freeTimeLeft;
                await Database.UpdateWithChildrenAsync(item);
                return true;
            }

            return false;
        }
    }
}