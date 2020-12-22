using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.Database.Repositories
{
    public class DailyTimetableRepository : BaseRepository<DailyTimetable>
    {
        private readonly IUserConfig _userUserConfig;
        public DailyTimetableRepository(DbConnection connection, IUserConfig userUserConfig) : base(connection)
        {
        }

        public override async Task<bool> UpdateItemAsync(DailyTimetable item)
        {
            var config = await _userUserConfig.GetConfig();
            var freeTimeLeft = 
                config.WeeklyAvailableTimeInHours.ElementAt((int)item.Day) - item.Assignments.Sum(x => x.DurationMinutes / 60);
            if (freeTimeLeft < 0)
                return false;
            return await base.UpdateItemAsync(item);
        }
    }
}