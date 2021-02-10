using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Domain.Models;
using myTask.Services.AssignmentsManager;
using myTask.Services.Database.Repositories;
using myTask.Services.Navigation;
using myTask.Services.UserConfigManager;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;
using XF.Material.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace myTask.ViewModels
{
    public class AssignmentListViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(AssignmentListPage);
        private readonly IAssignmentsManager _manager;
        private readonly IUserConfigManager _configManager;
        private WeeklyTimetable _week;

        public List<DaySubViewModel> Days { get; set; } 

        private DaySubViewModel _currentDay;
        public DaySubViewModel CurrentDay
        {
            get => _currentDay;
            set => SetValue(ref _currentDay, value);
        }

        private int _position;

        public int Position
        {
            get => _position;
            set => _position = value;
        }

        private string _currentDate;
        public string CurrentDate
        {
            get => _currentDate;
            set => SetValue(ref _currentDate, value);
        }


        public ICommand CreateNewCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        
        public ICommand ItemChangedCommand { get; set; }

        public AssignmentListViewModel(INavigationService navigationService, IAssignmentsManager manager,
            IUserConfigManager userConfigManager) : base(navigationService)
        {
            CreateNewCommand = new Command(CreateNewPage);
            ResetCommand = new Command(ResetAsync);
            ItemChangedCommand = new Command(ItemChanged);
            _configManager = userConfigManager;
            _manager = manager;
        }
        
        private async void ResetAsync()
        {
            string message = "This option allows you to reset the settings such as your routine timetable.\n" +
                             "This action can not be undone?" +
                             "Would you like to proceed?";
            var confirmed = await MaterialDialog.Instance.ConfirmAsync(message, "Reset", "Yes", "No") ?? false;
            if (confirmed)
            {
                await _configManager.SetConfigAsync(new UserConfig()
                {
                    IsInit = false
                });
                await _navigationService.InitMainNavigation();
                await MaterialDialog.Instance.SnackbarAsync("Reset has been successful!");
            }
            else
            {
                await MaterialDialog.Instance.SnackbarAsync("Reset has been dismissed!");
            }

        }
        
        private async void GoToDetailPage(Assignment assignment)
        {
            await _navigationService.NavigateToAsync<AssignmentDetailViewModel>(assignment);
        }

        private async void ItemChanged()
        { 
            var currentDay = Days[Position];
            CurrentDate = currentDay.CurrentDate;
            OnPropertyChanged(nameof(CurrentDate));
            if (!currentDay.Assignments.Any())
            {
                await MaterialDialog.Instance.SnackbarAsync("You don't have any assignments on this day", "Ok");
            }
        }
        
        private async void CreateNewPage()
        {
            var title = await MaterialDialog.Instance.InputAsync("Enter task title");
            title = title.TrimEnd();
            if (!string.IsNullOrWhiteSpace(title))
            {
                var assignment = new Assignment()
                {
                    Title = title,
                    Deadline = DateTime.Now + TimeSpan.FromMinutes(20)
                };
                GoToDetailPage(assignment);
            }
        }

        public override async Task Init(object param)
        {
            Calendar calendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
            int currentWeek = calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
            var week = await _manager.LoadWeeklyTimetableAsync(currentWeek);
            _week = week;
            Days = _week.Timetables.Select(x => new DaySubViewModel(this, x)).ToList();
            CurrentDay = Days.First(x => x.DailyTimetable.Day == DateTime.Now.DayOfWeek);
            Position = Days.IndexOf(CurrentDay);
            OnPropertyChanged(nameof(Days));
            OnPropertyChanged(nameof(CurrentDay));
            OnPropertyChanged(nameof(Position));
        }

        public class DaySubViewModel : SubViewModel
        {
            public ICommand DetailCommand { get; set; }

            private readonly AssignmentListViewModel _viewModel;
            public DailyTimetable DailyTimetable { get; set; }

            public DaySubViewModel(AssignmentListViewModel listViewModel, DailyTimetable dailyTimetable)
            {
                Calendar calendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
                DetailCommand = new Command<Assignment>(GoToDetailPage);
                _viewModel = listViewModel;
                Assignments = dailyTimetable.Assignments;
                DailyTimetable = dailyTimetable;
                _dateTime = calendar.AddDays(new DateTime(DateTime.Now.Year, 1, 1), DailyTimetable.DayNumber-1);
            }

            private DateTime _dateTime;
            
            public string CurrentDate
            {
                get
                {
                    //choose ending of the date depending on the day number
                    string ending;
                    switch (_dateTime.Day)
                    {
                        case 1:
                            ending = "st";
                            break;
                        case 2:
                            ending = "nd";
                            break;
                        case 3:
                            ending = "rd";
                            break;
                        default:
                            ending = "th";
                            break;
                    }

                    return _dateTime.ToString($"dddd, d'{ending}' MMMM");
                }
            }

            private List<Assignment> _assignments;

            //TODO: use stacks
            public List<Assignment> Assignments
            {
                get => _assignments;
                set => SetValue(ref _assignments, value);
            }

            private async void GoToDetailPage(Assignment assignment)
            {
                await _viewModel._navigationService.NavigateToAsync<AssignmentDetailViewModel>(assignment);
            }
            

            private async Task UpdateListAsync()
            {
                var allItemsAsync = await _viewModel._manager.LoadAssignmentsAsync(
                    DateTime.Now.DayOfWeek);
                if (allItemsAsync != null) Assignments = allItemsAsync.Assignments.ToList();
            }
        }
    }
}