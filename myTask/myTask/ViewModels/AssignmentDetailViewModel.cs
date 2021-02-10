using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.DataStructures;
using myTask.Domain.Models;
using myTask.Services.AssignmentsManager;
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

        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IAssignmentsManager _assignmentsManager;
        private readonly IRepository<Tag> _tagRepository;

        public ICommand UpdateTitleCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand PickNewIcon { get; set; }
        public ICommand DeleteCommand { get; set; }
        
        public ICommand RequiredTimeCompletedCommand { get; set; }


        // UPDATE does not work
        // Since dictionary works with KeyValuePair structure which is immutable
        //TODO: implement observable dictionary
        public ObservableCollection<SubTask> SubTasks { get; set; }
        public ObservableCollection<TagSubViewModel> TagSubViewModels { get; set; }
        
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

        public AssignmentDetailViewModel(INavigationService navigationService, 
            IRepository<Assignment> assignmentRepository,
            IRepository<Tag> tagRepository,
            IAssignmentsManager assignmentsManager) : base(navigationService)
        {
            UpdateCommand = new Command(UpdateAsync);
            UpdateTitleCommand = new Command(UpdateLabelAsync);
            DeleteCommand = new Command(DeleteAsync);
            RequiredTimeCompletedCommand = new Command(RequiredTimeCompleted);
            _assignmentRepository = assignmentRepository;
            _tagRepository = tagRepository;
            _assignmentsManager = assignmentsManager;
        }

        private async void UpdateAsync()
        {
            _assignment.SubTasks = SubTasks.ToList();
            _assignment.Deadline = DeadlineModel.GetTime();
            _assignment.DurationMinutes = TimeRequired.GetTotalInMinutes();
            var tags = TagSubViewModels
                .Select(x => x.Tag)
                .Where(x => x.Title != "Add new")
                .ToList();
            _assignment.Tags = new List<Tag>();
            foreach (var tag in tags)
            {
                var item = await _tagRepository.GetItemAsync(x =>
                    x.Title.ToUpper() == tag.Title.ToUpper());
                if (item == null)
                {
                    tag.Assignments = new List<Assignment> {Assignment};
                    await _tagRepository.CreateItemAsync(tag);
                    _assignment.Tags.Add(tag);
                }
                else
                {
                    _assignment.Tags.Add(item);
                }
            }

            if (_assignment.Id == Guid.Empty)
            {
                await _assignmentsManager.CreateAssigmentAsync(_assignment);
            }
            else
            {
                await _assignmentsManager.UpdateAssignmentAsync(_assignment);   
            }
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
                await _assignmentRepository.DeleteItemAsync(Assignment);
                await _navigationService.NavigateToAsync<AssignmentListViewModel>();
                await _navigationService.ClearTheStackAsync();
            }
        }

        private void RequiredTimeCompleted()
        {
            var timeBeforeDeadline = (_deadline.GetTime() - DateTime.Now).Minutes;
            if (timeBeforeDeadline <= TimeRequired.GetTotalInMinutes())
            {
                var newDeadline = DateTime.Now.AddMinutes(TimeRequired.GetTotalInMinutes());
                DeadlineModel.Date = newDeadline.Date;
                DeadlineModel.Time = newDeadline.TimeOfDay;
                OnPropertyChanged(nameof(DeadlineModel));
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

                TagSubViewModels = new ObservableCollection<TagSubViewModel>(
                    assignment.Tags.Select(x => new TagSubViewModel(this, x)))
                {
                    new TagSubViewModel(this, new Tag() {Id = Guid.Empty, Title = "Add new"})
                };
                DeadlineModel = new Deadline()
                {
                    Date = Assignment.Deadline.Date,
                    Time = Assignment.Deadline.TimeOfDay,
                };
                TimeRequired = Duration.InitFromMinutes(Assignment.DurationMinutes);
                OnPropertyChanged(nameof(DeadlineModel));
                OnPropertyChanged(nameof(SubTasks));
                OnPropertyChanged(nameof(TimeRequired));
                OnPropertyChanged(nameof(TagSubViewModels));
            }
            else
            {
                await base.Init(param);
            }
        }

        public class TagSubViewModel : SubViewModel
        {
            public Tag Tag { get; set; }
            public Color BackgroundClr { get; set; }

            private Color[] _colors =
            {
                Color.FromHex("#D81B60"),
                Color.FromHex("#8E24AA"),
                Color.FromHex("#D32F2F"),
                Color.FromHex("#1976D2"),
                Color.FromHex("#795548"),
            };
            public ImageSource ImageSource { get; set; }
            public ICommand ContextDisplayCommand { get; set; }
            private readonly AssignmentDetailViewModel _assignmentDetailViewModel;

            public TagSubViewModel(AssignmentDetailViewModel detailViewModel, Tag tag)
            {
                _assignmentDetailViewModel = detailViewModel;
                Tag = tag;
                ContextDisplayCommand = new Command(ContextDisplay);
                ImageSource = tag.Title == "Add new" ? "baseline_add_white_18dp.png" : "baseline_clear_white_18dp.png";
                Random rnd = new Random();
                int colorIndex = rnd.Next(0, _colors.Length);
                BackgroundClr = _colors[colorIndex];
            }

            public async void ContextDisplay()
            {
                if (Tag.Title != "Add new")
                {
                    var result = await MaterialDialog.Instance.ConfirmAsync("Delete this tag?", "", "Yes", "No");
                    if (result == true)
                    {
                        _assignmentDetailViewModel.TagSubViewModels.Remove(this);
                        OnPropertyChanged(nameof(TagSubViewModels));
                    }
                }
                else
                {
                    var title = await MaterialDialog.Instance.InputAsync("New Tag", "Enter the title of a new tag");
                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
                    {
                        Tag newTag = new Tag()
                        {
                            Id = Guid.NewGuid(),
                            Title = title,
                        };
                        _assignmentDetailViewModel.TagSubViewModels.Insert(0, new TagSubViewModel(
                            _assignmentDetailViewModel, newTag));
                    }
                }
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