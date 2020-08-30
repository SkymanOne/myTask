using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class TaskListViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(TaskListPage);
        
        public ICommand DetailCommand { get; set; }
        public ObservableCollection<Task> MyTasks { get; set; }

        public TaskListViewModel(INavigationService navigationService) : base(navigationService)
        {
            DetailCommand = new Command(GoToDetailPage);
        }

        public async void GoToDetailPage()
        {
            await _navigationService.NavigateToAsync<TaskDetailViewModel>();
        }
    }
}