using System;
using System.Windows.Input;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(TaskDetailPage);
        
        public ICommand BackCommand { get; set; }

        public TaskDetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            BackCommand = new Command(GoBackAsync);
        }

        private async void GoBackAsync()
        {
            await _navigationService.NavigateToAsync<TaskListViewModel>();
            await _navigationService.ClearTheStackAsync();
        }
    }
}