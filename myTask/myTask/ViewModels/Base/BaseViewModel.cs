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
        public abstract Type WiredPageType { get; }
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
        
        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, backingField)) return;
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        public virtual Task Init(object param)
        {
            return Task.FromResult(false);
        }
    }
}