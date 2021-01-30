using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Helpers;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Services.Database.Repositories
{
    public class TagRepository : BaseRepository<Tag>
    {
        public TagRepository(DbConnection connection) : base(connection)
        { }

        public override async Task<Tag> GetItemAsync(Expression<Func<Tag, bool>> expression)
        {
            try
            {
                return await Database.GetWithChildrenByQueryAsync(expression);
            }
            catch (InvalidOperationException)
            {
            }

            return null;
        }

        public override async Task<bool> UpdateItemAsync(Tag item)
        {
            if (item.Assignments.Count == 0) return await DeleteItemAsync(item);
            await Database.UpdateWithChildrenAsync(item);
            return true;
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