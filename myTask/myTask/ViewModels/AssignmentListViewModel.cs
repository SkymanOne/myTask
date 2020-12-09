using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Models;
using myTask.Services.Database.Repositories;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;
using XF.Material.Forms;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Internals;

namespace myTask.ViewModels
{
    public class AssignmentListViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(AssignmentListPage);
        private readonly IRepository<Assignment> _assignmentRepository;
        
        public ICommand DetailCommand { get; set; }
        public ICommand CreateNewCommand { get; set; }

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
        public List<Assignment> Assignments
        {
            get => _assignments;
            set => SetValue(ref _assignments, value);
        }

        public AssignmentListViewModel(INavigationService navigationService, IRepository<Assignment> repository) : base(navigationService)
        {
            DetailCommand = new Command<Assignment>(GoToDetailPage);
            CreateNewCommand = new Command(CreateNewPage);
            _assignmentRepository = repository;
        }

        public async void GoToDetailPage(Assignment assignment)
        {
            await _navigationService.NavigateToAsync<AssignmentDetailViewModel>(assignment);
        }

        public async void CreateNewPage()
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

        public async Task UpdateListAsync()
        {
            var resultAwait = await _assignmentRepository.GetAllItemsAsync();
            if (resultAwait != null) Assignments = resultAwait.ToList();
        }

        public override async Task Init(object param)
        {
            await UpdateListAsync();
        }
    }
}