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


        private void TimeEntry_OnFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                entry.ReturnCommand.Execute(e.IsFocused);
            }
        }

        private void VisualElement_OnFocused(object sender, FocusEventArgs e)
        {
            if (sender is Editor editor)
            {
                editor.Text = "";
                editor.HeightRequest = 300;
                editor.BackgroundColor = Color.Snow;
            }
        }

        private void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Editor editor)
            {
                editor.HeightRequest = 120;
                editor.BackgroundColor = Color.White;
                if (string.IsNullOrWhiteSpace(editor.Text) || string.IsNullOrEmpty(editor.Text))
                {
                    editor.Text = "No description provided";
                }
            }
        }
    }
}