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

        public ValueEditorProperty(ValueProperty valueProperty)
        {
            ValueProperty = valueProperty;
            AddChild(valueProperty);
            ValueProperty.ValueChanged += (newValue) =>
            {
                EmitChanged(GetEditedProperty(), newValue);
            };
            ValueProperty.SetBottomEditor = SetBottomEditor;
        }

        public override void UpdateProperty()
        {
            ValueProperty.SetValue(GetEditedObject().Get(GetEditedProperty()), false);
        }
    }
}
#endif