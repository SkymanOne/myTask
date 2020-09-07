using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Models;
using myTask.Services.Database.Repositories;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class TaskListViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(TaskListPage);
        private readonly IRepository<MyTask> _myTaskRepository;
        
        public ICommand DetailCommand { get; set; }
        public ObservableCollection<MyTask> MyTasks { get; set; }

        public TaskListViewModel(INavigationService navigationService, IRepository<MyTask> repository) : base(navigationService)
        {
            DetailCommand = new Command(GoToDetailPage);
            _myTaskRepository = repository;
        }

        public async void GoToDetailPage()
        {
            await _navigationService.NavigateToAsync<TaskDetailViewModel>();
        }

        public override async Task Init(object param)
        {
            MyTasks = await _myTaskRepository.GetAllItemsAsync() as ObservableCollection<MyTask>;
        }
    }
}