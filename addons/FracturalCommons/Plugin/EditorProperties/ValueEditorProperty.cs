using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;

#if TOOLS
namespace Fractural.Plugin
{
    /// <summary>
    /// Wraps around a ValueProperty to make it into a standalone EditorProperty
    /// Note that this might not work for all ValueProperties. Some ValueProperties like 
    /// NodePathValueProperty has it's own dedicated NodePathEditorProperty.
    /// </summary>
    [Tool]
    public class ValueEditorProperty : EditorProperty
    {
        public ValueProperty ValueProperty { get; set; }

        public ValueEditorProperty() { }
        public ValueEditorProperty(ValueProperty valueProperty)
        {
            ValueProperty = valueProperty;
            ValueProperty.ValueChanged += (newValue) =>
            {
                EmitChanged(GetEditedProperty(), newValue);
            };
            ValueProperty.MetaChanged += (key, value) =>
            {
                GD.Print("Saving meta ", key, " = ", value);
                GetEditedObject().SetMeta($"{GetEditedProperty()}/{key}", value);
            };

            ValueProperty.SetBottomEditor = OnSetBottomEditor;
            AddChild(valueProperty);
        }

        private void OnSetBottomEditor(Control control)
        {
            if (control != null)
            {
                control.Reparent(this);
                SetBottomEditor(control);
            }
        }

        public override void UpdateProperty()
        {
            var editedObject = GetEditedObject();
            var editedProperty = GetEditedProperty();
            foreach (var metaKey in editedObject.GetMetaList())
                if (metaKey.StartsWith($"{editedProperty}/"))
                {
                    GD.Print("Updating meta ", metaKey.TrimPrefix($"{editedProperty}/"), " = ", editedObject.GetMeta(metaKey));
                    ValueProperty.SetMeta(metaKey.TrimPrefix($"{editedProperty}/"), editedObject.GetMeta(metaKey), false);
                }

            ValueProperty.SetValue(editedObject.Get(editedProperty), false);
        }
    }
}
#endif