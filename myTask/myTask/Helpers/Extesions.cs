using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using myTask.Domain.Models;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using Xamarin.Forms;

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

        public static ImageSource GetImageSource(this Assignment assignment)
        {
            if (assignment.Icon == null)
                return ImageSource.FromFile("placeholder.png");
            return ImageSource.FromStream(() => new MemoryStream(assignment.Icon));
        }
    }
}