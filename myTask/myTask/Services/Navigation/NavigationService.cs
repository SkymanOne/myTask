using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public async Task InitMainNavigation()
        {
            //too lazy to do it manually, so instead deliver a list of viewModels to be resolved automatically
            var tabs = await ResolveNavigation(new List<Type>()
            {
                typeof(FeedViewModel),
                typeof(AssignmentListViewModel),
                typeof(ProgressViewModel),
                typeof(TimeTableViewModel)
            });

            var mainPageViewModel = SuperContainer.Resolve<MainNavigationViewModel>();
            mainPageViewModel.Tabs = tabs;
            
            var mainPage = ViewLocator.ResolvePageFromViewModel(mainPageViewModel);
            Application.Current.MainPage = mainPage;
            await (Application.Current.MainPage.BindingContext as MainNavigationViewModel)?.Init("Assignments")!;
        }


        private Task<ObservableCollection<Page>> ResolveNavigation(List<Type> viewModels)
        {
            ObservableCollection<Page> pages = new ObservableCollection<Page>();
            foreach (var viewModelType in viewModels)
            {
                var viewModel = (BaseViewModel) SuperContainer.Resolve(viewModelType);
                var page = ViewLocator.ResolvePageFromViewModel(viewModel);
                
                //wrap each page into a nav page in case we need to do some "in-tab" navigation
                //see https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/navigation/tabbed-page#navigate-within-a-tab
                var navPage = new NavigationPage(page)
                {
                    Title = page.Title,
                    IconImageSource = page.IconImageSource
                };
                pages.Add(navPage);
            }

            return Task.FromResult(pages);
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
            //resolve page's viewmodel
            var viewModel = SuperContainer.Resolve<TViewModel>();
            var page = ViewLocator.ResolvePageFromViewModel(viewModel);
            
            //check if the main navigation has been init
            if (Application.Current.MainPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                    #pragma warning disable 8602
                    await (page.BindingContext as BaseViewModel).Init(param);
                    #pragma warning restore 8602
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