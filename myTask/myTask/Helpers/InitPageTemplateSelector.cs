using System;
using myTask.ViewModels;
using Xamarin.Forms;

namespace myTask.Helpers
{
    public class InitPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WorkingDays { get; set; }
        public DataTemplate WorkingHours { get; set; }
        public DataTemplate AddTasks { get; set; }
        
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            switch (item)
            {
                case InitCarouselViewModel.WorkingDaysSubViewModel _:
                    return WorkingDays;
                case InitCarouselViewModel.WorkingHoursSubViewModel _:
                    return WorkingHours;
                default:
                    return AddTasks;
            }
        }
    }
}