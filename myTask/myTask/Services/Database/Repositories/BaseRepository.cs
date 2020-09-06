using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace myTask.Services.Database.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        protected readonly DbConnection Connection;

        public BaseRepository(DbConnection connection)
        {
            Connection = connection;
        }
        
        public virtual async Task<IEnumerable<T>> GetAllItemsAsync()
        {
            return await Connection.Database.Table<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetItemsByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return await Connection.Database.Table<T>().Where(expression).ToListAsync();
        }

        public virtual async Task<T> GetItemAsync(Expression<Func<T, bool>> expression)
        {
            return await Connection.Database.Table<T>().FirstOrDefaultAsync(expression);
        }

        public virtual async Task<bool> CreateItemAsync(T item)
        {
            var i = await Connection.Database.InsertAsync(item, typeof(T));
            return i != 0;
        }

        public virtual async Task<bool> UpdateItemAsync(T item)
        {
            var i = await Connection.Database.UpdateAsync(item, typeof(T));
            return i != 0;
        }

        public virtual async Task<bool> DeleteItemAsync(T item)
        {
            var i = await Connection.Database.DeleteAsync(item);
            return i != 0;
        }

        public virtual async Task<T> DeleteItemAsync(object id)
        {
            var item = await Connection.Database.GetAsync<T>(id);
            var i = await Connection.Database.DeleteAsync<T>(id);
            return i != 0 ? item : default;
        }
    }
}