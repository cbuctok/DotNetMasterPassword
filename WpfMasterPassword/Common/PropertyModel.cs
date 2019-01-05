using System;
using System.ComponentModel;

namespace WpfMasterPassword.Common
{
    public class PropertyModel<T> : BindableBase
    {
        private T _Value;

        public T Value
        {
            get => _Value;
            set
            {
                _Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public PropertyModel()
        {
        }

        public PropertyModel(T defaultValue)
        {
            this._Value = defaultValue;
        }
    }

    public class PropertyReadonlyModel<T> : BindableBase
    {
        private T _Value;
        public T Value => _Value;

        public PropertyReadonlyModel()
        {
        }

        public PropertyReadonlyModel(T defaultValue)
        {
            this._Value = defaultValue;
        }

        public void SetValue(T newValue)
        {
            _Value = newValue;
            OnPropertyChanged("Value");
        }
    }

    public class PropertyDelegateModel<T> : BindableBase
    {
        private readonly Func<T> GetValue;
        private readonly Action<T> SetValue;

        public T Value
        {
            get => GetValue();
            set
            {
                SetValue(value);
                OnPropertyChanged("Value");
            }
        }

        public PropertyDelegateModel(Func<T> getValue, Action<T> setValue)
        {
            GetValue = getValue ?? throw new ArgumentException("getValue");
            SetValue = setValue ?? throw new ArgumentException("setValue");
        }

        public void RaiseOnPropertyChanged()
        {
            OnPropertyChanged(() => Value);
        }
    }

    public class PropertyDelegateReadonlyModel<T> : BindableBase
    {
        private readonly Func<T> GetValue;
        public T Value => GetValue();

        public PropertyDelegateReadonlyModel(Func<T> getValue)
        {
            this.GetValue = getValue;
        }

        public void RaiseOnPropertyChanged()
        {
            OnPropertyChanged(() => Value);
        }

        public void MonitorForChanges(params INotifyPropertyChanged[] monitorThese)
        {
            // fire PropertyChanged whenever on of these guys changes
            foreach (var item in monitorThese)
            {
                item.PropertyChanged += delegate { RaiseOnPropertyChanged(); };
            }
        }
    }
}