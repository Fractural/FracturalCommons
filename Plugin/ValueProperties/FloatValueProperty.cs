using Godot;
using System;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class FloatValueProperty : ValueProperty<float>
    {
        private EditorSpinSlider _spinSlider;

        public FloatValueProperty(float step = 0.0001f) : base()
        {
            _spinSlider = new EditorSpinSlider();
            _spinSlider.Step = step;
            _spinSlider.AllowLesser = true;
            _spinSlider.AllowGreater = true;
            _spinSlider.HideSlider = true;
            _spinSlider.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _spinSlider.Connect("value_changed", this, nameof(OnSpinBoxChanged));
            AddChild(_spinSlider);
        }

        public override void UpdateProperty()
        {
            _spinSlider.Value = Value;
        }

        private void OnSpinBoxChanged(float newValue)
        {
            Value = newValue;
        }
    }
}
#endif