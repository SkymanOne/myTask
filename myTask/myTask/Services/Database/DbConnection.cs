using System;
using System.Linq;
using System.Threading.Tasks;
using myTask.Models;
using SQLite;
using Xamarin.Forms.Internals;

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

        public DbConnection()
        {
            Init().SafeFireAndForget(false);
        }
    }
    
    
    //Source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/data/databases
    public static class TaskExtensions
    {
        // NOTE: Async void is intentional here. This provides a way
        // to call an async method from the constructor while
        // communicating intent to fire and forget, and allow
        // handling of exceptions
        public static async void SafeFireAndForget(this Task task,
            bool returnToCallingContext,
            Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(returnToCallingContext);
            }

            // if the provided action is not null, catch and
            // pass the thrown exception
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }
    }
}