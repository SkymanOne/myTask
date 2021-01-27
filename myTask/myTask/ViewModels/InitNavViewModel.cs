using System;
using System.Threading.Tasks;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class InitNavViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(InitNavPage);
        
        public InitNavViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}