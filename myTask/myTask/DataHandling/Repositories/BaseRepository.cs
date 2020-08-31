using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using myTask.Services.Database;

namespace myTask.DataHandling.Repositories
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

        public virtual async Task<IEnumerable<T>> GetItemsByQuery(Expression<Func<T, bool>> expression)
        {
            return await Connection.Database.Table<T>().Where(expression).ToListAsync();
        }

        public virtual async Task<T> GetItem(Expression<Func<T, bool>> expression)
        {
            return await Connection.Database.Table<T>().FirstOrDefaultAsync(expression);
        }

        public virtual async Task<bool> CreateItem(T item)
        {
            var i = await Connection.Database.InsertAsync(item, typeof(T));
            return i != 0;
        }

        public virtual async Task<bool> UpdateItem(T item)
        {
            var i = await Connection.Database.UpdateAsync(item, typeof(T));
            return i != 0;
        }

        public virtual async Task<bool> DeleteItem(T item)
        {
            var i = await Connection.Database.DeleteAsync(item);
            return i != 0;
        }

        public virtual async Task<T> DeleteItem(int id)
        {
            var item = await Connection.Database.GetAsync<T>(id);
            var i = await Connection.Database.DeleteAsync<T>(id);
            return i != 0 ? item : default;
        }
    }
}