using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.DataStructures;
using myTask.Models;
using myTask.Services.Database.Repositories;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;
using XF.Material.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace myTask.ViewModels
{
    public class AssignmentDetailViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(AssignmentDetailPage);

        private readonly IRepository<Assignment> _repository;

        public ICommand UpdateTitleCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand PickNewIcon { get; set; }
        public ICommand DeleteCommand { get; set; }


        // does not work
        //TODO: implement observable dictionary
        public ObservableCollection<SubTask> SubTasks { get; set; }
        
        private Assignment _assignment;
        public Assignment Assignment
        {
            get => _assignment;
            set => SetValue(ref _assignment, value);
        }

        private Deadline _deadline;
        public Deadline DeadlineModel
        {
            get => _deadline;
            set => SetValue(ref _deadline, value);
        }

        private Duration _timeRequired;

        public Duration TimeRequired
        {
            get => _timeRequired;
            set => SetValue(ref _timeRequired, value);
        }

        public AssignmentDetailViewModel(INavigationService navigationService, IRepository<Assignment> repository) : base(navigationService)
        {
            UpdateCommand = new Command(UpdateAsync);
            UpdateTitleCommand = new Command(UpdateLabelAsync);
            DeleteCommand = new Command(DeleteAsync);
            _repository = repository;
        }

        private async void UpdateAsync()
        {
            Assignment.SubTasks = SubTasks.ToList();
            Assignment.Deadline = DeadlineModel.GetTime();
            Assignment.DurationMinutes = TimeRequired.GetTotalInMinutes();
            await _repository.UpdateItemAsync(Assignment);
            await _navigationService.NavigateToAsync<AssignmentListViewModel>();
            await _navigationService.ClearTheStackAsync();
        }

        private async void DeleteAsync()
        {
            bool? confirm = await MaterialDialog.Instance.ConfirmAsync("Do you want to delete current task?",
                "Delete", "Yes", "No"
            );
            if (confirm == true)
            {
                await _repository.DeleteItemAsync(Assignment);
                await _navigationService.NavigateToAsync<AssignmentListViewModel>();
                await _navigationService.ClearTheStackAsync();
            }
        }

        private async void UpdateLabelAsync()
        {
            string newTitle = await MaterialDialog.Instance.InputAsync("Update Title");
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                Assignment.Title = newTitle;
                OnPropertyChanged(nameof(Assignment));
            }
            else await MaterialDialog.Instance.SnackbarAsync("Invalid title", 2000);
        }

        public override async Task Init(object param)
        {
            if (param is Assignment assignment)
            {
                Assignment = assignment;
                var subTasksFromBlobbed = Assignment.SubTasks ?? new List<SubTask>
                {
                    new SubTask("Hello"),
                    new SubTask("World")
                };
                SubTasks = new ObservableCollection<SubTask>(subTasksFromBlobbed);
                DeadlineModel = new Deadline()
                {
                    Date = Assignment.Deadline.Date,
                    Time = Assignment.Deadline.TimeOfDay,
                };
                TimeRequired = Duration.InitFromMinutes(Assignment.DurationMinutes);
                OnPropertyChanged(nameof(DeadlineModel));
                OnPropertyChanged(nameof(SubTasks));
                OnPropertyChanged(nameof(TimeRequired));
            }
            else
            {
                await base.Init(param);
            }
        }

        public class Deadline
        {
            public TimeSpan Time { get; set; }
            public DateTime Date { get; set; }
            public DateTime MinDate = DateTime.Now.AddMinutes(5);

            public DateTime GetTime()
            {
                return Date.Add(Time);
            }
        }

        public class Duration
        {
            private int _hours, _minutes;

            public string Hours
            {
                get => _hours.ToString();
                set
                {
                    try
                    {
                        _hours = int.Parse(value);
                    }
                    catch (FormatException)
                    {
                        _hours = 0;
                    }
                }
            }
            
            public string Minutes
            {
                get => _minutes.ToString();
                set
                {
                    try
                    {
                        _minutes = int.Parse(value);
                    }
                    catch (FormatException)
                    {
                        _minutes = 0;
                    }
                }
            }

            public int GetTotalInMinutes()
            {
                return _hours * 60 + _minutes;
            }

            public static Duration InitFromMinutes(int minutes)
            {
                return new Duration()
                {
                    _hours = minutes / 60,
                    _minutes = minutes % 60
                };
            }
        }
    }
}