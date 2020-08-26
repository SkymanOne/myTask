using System;
using System.Collections.ObjectModel;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class MainNavigationViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(MainNavigationPage);

        public MainNavigationViewModel(INavigationService navigationService) : base(navigationService)
        {
            _tabs = new ObservableCollection<Page>();
        }

        private ObservableCollection<Page> _tabs;
        public ObservableCollection<Page> Tabs
        {
            get => _tabs;
            set => SetValue(ref _tabs, value);
        }

        private Page _currentPage;

        public Page CurrentPage
        {
            get => _currentPage;
            set => SetValue(ref _currentPage, value);
        }
    }
}