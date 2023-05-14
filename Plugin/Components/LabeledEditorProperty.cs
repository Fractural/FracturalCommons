using Fractural.Utils;
using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [CSharpScript]
    [Tool]
    public class LabeledEditorProperty : HSplitContainer
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

        private ExtendedEditorProperty _editorProperty;
        public ExtendedEditorProperty EditorProperty
        {
            get => _editorProperty;
            set
            {
                if (IsInsideTree() && _editorProperty != value)
                {
                    RemoveChild(_editorProperty);
                    value.Reparent(this);
                }
                _editorProperty = value;
            }
        }

        private Label _label;
        public LabeledEditorProperty() { }
        public LabeledEditorProperty(string propertyName, ExtendedEditorProperty editorProperty)
        {
            _propertyName = propertyName;
            _editorProperty = editorProperty;
        }

        public override void _Ready()
        {
            if (NodeUtils.IsInEditorSceneTab(this))
                return;
            DraggerVisibility = DraggerVisibilityEnum.Hidden;
            _label = new Label();
            _label.Text = PropertyName;
            _label.SizeFlagsHorizontal = _editorProperty.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            AddChild(_label);
            AddChild(_editorProperty);
        }
    }
}
#endif