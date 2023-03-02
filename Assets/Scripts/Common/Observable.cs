using System;

namespace Assets.Scripts.Common
{
    public class Observable<T>
    {
        private T innerValue;

        public delegate void OnChangedHandler(T newValue, T oldValue);

        public event OnChangedHandler OnChanged;

        public T Value
        {
            get => innerValue;
            set
            {
                var oldValue = innerValue;
                innerValue = value;
                OnChanged?.Invoke(innerValue, oldValue);
            }
        }

        public Observable(T initialValue)
        {
            innerValue = initialValue;
        }
    }
}
