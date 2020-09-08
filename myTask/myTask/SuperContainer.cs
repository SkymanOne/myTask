using System;
using myTask.Models;
using myTask.Services.Database;
using myTask.Services.Database.MockRepositories;
using myTask.Services.Database.Repositories;
using myTask.Services.Navigation;
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
            Container.Register<TaskListViewModel>();
            Container.Register<TaskDetailViewModel>();
            Container.Register<MainNavigationViewModel>();
            Container.Register<ProgressViewModel>();
            Container.Register<TimeTableViewModel>();
            Container.Register<FeedViewModel>();
            
            //register services
            Container.Register<INavigationService, NavigationService>();
        }

        public static void UpdateDependencies(bool useMocks)
        {
            Container.Register<INavigationService, NavigationService>();
            Container.Register<DbConnection>();
            if (useMocks)
            {
                Container.Register(typeof(IRepository<>), typeof(MockRepository<>));
            }
            else
            {
                Container.Register<IRepository<MyTask>, MyTaskRepository>();
                Container.Register<IRepository<Tag>, TagRepository>();
                Container.Register<IRepository<WeeklyTimetable>, WeeklyTimetableRepository>();
                Container.Register<IRepository<DailyTimetable>, DailyTimetableRepository>();
            }
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