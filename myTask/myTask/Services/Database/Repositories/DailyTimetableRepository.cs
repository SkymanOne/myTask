using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Services.UserConfigManager;

namespace myTask.Services.Database.Repositories
{
    public class DailyTimetableRepository : BaseRepository<DailyTimetable>
    {
        private readonly IUserConfigManager _userUserConfigManager;
        public DailyTimetableRepository(DbConnection connection, IUserConfigManager userUserConfigManager) : base(connection)
        {
        }

        public override async Task<bool> UpdateItemAsync(DailyTimetable item)
        {
            var config = await _userUserConfigManager.GetConfig();
            var freeTimeLeft = 
                config.WeeklyAvailableTimeInHours.ElementAt((int)item.Day) - item.Assignments.Sum(x => x.DurationMinutes / 60);
            if (freeTimeLeft < 0)
                return false;
            return await base.UpdateItemAsync(item);
        }
    }
}