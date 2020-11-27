using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using myTask.Helpers;
using myTask.Models;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Services.Database.Repositories
{
    public class MyTaskRepository : BaseRepository<MyTask>
    {
        public MyTaskRepository(DbConnection connection) : base(connection)
        {
            
        }

        public override async Task<MyTask> GetItemAsync(Expression<Func<MyTask, bool>> expression)
        {
            return await Database.GetWithChildrenByQueryAsync(expression);
        }

        public override async Task<bool> DeleteItemAsync(MyTask item)
        {
            return await base.DeleteItemAsync(item);
        }

        public override async Task<bool> CreateItemAsync(MyTask item)
        {
            if (item.Kinbens < 0) return await Task.FromCanceled<bool>(new CancellationToken(true));
            item.Id = Guid.NewGuid();
            return await base.CreateItemAsync(item);
        }
        
    }
}