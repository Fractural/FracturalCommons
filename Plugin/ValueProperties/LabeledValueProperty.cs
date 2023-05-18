﻿using Fractural.Utils;
using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class LabeledValueProperty : WrappedValueProperty
    {
        private string _propertyName = "";
        [Export]
        public string PropertyName
        {
            get => _propertyName;
            set
            {
                _propertyName = value;
                if (IsInsideTree())
                    _label.Text = _propertyName;
            }
        }

        private Label _label;

        public LabeledValueProperty(string propertyName) : base()
        {
            _propertyName = propertyName;
        }

        public override void _Ready()
        {
            base._Ready();
            if (NodeUtils.IsInEditorSceneTab(this))
                return;

            _label = new Label();
            _label.Text = PropertyName;
            _label.SizeFlagsHorizontal = ValueProperty.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

            var hSplit = new HSplitContainer();
            hSplit.DraggerVisibility = SplitContainer.DraggerVisibilityEnum.Hidden;
            hSplit.AddChild(_label);
            hSplit.AddChild(ValueProperty);

            AddChild(hSplit);
        }
    }
}
#endif