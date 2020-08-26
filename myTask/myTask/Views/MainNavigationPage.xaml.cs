using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myTask.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavigationPage : TabbedPage
    {
        public static readonly BindableProperty ChildrenProperty = BindableProperty.Create("Children",
            typeof(IEnumerable<Page>), typeof(MainNavigationPage), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnItemsSourcePropertyChanged);
        
        public static readonly BindableProperty CurrentPageProperty = BindableProperty.Create("CurrentPage", typeof(Page),
            typeof(MainNavigationPage), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnCurrentPagePropertyChanged);


        public new Page CurrentPage
        {
            get => base.CurrentPage;
            set => SetValue(CurrentPageProperty, value);
        }
        
        public MainNavigationPage()
        {
            InitializeComponent();
        }
        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue,
            object newValue)
        {
            var tabbedPage = (TabbedPage) bindable;
            
            if (newValue is INotifyCollectionChanged notifyCollection) {
                notifyCollection.CollectionChanged += (sender, args) => {
                    if (args.NewItems != null) {
                        foreach (var newItem in args.NewItems) {
                            tabbedPage.Children.Add((Page)newItem);
                        }
                    }
                    if (args.OldItems != null) {
                        foreach (var oldItem in args.OldItems) {
                            tabbedPage.Children.Remove((Page)oldItem);
                        }
                    }
                };
            }

            if (newValue == null) return;

            tabbedPage.Children.Clear();

            foreach (var item in (IEnumerable<Page>) newValue) tabbedPage.Children.Add(item);
            
        }

        private static void OnCurrentPagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabbedPage = (TabbedPage) bindable;
            if (newValue is INotifyPropertyChanged propertyChanged)
            {
                propertyChanged.PropertyChanged += (sender, args) =>
                {
                    tabbedPage.CurrentPage = (Page) newValue;
                };
            }
        }
    }
}