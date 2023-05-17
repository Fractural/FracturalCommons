﻿using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class IntegerValueProperty : ValueProperty<int>
    {
        private SpinBox _spinBox;

        public IntegerValueProperty()
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
            Value = (int)newValue;
        }
    }
}
#endif