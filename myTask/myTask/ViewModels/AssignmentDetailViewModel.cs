using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.DataStructures;
using myTask.Domain.Models;
using myTask.Helpers;
using myTask.Services.AssignmentsManager;
using myTask.Services.Database.Repositories;
using myTask.Services.FeedService;
using myTask.Services.Navigation;
using myTask.Services.UserConfigManager;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Essentials;
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
        private readonly IFeedService _feedService;
        private readonly IUserConfigManager _config;

        public ICommand UpdateTitleCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand PickNewIconCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand RequiredTimeCompletedCommand { get; set; }
        public ICommand StartPauseCommand { get; set; }
        public ICommand FinishCommand { get; set; }


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

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetValue(ref _imageSource, value);
        }

        private string _status;

        public string StatusButton
        {
            get => _status;
            set => SetValue(ref _status, value);
        }

        public string UpdateButtonLabel
        {
            get
            {
                if (Assignment == null || Assignment.Id == default) return "Create";
                return "Update";
            }
        }

        private void SetButtonLabel()
        {
            if (_assignment == null)
            {
                StatusButton = "Start";
                OnPropertyChanged(nameof(StatusButton));
                return;
            }
            switch (_assignment.Status)
            {
                case Status.Paused:
                    StatusButton = "Resume";
                    break;
                case Status.Created:
                case Status.Finished:
                    StatusButton = "Start";
                    break;
                default:
                    StatusButton = "Pause";
                    break;
            }
            OnPropertyChanged(nameof(StatusButton));
        }

        public string Time => _assignment == null ? "00:00:00" : $"{(_assignment.TimeElapsedSeconds / 3600):00}:{((_assignment.TimeElapsedSeconds % 3600) / 60):00}:{(_assignment.TimeElapsedSeconds % 3600 % 60):00}";

        public AssignmentDetailViewModel(INavigationService navigationService, 
            IRepository<Assignment> assignmentRepository,
            IRepository<Tag> tagRepository,
            IAssignmentsManager assignmentsManager,
            IFeedService feedService,
            IUserConfigManager configManager) : base(navigationService)
        {
            UpdateCommand = new Command(UpdateAsync);
            UpdateTitleCommand = new Command(UpdateLabelAsync);
            DeleteCommand = new Command(DeleteAsync);
            RequiredTimeCompletedCommand = new Command(RequiredTimeCompleted);
            StartPauseCommand = new Command(StartPauseAssignment);
            FinishCommand = new Command(FinishAssignment);
            PickNewIconCommand = new Command(PickPicture);
            _assignmentRepository = assignmentRepository;
            _tagRepository = tagRepository;
            _assignmentsManager = assignmentsManager;
            _feedService = feedService;
            _config = configManager;
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
                await _feedService.RegisterUpdate("New assignment", "You created new assignment! Congratulations",
                    _assignment);
            }
            else
            {
                await _assignmentsManager.UpdateAssignmentAsync(_assignment);   
                await _feedService.RegisterUpdate("Assignment update", "You updated an assignment! Get your hands on it",
                    _assignment);
            }

            await _navigationService.PopAsync();
        }
        

        private async void DeleteAsync()
        {
            bool? confirm = await MaterialDialog.Instance.ConfirmAsync("Do you want to delete current task?",
                "Delete", "Yes", "No"
            );
            if (confirm == true)
            {
                await _feedService.RegisterUpdate("Remove",
                    "You deleted an assignment, don't worry, there are others to do");
                await _assignmentsManager.DeleteAssignmentAsync(Assignment);
                await _navigationService.PopAsync();
            }
        }

        private void RequiredTimeCompleted()
        {
            var timeBeforeDeadline = (_deadline.GetTime() - DateTime.Now).TotalMinutes;
            if (timeBeforeDeadline <= TimeRequired.GetTotalInMinutes())
            {
                var newDeadline = DateTime.Now.AddMinutes(TimeRequired.GetTotalInMinutes() + 15);
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

        private async void StartPauseAssignment()
        {
            if (Assignment.Status == Status.Finished)
            {
                await MaterialDialog.Instance.SnackbarAsync("Assignment is completed");
            }
            if (Assignment.Status != Status.Going)
            {
                Assignment.Status = Status.Going;
                SetButtonLabel();
                Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                {
                    Assignment.TimeElapsedSeconds++;
                    OnPropertyChanged(nameof(Time));
                    return Assignment.Status == Status.Going;
                });
                await _feedService.RegisterUpdate("New start",
                    $"You started an assignment {_assignment.Title}. Well done!", _assignment);
            }
            else
            {
                Assignment.Status = Status.Paused;
                SetButtonLabel();
                await _feedService.RegisterUpdate("New start",
                    $"You paused an assignment {_assignment.Title}. Well done!", _assignment);
            }
            await _assignmentRepository.UpdateItemAsync(_assignment);
        }

        private async void FinishAssignment()
        {
            if (Assignment.Status == Status.Going || Assignment.Status == Status.Paused)
            {
                bool confirmed = await MaterialDialog.Instance.ConfirmAsync("Would you like to finish the assignment",
                    "Confirmation", "Yes", "No") ?? false;
                if (confirmed)
                {
                    Assignment.Status = Status.Finished;
                    SetButtonLabel();
                    var userConfig = await _config.GetConfigAsync();
                    userConfig.Kinbens += Assignment.Kinbens;
                    await _config.SetConfigAsync(userConfig);
                    await _assignmentRepository.UpdateItemAsync(_assignment);
                    await _feedService.RegisterUpdate("Completion",
                        $"You completed an assignment {_assignment.Title}. Congratulations! You earned {_assignment.Kinbens} kinbens", _assignment);
                }
            }
            else
            {
                await MaterialDialog.Instance.SnackbarAsync(
                    "The assignments has either already been finished or not started");
            }
        }

        private async void PickPicture()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    await using (var stream = await photo.OpenReadAsync())
                    {
                        MemoryStream memStream = new MemoryStream();
                        await stream.CopyToAsync(memStream);
                        Assignment.Icon = memStream.GetBuffer();
                    }
                    ImageSource = Assignment.GetImageSource();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override async Task Init(object param)
        {
            using (await MaterialDialog.Instance.LoadingDialogAsync("Loading"))
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
                    ImageSource = Assignment.GetImageSource();
                    TimeRequired = Duration.InitFromMinutes(Assignment.DurationMinutes);
                    SetButtonLabel();
                    OnPropertyChanged(nameof(Time));
                    OnPropertyChanged(nameof(ImageSource));
                    OnPropertyChanged(nameof(DeadlineModel));
                    OnPropertyChanged(nameof(SubTasks));
                    OnPropertyChanged(nameof(TimeRequired));
                    OnPropertyChanged(nameof(TagSubViewModels));
                    OnPropertyChanged(nameof(UpdateButtonLabel));
                }
                else
                {
                    await base.Init(param);
                }
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