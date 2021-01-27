using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Helpers;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Services.Database.Repositories
{
    public class AssignmentRepository : BaseRepository<Assignment>
    {
        public AssignmentRepository(DbConnection connection) : base(connection)
        {
            
        }

        public override async Task<IEnumerable<Assignment>> GetAllItemsAsync()
        {
            return await Database.GetAllWithChildrenAsync<Assignment>();
        }

        public override async Task<Assignment> GetItemAsync(Expression<Func<Assignment, bool>> expression)
        {
            return await Database.GetWithChildrenByQueryAsync(expression);
        }

        public override async Task<bool> UpdateItemAsync(Assignment item)
        {
            try
            {
                await Database.UpdateWithChildrenAsync(item);
                return true;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public override async Task<bool> CreateItemAsync(Assignment item)
        {
            if (item.Kinbens < 0) return await Task.FromCanceled<bool>(new CancellationToken(true));
            item.Id = Guid.NewGuid();
            return await base.CreateItemAsync(item);
        }
        
    }
}