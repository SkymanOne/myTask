using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myTask.ViewModels.Base
{
    public class SubViewModel
    {
        public SubViewModel()
        {
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
    }
}