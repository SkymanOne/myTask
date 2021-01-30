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
    public partial class AssignmentDetailPage : ContentPage
    {
        public AssignmentDetailPage()
        {
            InitializeComponent();
        }

        private void VisualElement_OnFocused(object sender, FocusEventArgs e)
        {
            (sender as Editor).HeightRequest = 300;
        }

        private void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
        {
            (sender as Editor).HeightRequest = 30;
        }
    }
}