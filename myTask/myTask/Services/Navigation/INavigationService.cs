using System.Threading.Tasks;
using myTask.ViewModels;
using myTask.ViewModels.Base;

namespace myTask.Services.Navigation
{
    public interface INavigationService
    {
        Task InitMainNavigation();
        Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;
        Task NavigateToAsync<TViewModel>(object parameters)where TViewModel : BaseViewModel;

        Task ClearTheStackAsync();

        Task NavigateToModalAsync<TViewModel>() where TViewModel : BaseViewModel;
        Task PopModalAsync();
    }
}