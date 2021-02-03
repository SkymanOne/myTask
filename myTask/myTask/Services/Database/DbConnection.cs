using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using myTask.Domain.Models;
using SQLite;

namespace myTask.Services.Database
{
    public class DbConnection
    {
        private static readonly Lazy<SQLiteAsyncConnection> LazyDbInitializer =
            new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(Constants.DB_PATH));

        public SQLiteAsyncConnection Database { get; } = LazyDbInitializer.Value;
        private bool _setup = false;

        public async Task Init()
        {

            var tableTypes = new[]
            {
                typeof(Tag),
                typeof(Assignment),
                typeof(AssignmentTag),
                typeof(DailyTimetable),
                typeof(WeeklyTimetable)
            };
            if (!_setup)
            { 
                //check if each model type in the list has a corresponding table in the db
                //if not, drop db and create a new one
                bool missedTable = Database.TableMappings.Count() != tableTypes.Length;
                foreach (var tableMapping in Database.TableMappings)
                {
                    if (tableTypes.FirstOrDefault(x => x.Name == tableMapping.TableName) == null)
                        missedTable = true;
                    break;

                }
                if (missedTable)
                {
                    await Database.CreateTablesAsync(CreateFlags.None, tableTypes).ConfigureAwait(false);
                }
                _setup = true;
            }
        }
    }
}