using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using myTask.Services.Navigation;

namespace myTask.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Link the ViewModel with corresponding Page
        /// </summary>
        // used in order to instance the type during runtime
        public abstract Type WiredPageType { get; }
        //every child implementations have to injected with Navigation Service
        protected INavigationService _navigationService;

        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        //short-cut for setting updating properties' values and notification
        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, backingField)) return;
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        //if ViewModel has a parameters to be passed
        //the method is called after the viewmodel was instantiated
        public virtual Task Init(object param)
        {
            return Task.FromResult(false);
        }
    }
}