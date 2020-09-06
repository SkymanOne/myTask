using System;
using System.Threading;
using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services.Database.Repositories
{
    public class TagRepository : BaseRepository<Tag>
    {
        public TagRepository(DbConnection connection) : base(connection)
        { }

        public override async Task<bool> UpdateItemAsync(Tag item)
        {
            var children = item.Tasks;
            if (children.Count == 0) return await DeleteItemAsync(item);
            return await base.UpdateItemAsync(item);
        }

        public override async Task<Tag> DeleteItemAsync(object id)
        {
            if (id is Guid guid)
            {
                return await base.DeleteItemAsync(guid);
            }
            return await Task.FromCanceled<Tag>(new CancellationToken(true));
        }
    }
}