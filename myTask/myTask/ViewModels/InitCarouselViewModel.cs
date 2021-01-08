using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using myTask.Models;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;

namespace myTask.ViewModels
{
    public class InitCarouselViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(InitCarouselPage);
        public ObservableCollection<SubViewModel> SubPages { get; set; }

        private ObservableCollection<WorkingDay> _workingDays = new ObservableCollection<WorkingDay>()
        {
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Sunday.ToString(),
                DayOfWeek = DayOfWeek.Sunday,
            },

            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Monday.ToString(),
                DayOfWeek = DayOfWeek.Monday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Tuesday.ToString(),
                DayOfWeek = DayOfWeek.Tuesday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Wednesday.ToString(),
                DayOfWeek = DayOfWeek.Wednesday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Thursday.ToString(),
                DayOfWeek = DayOfWeek.Thursday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Friday.ToString(),
                DayOfWeek = DayOfWeek.Friday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Saturday.ToString(),
                DayOfWeek = DayOfWeek.Saturday,
            },

        };

        public InitCarouselViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
        

        public override Task Init(object param)
        {
            SubPages = new ObservableCollection<SubViewModel>()
            {
                new WorkingDaysSubViewModel(_workingDays),
                new WorkingHoursSubViewModel(_workingDays),
                new AddTasksSubViewModel(_workingDays)
            };
            return Task.CompletedTask;
        }


        public class SubViewModel
        {
            public ObservableCollection<WorkingDay> WorkingDays { get; set; }

            public SubViewModel(ObservableCollection<WorkingDay> workingDays)
            {
                WorkingDays = workingDays;
            }
        }
        
        public class WorkingDaysSubViewModel : SubViewModel
        {
            public WorkingDaysSubViewModel(ObservableCollection<WorkingDay> workingDays) : base(workingDays)
            {
            }
        }
        
        public class WorkingHoursSubViewModel : SubViewModel
        {
            public WorkingHoursSubViewModel(ObservableCollection<WorkingDay> workingDays) : base(workingDays)
            {
            }
        }

        public class AddTasksSubViewModel : SubViewModel
        {
            public AddTasksSubViewModel(ObservableCollection<WorkingDay> workingDays) : base(workingDays)
            {
            }
        }
    }
}