using Fractural.Utils;
using Godot;
using System;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class DictionaryValuePropertyKeyValueEntry : HBoxContainer
    {
        /// <summary>
        /// KeyChanged(object key, object newKey)
        /// </summary>
        public event Action<object, DictionaryValuePropertyKeyValueEntry> KeyChanged;
        /// <summary>
        /// ValueChanged(object key, object newValue)
        /// </summary>
        public event Action<object, object> ValueChanged;
        /// <summary>
        /// Deleted(object key)
        /// </summary>
        public event Action<object> Deleted;

        public object CurrentKey { get; set; }
        public ValueProperty KeyProperty { get; set; }
        public ValueProperty ValueProperty { get; set; }
        private bool _disabled;
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;

                if (IsInsideTree())
                {
                    KeyProperty.Disabled = value;
                    ValueProperty.Disabled = value;
                    _deleteButton.Disabled = value;
                }
            }
        }

        private Button _deleteButton;

        public DictionaryValuePropertyKeyValueEntry() { }
        public DictionaryValuePropertyKeyValueEntry(ValueProperty keyProperty, ValueProperty valueProperty)
        {
            var control = new Control();
            control.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            control.SizeFlagsStretchRatio = 0.25f;
            control.RectSize = new Vector2(24, 0);
            AddChild(control);

            KeyProperty = keyProperty;
            KeyProperty.ValueChanged += OnKeyChanged;
            KeyProperty.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            ValueProperty = valueProperty;
            ValueProperty.ValueChanged += OnValueChanged;
            ValueProperty.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            AddChild(KeyProperty);

            var label = new Label();
            label.Text = ":  ";
            label.ClipText = true;
            AddChild(label);

            AddChild(ValueProperty);

            _deleteButton = new Button();
            _deleteButton.Connect("pressed", this, nameof(OnDeletePressed));
            AddChild(_deleteButton);
        }

        public override void _Ready()
        {
            base._Ready();
            if (NodeUtils.IsInEditorSceneTab(this))
                return;
            _deleteButton.Icon = GetIcon("Remove", "EditorIcons");
        }

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
            {
                KeyProperty.ValueChanged -= OnKeyChanged;
                ValueProperty.ValueChanged -= OnValueChanged;
            }
        }

        public void SetKeyValue(object key, object value)
        {
            KeyProperty.SetValue(key);
            ValueProperty.SetValue(value);
            CurrentKey = key;
        }

        private void OnKeyChanged(object newKey)
        {
            var oldKey = CurrentKey;
            CurrentKey = newKey;
            KeyChanged?.Invoke(oldKey, this);
        }

        private void OnValueChanged(object newValue) => ValueChanged?.Invoke(CurrentKey, newValue);
        private void OnDeletePressed() => Deleted?.Invoke(CurrentKey);
    }
}
#endif