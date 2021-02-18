using System;
using System.IO;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Services.AssignmentsManager;
using myTask.Services.Database;
using myTask.Services.Database.MockRepositories;
using myTask.Services.Database.Repositories;
using myTask.Services.Database.RepositoryWrapper;
using myTask.Services.FeedService;
using myTask.Services.Navigation;
using myTask.Services.UserConfigManager;
using myTask.ViewModels;
using TinyIoC;

namespace myTask
{
    public static class SuperContainer
    {
        private static readonly TinyIoCContainer Container;

        static SuperContainer()
        {
            Container = TinyIoCContainer.Current;

            //register viewmodels as multi instances
            Container.Register<AssignmentListViewModel>();
            Container.Register<AssignmentDetailViewModel>();
            Container.Register<MainNavigationViewModel>();
            Container.Register<ProgressViewModel>();
            Container.Register<TimeTableViewModel>();
            Container.Register<FeedViewModel>();
            Container.Register<SetWorkingDaysViewModel>();
            Container.Register<SetWorkingHoursViewModel>();
            Container.Register<InitNavViewModel>();

            //register services
            Container.Register<INavigationService, NavigationService>();
        }

        public static void UpdateDependencies(bool useMocks)
        {
            if (useMocks)
            {
                FileInfo info = new FileInfo(Constants.DB_PATH);
                info.Delete();
                //Container.Register(typeof(IRepository<>), typeof(MockRepository<>));
            }
            Container.Register<IRepository<Assignment>, AssignmentRepository>();
            Container.Register<IRepository<Tag>, TagRepository>();
            Container.Register<IRepository<WeeklyTimetable>, WeeklyTimetableRepository>();
            Container.Register<IRepository<DailyTimetable>, DailyTimetableRepository>();
            Container.Register<IRepository<UserUpdate>, FeedRepository>();
            Container.Register<IExtendedRepository<UserUpdate>, FeedRepository>();
            Container.Register<IRepositoryWrapper, RepositoryWrapper>();
            Container.Register<INavigationService, NavigationService>();
            Container.Register<IUserConfigManager, UserConfigManager>();
            Container.Register<IAssignmentsManager, AssignmentsManager>();
            Container.Register<IFeedService, FeedService>();
            Container.Register<DbConnection>(((c, o) =>
            {
                var dbConnection = new DbConnection();
                Task.Run(async () =>
                {
                    //init the db connection
                    //check the table mappings
                    //and open the connections lazily
                    await dbConnection.Init();
                });
                return dbConnection;
            }));
        }

        public static T Resolve<T>() where T : class
        {
            if (Container.CanResolve<T>()) return Container.Resolve<T>();
            return default;
        }

        public static object Resolve<T>(T objType) where T : Type
        {
            if (Container.CanResolve(objType)) return Container.Resolve(objType);
            return default;
        }

        public static void Register<T>() where T : class
        {
            Container.Register<T>();
        }
    }
}