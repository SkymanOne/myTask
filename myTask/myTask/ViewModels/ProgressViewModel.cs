using System;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;

namespace myTask.ViewModels
{
    public class ProgressViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(ProgressPage);

        public ProgressViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}