using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class StringValueProperty : ValueProperty<string>
    {
        private LineEdit _lineEdit;

        public StringValueProperty()
        {
            _lineEdit = new LineEdit();
            _lineEdit.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _lineEdit.Connect("text_changed", this, nameof(OnLineEditChanged));
            AddChild(_lineEdit);
        }

        public override void UpdateProperty()
        {
            _lineEdit.Text = Value;
        }

        private void OnLineEditChanged(string newText)
        {
            Value = newText;
        }
    }
}
#endif