using Godot;
using System.Collections.Generic;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class BoolValueProperty : ValueProperty<bool>
    {
        private CheckBox _checkBox;

        public BoolValueProperty()
        {
            _checkBox = new CheckBox();
            _checkBox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _checkBox.Connect("toggled", this, nameof(OnCheckBoxToggled));
            AddChild(_checkBox);
        }

        public override void UpdateProperty()
        {
            _checkBox.SetPressedNoSignal(Value);
        }

        private void OnCheckBoxToggled(bool pressed)
        {
            Value = pressed;
        }
    }
}
#endif