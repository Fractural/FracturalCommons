using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class StringValueProperty : ValueProperty<string>
    {
        private LineEdit _lineEdit;

        public StringValueProperty() : base()
        {
            _lineEdit = new LineEdit();
            _lineEdit.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _lineEdit.Connect("text_entered", this, nameof(OnTextEntered));
            _lineEdit.Connect("focus_exited", this, nameof(OnFocusExited));
            AddChild(_lineEdit);
        }

        public override void UpdateProperty()
        {
            _lineEdit.Text = Value;
        }

        private void OnTextEntered(string newText)
        {
            Value = newText;
        }

        private void OnFocusExited()
        {
            Value = _lineEdit.Text;
        }
    }
}
#endif