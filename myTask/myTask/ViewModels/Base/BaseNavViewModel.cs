using System;
using myTask.Services.Navigation;

namespace myTask.ViewModels.Base
{
    public abstract class BaseNavViewModel : BaseViewModel
    {
        public virtual string IconSource { get; set; }
        public BaseNavViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}