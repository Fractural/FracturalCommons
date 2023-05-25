using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class Vector2ValueProperty : ValueProperty<Vector2>
    {
        private EditorSpinSlider _xSpinSlider;
        private EditorSpinSlider _ySpinSlider;

        public Vector2ValueProperty() : this(0.0001f) { }
        public Vector2ValueProperty(float step) : base()
        {
            var hBox = new HBoxContainer();
            AddChild(hBox);

            _xSpinSlider = new EditorSpinSlider();
            _xSpinSlider.Flat = true;
            _xSpinSlider.Label = "x";
            _xSpinSlider.Step = step;
            _xSpinSlider.AllowLesser = true;
            _xSpinSlider.AllowGreater = true;
            _xSpinSlider.HideSlider = true;
            _xSpinSlider.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _xSpinSlider.Connect("value_changed", this, nameof(OnXSpinSliderChanged));
            hBox.AddChild(_xSpinSlider);

            _ySpinSlider = new EditorSpinSlider();
            _ySpinSlider.Flat = true;
            _ySpinSlider.Label = "y";
            _ySpinSlider.Step = step;
            _ySpinSlider.AllowLesser = true;
            _ySpinSlider.AllowGreater = true;
            _ySpinSlider.HideSlider = true;
            _ySpinSlider.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _ySpinSlider.Connect("value_changed", this, nameof(OnYSpinSliderChanged));
            hBox.AddChild(_ySpinSlider);
        }

        public override void UpdateProperty()
        {
            _xSpinSlider.SetBlockSignals(true);
            _ySpinSlider.SetBlockSignals(true);
            _xSpinSlider.Value = Value.x;
            _ySpinSlider.Value = Value.y;
            _xSpinSlider.SetBlockSignals(false);
            _ySpinSlider.SetBlockSignals(false);
        }

        protected override void OnDisabled(bool disabled)
        {
            _xSpinSlider.ReadOnly = disabled;
            _ySpinSlider.ReadOnly = disabled;
        }

        private void OnXSpinSliderChanged(float newValue)
        {
            var value = Value;
            value.x = newValue;
            Value = value;
        }

        private void OnYSpinSliderChanged(float newValue)
        {
            var value = Value;
            value.y = newValue;
            Value = value;
        }
    }
}
#endif