using Fractural.Utils;
using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class WrappedValueProperty : ValueProperty
    {
        private ValueProperty _valueProperty;
        public ValueProperty ValueProperty
        {
            get => _valueProperty;
            set
            {
                if (IsInsideTree() && _valueProperty != value)
                {
                    RemoveChild(_valueProperty);
                    value.Reparent(this);
                }
                _valueProperty = value;
            }
        }
        public override object Value
        {
            get => ValueProperty.Value;
            set => ValueProperty.Value = value;
        }

        public WrappedValueProperty() { }
        public WrappedValueProperty(ValueProperty valueProperty)
        {
            _valueProperty = valueProperty;
            _valueProperty.ValueChanged += (newValue) => InvokeValueChanged(newValue);
        }

        public override void _Ready()
        {
            if (ValueProperty != null)
                ValueProperty.Reparent(this);
        }
    }
}
#endif