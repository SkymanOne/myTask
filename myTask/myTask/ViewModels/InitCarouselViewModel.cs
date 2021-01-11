using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Helpers;
using myTask.Models;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

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
                DayOfWeekName = DayOfWeek.Sunday.ToFriendlyString(),
                DayOfWeek = DayOfWeek.Sunday,
            },

            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Monday.ToFriendlyString(),
                DayOfWeek = DayOfWeek.Monday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Tuesday.ToFriendlyString(),
                DayOfWeek = DayOfWeek.Tuesday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Wednesday.ToFriendlyString(),
                DayOfWeek = DayOfWeek.Wednesday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Thursday.ToFriendlyString(),
                DayOfWeek = DayOfWeek.Thursday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Friday.ToFriendlyString(),
                DayOfWeek = DayOfWeek.Friday,
            },
            new WorkingDay()
            {
                DayOfWeekName = DayOfWeek.Saturday.ToFriendlyString(),
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
            public ObservableCollection<string> WorkingDaysStrings { get; set; } = new ObservableCollection<string>()
            {
                "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
            };
            public ObservableCollection<int> SelectedDays { get; set; }
            public ICommand ChangedSelectedDaysCommand { get; set; }
            public WorkingDaysSubViewModel(ObservableCollection<WorkingDay> workingDays) : base(workingDays)
            {
                ChangedSelectedDaysCommand = new Command<List<int>>(OnChangedSelectedDays);
            }

            private void OnChangedSelectedDays(List<int> indicies)
            {
                foreach (var id in indicies)
                {
                    WorkingDays.First(x => x.DayOfWeekName == WorkingDaysStrings[id]).IsChecked = true;
                }
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