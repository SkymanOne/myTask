using System;
using System.Linq;
using System.Threading.Tasks;
using myTask.Models;
using SQLite;

namespace myTask.Services.Database
{
    public class DbConnection
    {
        private static readonly Lazy<SQLiteAsyncConnection> LazyDbInitializer =
            new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(Constants.DB_PATH));

        public SQLiteAsyncConnection Database { get; } = LazyDbInitializer.Value;
        private bool _setup = false;

        private async Task Init()
        {
            var tableTypes = new[]
            {
                typeof(Tag),
                typeof(MyTask),
                typeof(DailyTimetable),
                typeof(WeeklyTimetable)
            };
            if (!_setup)
            { 
                //check if each model type in the list has a corresponding table in the db
                //if not, drop db and create a new one
                if (!tableTypes.Any(x => Database
                    .TableMappings
                    .Any(z => z.TableName == x.Name))) {
                    await Database.CreateTablesAsync(CreateFlags.None, tableTypes);
                }
                _setup = true;
            }
        }

        public DbConnection()
        {
            Init().ConfigureAwait(false);
        }
    }
}