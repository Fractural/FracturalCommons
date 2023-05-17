using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class FloatValueProperty : ValueProperty<float>
    {
        private SpinBox _spinBox;

        public FloatValueProperty()
        {
            _spinBox = new SpinBox();
            _spinBox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _spinBox.Connect("changed", this, nameof(OnSpinBoxChanged));
            AddChild(_spinBox);
        }

        public override void UpdateProperty()
        {
            _spinBox.Value = Value;
        }

        private void OnSpinBoxChanged(float newValue)
        {
            Value = newValue;
        }
    }
}
#endif