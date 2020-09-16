using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace myTask.Helpers
{
    public static class Extesions
    {
        public static async Task<T> GetWithChildrenByQueryAsync<T>(this SQLiteAsyncConnection conn, Expression<Func<T, bool>> expression) where T : class, new()
        {
            var item = await conn.GetAsync(expression);
            await conn.GetChildrenAsync(item);
            return item;
        }
    }
}