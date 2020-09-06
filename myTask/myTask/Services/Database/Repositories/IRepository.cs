using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace myTask.Services.Database.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAllItemsAsync();
        Task<IEnumerable<T>> GetItemsByQueryAsync(Expression<Func<T, bool>> expression);
        Task<T> GetItemAsync(Expression<Func<T, bool>> expression);
        Task<bool> CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(T item);
        Task<T> DeleteItemAsync(object id);
        //not sure whether it is needed
        //Task<T> SaveChanges();
    }
}