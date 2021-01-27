using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Helpers;
using myTask.Models;
using myTask.Services.Navigation;
using myTask.Services.UserConfigManager;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class SetWorkingHoursViewModel : BaseViewModel
    {

        private readonly IUserConfigManager _configManager;
        private int[] _selectedIndices;
        public override Type WiredPageType => typeof(InitWorkingHoursPage);

        public ObservableCollection<WorkingDay> Days { get; set; }
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
            //Days = new ObservableCollection<WorkingDay>();
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
        }

        public override async Task Init(object param)
        {
            if (param is List<int> daysIndices)
            {
                daysIndices.Sort();
                var days = new List<WorkingDay>(daysIndices.Count);
                foreach (var id in daysIndices)
                {
                    days.Add(new WorkingDay()
                    {
                        DayOfWeekName = ((DayOfWeek) id).ToFriendlyString(),
                        NumberOfWorkingHours = 1,
                    });
                }
                Days = new ObservableCollection<WorkingDay>(days);
            }
            await base.Init(param);
        }
    }
}