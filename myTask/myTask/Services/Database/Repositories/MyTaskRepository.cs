using System.Threading;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.Database.Repositories
{
    public class MyTaskRepository : BaseRepository<MyTask>
    {
        public MyTaskRepository(DbConnection connection) : base(connection)
        {
            
        }

        public override async Task<bool> CreateItemAsync(MyTask item)
        {
            if (item.Kinbens < 0) return await Task.FromCanceled<bool>(new CancellationToken(true));
            return await base.CreateItemAsync(item);
        }
        
    }
}