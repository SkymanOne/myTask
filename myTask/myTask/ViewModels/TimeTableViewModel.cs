using System;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;

namespace myTask.ViewModels
{
    public class TimeTableViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(TimetablePage);

        public TimeTableViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}