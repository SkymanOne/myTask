using myTask.ViewModels;
using myTask.ViewModels.Base;
using Xamarin.Forms;

namespace myTask.Helpers
{
    public static class ViewLocator
    {
        public static Page ResolvePageFromViewModel<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
        {
            Page page = ObjectCreator.CreateObject<Page>(viewModel.WiredPageType);
            page.BindingContext = viewModel;
            return page;
        }
    }
}