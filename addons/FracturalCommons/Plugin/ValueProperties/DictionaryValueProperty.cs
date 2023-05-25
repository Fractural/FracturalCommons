using Fractural.Utils;
using Godot;
using System;
using System.Collections;
using System.Linq;
using GDC = Godot.Collections;

#if TOOLS
namespace Fractural.Plugin
{
    // TODO LATER: Add pagination
    [Tool]
    public class DictionaryValueProperty : ValueProperty<GDC.Dictionary>
    {
        private Button _editButton;
        private Control _container;
        private Button _addElementButton;
        private VBoxContainer _keyValueEntriesVBox;

        private Type _keyType;
        private Type _valueType;
        private Node _sceneRoot;
        private Node _relativeToNode;
        private string EditButtonText => $"[{_keyType.Name}]:{_valueType.Name} [{Value.Count}]";

        public DictionaryValueProperty() { }
        public DictionaryValueProperty(Type keyType, Type valueType, Node sceneRoot, Node relativeToNode) : base()
        {
            _keyType = keyType;
            _valueType = valueType;
            _sceneRoot = sceneRoot;
            _relativeToNode = relativeToNode;

            _editButton = new Button();
            _editButton.ToggleMode = true;
            _editButton.ClipText = true;
            _editButton.Connect("toggled", this, nameof(OnEditToggled));
            AddChild(_editButton);

            _addElementButton = new Button();
            _addElementButton.Text = "Add Key Value Pair";
            _addElementButton.Connect("pressed", this, nameof(OnAddElementPressed));
            _addElementButton.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _addElementButton.RectMinSize = new Vector2(24 * 4, 0);

            _keyValueEntriesVBox = new VBoxContainer();

            var vbox = new VBoxContainer();
            vbox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            vbox.AddChild(_addElementButton);
            vbox.AddChild(_keyValueEntriesVBox);

            _container = vbox;
            AddChild(_container);
        }

        public override void _Ready()
        {
#if TOOLS
            if (NodeUtils.IsInEditorSceneTab(this))
                return;
#endif
            _addElementButton.Icon = GetIcon("Add", "EditorIcons");
            GetViewport().Connect("gui_focus_changed", this, nameof(OnFocusChanged));
        }

        public override void _Process(float delta)
        {
            // We need SetBottomEditor to run here because it won't work in _Ready due to
            // the tree being busy setting up nodes.
            SetBottomEditor(_container);
            SetProcess(false);
        }

        private Control _currentFocused;
        private void OnFocusChanged(Control control) => _currentFocused = control;

        public override void UpdateProperty()
        {
            _container.Visible = this.GetMeta<bool>("visible", false);
            _editButton.Pressed = _container.Visible;

            _editButton.Text = EditButtonText;

            int index = 0;
            int childCount = _keyValueEntriesVBox.GetChildCount();

            var currFocusedEntry = _currentFocused?.GetAncestor<DictionaryValuePropertyKeyValueEntry>();
            if (currFocusedEntry != null)
            {
                int keyIndex = 0;
                foreach (var key in Value.Keys)
                {
                    if (key != null && key.Equals(currFocusedEntry.CurrentKey))
                        break;
                    keyIndex++;
                }
                if (keyIndex == Value.Keys.Count)
                {
                    // Set current focused entry back to null. We couldn't
                    // find the entry in the new dictionary, meaning this entry
                    // must have been deleted, therefore we don't care about it
                    // anymore.
                    currFocusedEntry = null;
                }
                else
                {
                    var targetEntry = _keyValueEntriesVBox.GetChild<DictionaryValuePropertyKeyValueEntry>(keyIndex);
                    _keyValueEntriesVBox.SwapChildren(targetEntry, currFocusedEntry);
                }
            }

            foreach (var key in Value.Keys)
            {
                DictionaryValuePropertyKeyValueEntry entry;
                if (index >= childCount)
                    entry = CreateDefaultEntry();
                else
                    entry = _keyValueEntriesVBox.GetChild<DictionaryValuePropertyKeyValueEntry>(index);

                if (currFocusedEntry == null || entry != currFocusedEntry)
                    entry.SetKeyValue(key, Value[key]);
                index++;
            }

            // Free extra entries
            if (index < childCount)
            {
                for (int i = childCount - 1; i >= index; i--)
                {
                    var entry = _keyValueEntriesVBox.GetChild<DictionaryValuePropertyKeyValueEntry>(i);
                    entry.KeyChanged -= OnDictKeyChanged;
                    entry.ValueChanged -= OnDictValueChanged;
                    entry.QueueFree();
                }
            }

            if (!IsInstanceValid(currFocusedEntry))
                currFocusedEntry = null;

            var nextKey = DefaultValueUtils.GetDefault(_keyType, Value.Keys);
            _addElementButton.Disabled = Value.Contains(nextKey);
        }

        protected override void OnDisabled(bool disabled)
        {
            foreach (DictionaryValuePropertyKeyValueEntry entry in _keyValueEntriesVBox.GetChildren())
                entry.Disabled = disabled;
        }

        private new ValueProperty CreateValueProperty(Type type)
        {
            var property = ValueProperty.CreateValueProperty(type);
            if (type == typeof(NodePath) && property is NodePathValueProperty valueProperty)
            {
                valueProperty.SelectRootNode = _sceneRoot;
                valueProperty.RelativeToNode = _relativeToNode;
            }
            return property;
        }

        private DictionaryValuePropertyKeyValueEntry CreateDefaultEntry()
        {
            var entry = new DictionaryValuePropertyKeyValueEntry(CreateValueProperty(_keyType), CreateValueProperty(_valueType));
            entry.KeyChanged += OnDictKeyChanged;
            entry.ValueChanged += OnDictValueChanged;
            entry.Deleted += OnDictKeyDeleted;
            // Add entry if we ran out of existing ones
            _keyValueEntriesVBox.AddChild(entry);

            return entry;
        }

        private void OnDictKeyChanged(object oldKey, DictionaryValuePropertyKeyValueEntry entry)
        {
            var newKey = entry.CurrentKey;
            if (Value.Contains(newKey))
            {
                // Revert CurrentKey back
                entry.CurrentKey = oldKey;
                // Reject change since the newKey already exists
                entry.KeyProperty.SetValue(oldKey);
                return;
            }
            var currValue = Value[oldKey];
            Value.Remove(oldKey);
            Value[newKey] = currValue;
            InvokeValueChanged(Value);
        }

        private void OnDictValueChanged(object key, object newValue)
        {
            Value[key] = newValue;
            InvokeValueChanged(Value);
        }

        private void OnDictKeyDeleted(object key)
        {
            Value.Remove(key);
            InvokeValueChanged(Value);
        }

        private void OnAddElementPressed()
        {
            // The adding is done in UpdateProperty
            // Note the edited a field in Value doesn't invoke ValueChanged, so we must do it manually
            //
            // Use default types for the newly added element
            var nextKey = DefaultValueUtils.GetDefault(_keyType, Value.Keys);
            Value[nextKey] = DefaultValueUtils.GetDefault(_valueType);
            InvokeValueChanged(Value);
        }

        private void OnEditToggled(bool toggled)
        {
            SetMeta("visible", toggled);
            _container.Visible = toggled;
        }
    }
}
#endif