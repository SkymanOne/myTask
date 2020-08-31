using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace myTask.DataHandling.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAllItemsAsync();
        Task<IEnumerable<T>> GetItemsByQuery(Expression<Func<T, bool>> expression);
        Task<T> GetItem(Expression<Func<T, bool>> expression);
        Task<bool> CreateItem(T item);
        Task<bool> UpdateItem(T item);
        Task<bool> DeleteItem(T item);
        Task<T> DeleteItem(int id);
        //not sure whether it is needed
        //Task<T> SaveChanges();
    }
}