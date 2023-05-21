using Godot;
using System;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class FloatValueProperty : ValueProperty<float>
    {
        private EditorSpinSlider _spinSlider;

        public FloatValueProperty() : this(0.0001f) { }
        public FloatValueProperty(float step) : base()
        {
            _spinSlider = new EditorSpinSlider();
            _spinSlider.Step = step;
            _spinSlider.AllowLesser = true;
            _spinSlider.AllowGreater = true;
            _spinSlider.HideSlider = true;
            _spinSlider.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _spinSlider.Connect("value_changed", this, nameof(OnSpinSliderChanged));
            AddChild(_spinSlider);
        }

        public override void UpdateProperty()
        {
            _spinSlider.SetBlockSignals(true);
            _spinSlider.Value = Value;
            _spinSlider.SetBlockSignals(false);
        }

        protected override void OnDisabled(bool disabled) => _spinSlider.ReadOnly = disabled;

        private void OnSpinSliderChanged(float newValue)
        {
            Value = newValue;
        }
    }
}
#endif