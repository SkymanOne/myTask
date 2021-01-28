using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Domain.Models;
using myTask.Helpers;
using myTask.Services.Navigation;
using myTask.Services.UserConfigManager;
using myTask.ViewModels.Base;
using myTask.Views;
using Neemacademy.CustomControls.Xam.Plugin.TabView;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class SetWorkingHoursViewModel : BaseViewModel
    {

        private readonly IUserConfigManager _configManager;
        private int[] _selectedIndices;
        public override Type WiredPageType => typeof(InitWorkingHoursPage);

        public ObservableCollection<DayTab> Days { get; set; }
        public ICommand AddAssignmentsCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        
        public SetWorkingHoursViewModel(INavigationService navigationService, IUserConfigManager configManager) : base(navigationService)
        {
            _configManager = configManager;
            _navigationService = navigationService;
            AddAssignmentsCommand = new Command(GoToAddPage);
            GoBackCommand = new Command(GoBack);
            RefreshCommand = new Command(Refresh);
        }


        private async void GoToAddPage()
        {
            await FinishSetup();
        }


        private async void GoBack()
        {
            await _navigationService.NavigateToAsync<SetWorkingDaysViewModel>();
            await _navigationService.ClearTheStackAsync();
        }

        private async void Refresh()
        {
            Days = Days;
        }

        private async Task FinishSetup()
        {
            //TODO: finish registration
            /*
            double[] workingHours = new double[7];
            foreach (var index in _selectedIndices)
            {
                workingHours[index] = Days
                    .First(x => x.DayOfWeekName == ((DayOfWeek) index).ToFriendlyString())
                    .NumberOfWorkingHours;
            }
            await _configManager.SetConfig(new UserConfig()
            {
                WeeklyAvailableTimeInHours = workingHours
            });
            */
        }

        public override async Task Init(object param)
        {
            if (param is List<int> daysIndices)
            {
                daysIndices.Sort();
                var days = new List<DayTab>(daysIndices.Count);
                foreach (var id in daysIndices)
                {
                    days.Add(new DayTab()
                    {
                        TabViewControlTabItemTitle = ((DayOfWeek) id).ToFriendlyString(),
                    });
                }
                Days = new ObservableCollection<DayTab>(days);
                OnPropertyChanged(nameof(Days));
            }
        }

        public class DayTab : INotifyPropertyChanged, ITabViewControlTabItem
        {
            private string _tabViewControlTabItemTitle;
            private int _numberOfHours = 1;
            private ImageSource _tabViewControlTabItemIconSource;
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public void TabViewControlTabItemFocus()
            {
            }

            public int NumberOfHours
            {
                get => _numberOfHours;
                set
                {
                    _numberOfHours = value;
                    OnPropertyChanged(nameof(NumberOfHours));
                }
            }

            public string TabViewControlTabItemTitle
            {
                get => _tabViewControlTabItemTitle;
                set => _tabViewControlTabItemTitle = value;
            }

            public ImageSource TabViewControlTabItemIconSource { get; set; } = "icon.png";
        }
    }
}