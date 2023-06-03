using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class StringValueProperty : ValueProperty<string>
    {
        public string PlaceholderText 
        {
            get => _lineEdit.PlaceholderText;
            set => _lineEdit.PlaceholderText = value;
        }

        public Font Font 
        {
            get => _lineEdit.GetFont("font");
            set => _lineEdit.AddFontOverride("font", value);
        }
        
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
            _lineEdit.SetBlockSignals(true);
            _lineEdit.Text = Value;
            _lineEdit.SetBlockSignals(false);
        }

        protected override void OnDisabled(bool disabled)
        {
            if (disabled)
                _lineEdit.ReleaseFocus();
            _lineEdit.FocusMode = disabled ? FocusModeEnum.None : FocusModeEnum.All;
            _lineEdit.Editable = !disabled;
        }

        private void OnTextEntered(string newText)
        {
            Value = newText;
        }

        private void OnFocusExited()
        {
            if (!Disabled)
                Value = _lineEdit.Text;
        }
    }
}
#endif