using Fractural.Utils;
using Godot;
using System;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class DictionaryValueProperty : ValueProperty<bool>
    {
        private Button _editButton;
        private VBoxContainer _vBox;
        private Button _addElementButton;
        private VBoxContainer _contentVBox;

        private Type _keyType;
        private Type _valueType;

        public DictionaryValueProperty(Type keyType, Type valueType) : base()
        {
            _keyType = keyType;
            _valueType = valueType;

            _editButton = new Button();
            _editButton.ToggleMode = true;
            _editButton.ClipText = true;
            _editButton.Connect("pressed", this, nameof(OnEditPressed));
            AddChild(_editButton);

            _vBox = new VBoxContainer();
            _contentVBox = new VBoxContainer();
            _vBox.AddChild(_contentVBox);
        }

        public override void _Ready()
        {
            base._Ready();
            if (NodeUtils.IsInEditorSceneTab(this))
                return;

            SetBottomEditor();
        }

        private void OnEditPressed()
        {

        }
    }
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
            _checkBox.Pressed = Value;
        }

        private void OnCheckBoxToggled(bool pressed)
        {
            Value = pressed;
        }
    }
}
#endif