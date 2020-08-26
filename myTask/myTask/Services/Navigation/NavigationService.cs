using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using myTask.Helpers;
using myTask.ViewModels;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.Services.Navigation
{
    public class NavigationService : INavigationService
    {

        public NavigationService()
        {
            
        }

        public Task InitMainNavigation()
        {
            var tabs = ResolveNavigation(new List<Type>()
            {
                typeof(FeedViewModel),
                typeof(TaskListViewModel),
                typeof(ProgressViewModel),
                typeof(TimeTableViewModel)
            });

            var mainPageViewModel = SuperContainer.Resolve<MainNavigationViewModel>();

            mainPageViewModel.Tabs = tabs;

            var mainPage = ViewLocator.ResolvePageFromViewModel(mainPageViewModel);
            Application.Current.MainPage = mainPage;
            return Task.CompletedTask;
        }


        private ObservableCollection<Page> ResolveNavigation(List<Type> viewModels)
        {
            ObservableCollection<Page> pages = new ObservableCollection<Page>();
            foreach (var viewModelType in viewModels)
            {
                var viewModel = (BaseViewModel) SuperContainer.Resolve(viewModelType);
                var page = ViewLocator.ResolvePageFromViewModel(viewModel);
                
                var navPage = new NavigationPage(page) {Title = page.Title};
                pages.Add(navPage);
            }

            return pages;
        }

        public async Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
        {
            await NavigateToHelper<TViewModel>(null);
        }

        public async Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel
        {
            await NavigateToHelper<TViewModel>(parameter);
        }

        private async Task NavigateToHelper<TViewModel>(object param) where TViewModel : BaseViewModel
        {
            var viewModel = SuperContainer.Resolve<TViewModel>();
            await viewModel.PassParameter(param);
            var page = ViewLocator.ResolvePageFromViewModel(viewModel);

            if (Application.Current.MainPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                   await navigationPage.PushAsync(page);
                }
            }
            else
            {
                await InitMainNavigation();
            }
        }

        public Task ClearTheStackAsync()
        {
            for (int i = 0; i < Application.Current.MainPage.Navigation.NavigationStack.Count - 1; i++)
            {
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[i]);
            }
            return Task.CompletedTask;
        }

        public async Task NavigateToModalAsync<TViewModel>() where TViewModel : BaseViewModel
        {
            var viewModel = SuperContainer.Resolve<TViewModel>();
            var page = ViewLocator.ResolvePageFromViewModel(viewModel);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

        public async Task PopModalAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}