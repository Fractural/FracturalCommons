using Godot;
using System;

#if TOOLS
namespace Fractural.Plugin
{
    public abstract class ValueProperty : Container
    {
        public event Action<object> ValueChanged;
        private object _value;
        public virtual object Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value; ;
                if (_value != oldValue)
                {
                    InvokeValueChanged(_value);
                    if (IsInsideTree())
                        UpdateProperty();
                }
            }
        }

        public void SetValue
        public virtual void UpdateProperty() { }
        protected void InvokeValueChanged(object value) => ValueChanged?.Invoke(value);
    }

    public abstract class ValueProperty<T> : ValueProperty
    {
        public new event Action<T> ValueChanged;
        public new T Value
        {
            get => (T)base.Value;
            set
            {
                if (!base.Value.Equals(value))
                    ValueChanged?.Invoke(value);
                base.Value = value;
            }
        }
        protected void InvokeValueChanged(T value) => base.InvokeValueChanged(value);
    }
}
#endif