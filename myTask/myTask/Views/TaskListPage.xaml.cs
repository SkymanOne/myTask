using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myTask.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myTask.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage()
        {
            InitializeComponent();
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView listView)
                listView.SelectedItem = null;
        }


        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (BindingContext is TaskListViewModel viewModel)
                viewModel.DetailCommand.Execute(e.Item);
        }
    }
}