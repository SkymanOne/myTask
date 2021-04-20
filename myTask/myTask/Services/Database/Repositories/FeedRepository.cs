using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Domain.Models;

namespace myTask.Services.Database.Repositories
{
    public class FeedRepository : BaseRepository<UserUpdate>, IExtendedRepository<UserUpdate>
    {

        public FeedRepository(DbConnection connection) : base(connection)
        {
        }

        public async Task<IEnumerable<UserUpdate>> GetRecentItemsByPageAsync(int number, int page)
        {
            try
            {
                return await Database.Table<UserUpdate>()
                    .OrderByDescending(x => x.DateTime)
                    .Skip((page - 1) * number).Take(number)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                return new List<UserUpdate>();
            }
        }
    }
}