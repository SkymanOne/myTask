using System;
using System.Collections.ObjectModel;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;

namespace myTask.ViewModels
{
    public class FeedViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(FeedPage);

        public FeedViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}