using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using myTask.Services.Database.Repositories;
using Xamarin.Forms.Internals;

namespace myTask.Services.Database.MockRepositories
{
    public class MockRepository<T> : IRepository<T> where T : class, new()
    {
        private static readonly Lazy<List<T>> LazyDatasourceInit = new Lazy<List<T>>(new List<T>());
        private List<T> Datasource { get; set; } = LazyDatasourceInit.Value;
        public Task<IEnumerable<T>> GetAllItemsAsync()
        {
            var result = Datasource;
            return Task.FromResult(result as IEnumerable<T>);
        }

        public Task<IEnumerable<T>> GetItemsByQueryAsync(Expression<Func<T, bool>> expression)
        {
            var result = Datasource.Where(expression.Compile());
            return Task.FromResult(result);
        }

        public Task<T> GetItemAsync(Expression<Func<T, bool>> expression)
        {
            var result = Datasource.FirstOrDefault(expression.Compile());
            return Task.FromResult(result);
        }

        public Task<T> GetItemByIdAsync(object id)
        {
            return Task.FromResult<T>(null);
        }

        public Task<bool> CreateItemAsync(T item)
        {
            Datasource.Add(item);
            return Task.FromResult<bool>(true);
        }

        public virtual Task<bool> UpdateItemAsync(T item)
        {
            return Task.FromResult(false);
        }

        public Task<bool> DeleteItemAsync(T item)
        {
            Datasource.Remove(item);
            return Task.FromResult(true);
        }

        public Task<T> DeleteItemAsync(object id)
        {
            return Task.FromResult<T>(null);
        }

        public Task<bool> UpdateItemAsync(T item, T oldItem)
        {
            int i = Datasource.IndexOf(oldItem);
            Datasource[i] = item;
            return Task.FromResult(true);
        }
    }
}