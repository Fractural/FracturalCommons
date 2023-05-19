using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class IntegerValueProperty : ValueProperty<int>
    {
        private SpinBox _spinBox;

        public IntegerValueProperty() : base()
        {
            _spinBox = new SpinBox();
            _spinBox.Rounded = true;
            _spinBox.Step = 1;
            _spinBox.AllowGreater = true;
            _spinBox.AllowLesser = true;
            _spinBox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _spinBox.Connect("value_changed", this, nameof(OnSpinBoxChanged));
            AddChild(_spinBox);
        }

        public override void UpdateProperty()
        {
            _spinBox.SetBlockSignals(true);
            _spinBox.Value = Value;
            _spinBox.SetBlockSignals(false);
        }

        private void OnSpinBoxChanged(float newValue)
        {
            Value = (int)newValue;
        }
    }
}
#endif