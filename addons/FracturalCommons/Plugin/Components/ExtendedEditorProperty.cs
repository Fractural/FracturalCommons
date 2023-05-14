using Godot;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public abstract class ExtendedEditorProperty : EditorProperty
    {
        private Godot.Object _overrideEditedObject;
        public Godot.Object EditedObject { get => GetEditedObject(); set => _overrideEditedObject = value; }

        private string _overrideEditedProperty;
        public string EditedProperty { get => GetEditedProperty(); set => _overrideEditedProperty = value; }

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
    }

    [Tool]
    public abstract class ExtendedEditorProperty<T> : ExtendedEditorProperty where T : Godot.Object
    {
        public T EditedObjectTyped { get => (T)EditedObject; set => EditedObject = value; }
    }
}
#endif