using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myTask.Views.Reusable;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myTask.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitWorkingDaysPage : ContentPage
    {
        public InitWorkingDaysPage() 
        {
            InitializeComponent();
        }
    }
}