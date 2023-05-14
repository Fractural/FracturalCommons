using Godot;
using System.Collections.Generic;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public abstract class ExtendedEditorProperty : EditorProperty
    {
        public delegate void PropertyValueChanged(object value);

        protected Godot.Object _overrideEditedObject;
        public Godot.Object EditedObject { get => GetEditedObject(); set => _overrideEditedObject = value; }

        protected object _overrideEditedPropertyValue;
        public object EditedPropertyValue
        {
            get
            {
                if (_overrideEditedPropertyValue != null)
                    return _overrideEditedPropertyValue;
                return GetEditedObject()?.Get(GetEditedProperty());
            }
            set
            {
                if (GetEditedObject() != null)
                    // This property is used as an editor property
                    EmitChanged(GetEditedProperty(), value);
                else
                    // This property is used manually with a value override
                    _overrideEditedPropertyValue = value;
            }
        }

        protected string _overrideEditedProperty;
        public string EditedProperty { get => GetEditedProperty(); set => _overrideEditedProperty = value; }

        public ExtendedEditorProperty() { }
        public ExtendedEditorProperty(Godot.Object editedObject, string editedProperty)
        {
            _overrideEditedObject = editedObject;
            _overrideEditedProperty = editedProperty;
        }
        public ExtendedEditorProperty(object editedPropertyValue)
        {
            _overrideEditedPropertyValue = editedPropertyValue;
        }

        public new Godot.Object GetEditedObject()
        {
            if (_overrideEditedObject != null)
                return _overrideEditedObject;
            return base.GetEditedObject();
        }

        public new string GetEditedProperty()
        {
            if (_overrideEditedProperty != null)
                return _overrideEditedProperty;
            return base.GetEditedProperty();
        }

        public void EmitChanged()
        {
            EmitSignal(nameof(PropertyValueChanged), EditedPropertyValue);
        }
    }

    [Tool]
    public abstract class ExtendedEditorProperty<T> : ExtendedEditorProperty
    {
        public T EditedPropertyValueTyped { get => (T)EditedPropertyValue; set => EditedPropertyValue = value; }

        public ExtendedEditorProperty() { }
        public ExtendedEditorProperty(Godot.Object editedObject, string editedProperty)
        {
            _overrideEditedObject = editedObject;
            _overrideEditedProperty = editedProperty;
        }
        public ExtendedEditorProperty(T editedPropertyValue)
        {
            _overrideEditedPropertyValue = editedPropertyValue;
        }
    }
}
#endif