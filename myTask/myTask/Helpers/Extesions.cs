using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
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
        public static string ToFriendlyString(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Sunday";
                case DayOfWeek.Monday:
                    return "Monday";
                case DayOfWeek.Tuesday:
                    return "Tuesday";
                case DayOfWeek.Wednesday:
                    return "Wednesday";
                case DayOfWeek.Thursday:
                    return "Thursday";
                case DayOfWeek.Friday:
                    return "Friday";
                case DayOfWeek.Saturday:
                    return "Saturday";
                default:
                    return "";
            }
        }
    }
}