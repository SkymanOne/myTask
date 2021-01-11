using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myTask.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitCarouselPage : ContentPage
    {
        public InitCarouselPage()
        {
            InitializeComponent();
        }

        private void ListView_OnScrolled(object sender, ScrolledEventArgs e)
        {
            //disable scrolling
            var listview = sender as ListView;
            var firstElement = listview.ItemsSource.OfType<object>().First();
            listview.ScrollTo(firstElement, ScrollToPosition.Start, false);
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}