using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Services.Database.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        protected readonly SQLiteAsyncConnection Database;

        public BaseRepository(DbConnection connection)
        {
            Database = connection.Database;
        }
        
        public virtual async Task<IEnumerable<T>> GetAllItemsAsync()
        {
            return await Database.Table<T>().ToListAsync();
        }

        public virtual async Task<T> GetItemByIdAsync(object id)
        {
            return await Database.GetAsync<T>(id);
        }

        protected async Task LoadChildrenAsync(T item)
        {
            await Database.GetChildrenAsync(item, true);
        }

        public virtual async Task<IEnumerable<T>> GetItemsByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return await Database.Table<T>().Where(expression).ToListAsync();
        }

        public virtual async Task<T> GetItemAsync(Expression<Func<T, bool>> expression)
        {
            return await Database.Table<T>().FirstOrDefaultAsync(expression);
        }

        public virtual async Task<bool> CreateItemAsync(T item)
        {
            var i = await Database.InsertAsync(item, typeof(T));
            return i != 0;
        }

        public virtual async Task<bool> UpdateItemAsync(T item)
        {
            var i = await Database.UpdateAsync(item, typeof(T));
            return i != 0;
        }

        public virtual async Task<bool> DeleteItemAsync(T item)
        {
            var i = await Database.DeleteAsync(item);
            return i != 0;
        }

        public virtual async Task<T> DeleteItemAsync(object id)
        {
            var item = await Database.GetAsync<T>(id);
            var i = await Database.DeleteAsync<T>(id);
            return i != 0 ? item : default;
        }
    }
}