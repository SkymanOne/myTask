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

namespace myTask.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        //TODO: revert code
        public override Type WiredPageType => typeof(TaskDetailPage);

        private readonly IRepository<MyTask> _repository;
        
        public ICommand BackCommand { get; set; }
        public ICommand PickNewIcon { get; set; }
        public ICommand UpdateLabel { get; set; }
        

        // does not work
        //TODO: implement observable dictionary
        public ObservableCollection<SubTask> SubTasks { get; set; }
        
        public MyTask _myTask;

        public MyTask MyTask
        {
            get => _myTask;
            set => SetValue(ref _myTask, value);
        }


        public TaskDetailViewModel(INavigationService navigationService, IRepository<MyTask> repository) : base(navigationService)
        {
            BackCommand = new Command(GoBackAsync);
            _repository = repository;
        }

        private async void GoBackAsync()
        {
            MyTask.SubTasks = SubTasks.ToList();
            await _repository.UpdateItemAsync(MyTask);
            await _navigationService.NavigateToAsync<TaskListViewModel>();
            await _navigationService.ClearTheStackAsync();
        }

        public override async Task Init(object param)
        {
            if (param is MyTask myTask)
            {
                MyTask = myTask;
                SubTasks = new ObservableCollection<SubTask>(MyTask.SubTasks);
                OnPropertyChanged(nameof(SubTasks));
            }
            else
            {
                await base.Init(param);
            }
        }
    }
}