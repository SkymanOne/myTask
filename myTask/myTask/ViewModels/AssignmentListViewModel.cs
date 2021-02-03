using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Domain.Models;
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
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IUserConfigManager _configManager;
        
        public ICommand DetailCommand { get; set; }
        public ICommand CreateNewCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public string CurrentDate
        {
            get
            {
                //choose ending of the date depending on the day number
                string ending;
                switch (DateTime.Now.Day)
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
                return DateTime.Now.ToString($"dddd, d'{ending}' MMMM");
            }
        }

        private List<Assignment> _assignments;
        
        //TODO: use stacks
        public List<Assignment> Assignments
        {
            get => _assignments;
            set => SetValue(ref _assignments, value);
        }

        public AssignmentListViewModel(INavigationService navigationService, IRepository<Assignment> repository, IUserConfigManager userConfigManager) : base(navigationService)
        {
            DetailCommand = new Command<Assignment>(GoToDetailPage);
            CreateNewCommand = new Command(CreateNewPage);
            ResetCommand = new Command(ResetAsync);
            _assignmentRepository = repository;
            _configManager = userConfigManager;
        }

        private async void GoToDetailPage(Assignment assignment)
        {
            await _navigationService.NavigateToAsync<AssignmentDetailViewModel>(assignment);
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
                var result = await _assignmentRepository.CreateItemAsync(assignment);
                if (result) await UpdateListAsync();
            }
        }

        private async Task UpdateListAsync()
        {
            //TODO: replace with AssignmentManager
            var resultAwait = await _assignmentRepository.GetAllItemsAsync();
            if (resultAwait != null) Assignments = resultAwait.ToList();
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

        public override async Task Init(object param)
        {
            await UpdateListAsync();
        }
    }
}