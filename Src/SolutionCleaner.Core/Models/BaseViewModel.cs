using System;
using System.ComponentModel;

namespace SolutionCleaner.Core.Models
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<TProperty>(BusinessProperty<TProperty> businessProperty)
        {
            OnPropertyChanged(businessProperty.Name);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void PropertySetter<TProperty>(BusinessProperty<TProperty> businessProperty, ref TProperty backingField, TProperty newValue, Action<TProperty> onPropertyChanged = null)
        {
            if (backingField == null && newValue == null)
            {
                return;
            }
            if (backingField == null || !backingField.Equals(newValue))
            {
                TProperty oldValue = backingField;
                backingField = newValue;
                onPropertyChanged?.Invoke(oldValue);
                OnPropertyChanged(businessProperty);
            }
        }
    }
}
