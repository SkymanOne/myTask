using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Domain.Models;
using myTask.Services.Database.Repositories;
using myTask.Services.FeedService;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace myTask.ViewModels
{
    public class TimeTableViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(TimetablePage);

        private readonly IRepository<Assignment> _assignmentRepository;
        private Calendar _calendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
        public ICommand TodayCommand { get; set; }

        private EventCollection _eventCollection = new EventCollection();
        public EventCollection Deadlines
        {
            get => _eventCollection;
            set => SetValue(ref _eventCollection, value);
        }

        private int _month = DateTime.Today.Month;
        public int Month
        {
            get => _month;
            set
            {
                SetValue(ref _month, value);
                ExecuteUpdate();
            }
        }

        public int _year = DateTime.Today.Year;
        public int Year
        {
            get => _year;
            set
            {
                SetValue(ref _year, value);
                ExecuteUpdate();
            }
        }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetValue(ref _selectedDate, value);
        }

        private DateTime _minimumDate = new DateTime(2021, 1, 1);
        public DateTime MinimumDate
        {
            get => _minimumDate;
            set => SetValue(ref _minimumDate, value);
        }

        private DateTime _maximumDate = DateTime.Today.AddMonths(5);
        public DateTime MaximumDate
        {
            get => _maximumDate;
            set => SetValue(ref _maximumDate, value);
        }

        public TimeTableViewModel(INavigationService navigationService,
            IRepository<Assignment> assignmentRepository) : base(navigationService)
        {
            _assignmentRepository = assignmentRepository;
            TodayCommand = new Command(MoveToToday);
            MessagingCenter.Subscribe<FeedService>(this, "New Update", 
                async model =>
                {
                    await Init(null);
                });
        }

        private void MoveToToday()
        {
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
            SelectedDate = DateTime.Now;
        }

        private async void ExecuteUpdate()
        {
            await AddDeadlines();
        }

        private async Task<IEnumerable<Assignment>> GetDeadlinesDuringTheMonth()
        {
            var lowerBound = new DateTime(Year, Month, 1);
            var upperBound = new DateTime(Year, Month, _calendar.GetDaysInMonth(Year, _month));
            var r = await _assignmentRepository.GetAllItemsAsync();
            return r.Where(x => x.Deadline >= lowerBound
                                && x.Deadline.Ticks <= upperBound.Ticks);
        }

        private async Task AddDeadlines()
        {
            var assignments = await GetDeadlinesDuringTheMonth();
            for (int i = 1; i <= _calendar.GetDaysInMonth(Year, Month); i++)
            {
                DateTime date = new DateTime(Year, Month, i);
                var list = assignments
                    .Where(x => x.Deadline.DayOfYear == date.DayOfYear)
                    .Select(x => new DeadlineModel(x, this)).ToList();
                if (list.Any())
                {
                    Deadlines[date] = list;
                }
            }
        }

        public override async Task Init(object param)
        {
            await AddDeadlines();
        }

        public class DeadlineModel : SubViewModel
        {
            private readonly TimeTableViewModel _parentViewModel;
            private Assignment _assignment;
            public Assignment Assignment
            {
                get => _assignment;
                set => SetValue(ref _assignment, value);
            }
            
            public ICommand TappedCommand { get; set; }

            public DeadlineModel(Assignment assignment, TimeTableViewModel parentViewModel)
            {
                _assignment = assignment;
                _parentViewModel = parentViewModel;
                TappedCommand = new Command(GoToDetail);
            }

            private async void GoToDetail()
            {
                await _parentViewModel._navigationService.NavigateToAsync<AssignmentDetailViewModel>(Assignment);
            }
            
        }
        
    }
}