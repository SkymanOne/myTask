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
    public class TaskListViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(TaskListPage);
        private readonly IRepository<MyTask> _myTaskRepository;
        
        public ICommand DetailCommand { get; set; }
        public ICommand CreateNewCommand { get; set; }

        public List<MyTask> MyTasks => _myTaskRepository.GetAllItemsAsync().Result.ToList();

        public TaskListViewModel(INavigationService navigationService, IRepository<MyTask> repository) : base(navigationService)
        {
            DetailCommand = new Command(GoToDetailPage);
            CreateNewCommand = new Command(CreateNewPage);
            _myTaskRepository = repository;
        }

        public async void GoToDetailPage()
        {
            await _navigationService.NavigateToAsync<TaskDetailViewModel>();
        }

        public async void CreateNewPage()
        {
            var title = await MaterialDialog.Instance.InputAsync("Enter task title");
            title = title.TrimEnd();
            if (!string.IsNullOrWhiteSpace(title))
            {
                var myTask = new MyTask()
                {
                    Title = title
                };
                var result = await _myTaskRepository.CreateItemAsync(myTask);
                if (result) OnPropertyChanged("MyTasks");
            }
        }

        public override async Task Init(object param)
        {
            var list = await _myTaskRepository.GetAllItemsAsync();
            //MyTasks = new ObservableCollection<MyTask>(list);
        }
    }
}