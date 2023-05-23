﻿using Godot;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class NodePathSelectEditorProperty : EditorProperty
    {
        public delegate bool NodeConditionFuncDelegate(Node node, Godot.Object editedObject);

        private NodePathValueProperty _nodePathValueProperty;

        public NodePathSelectEditorProperty() { }
        public NodePathSelectEditorProperty(Node selectRootNode, NodeConditionFuncDelegate nodeCondition = null)
        {
            _nodePathValueProperty = new NodePathValueProperty(selectRootNode, (node) => nodeCondition(node, GetEditedObject()));
            _nodePathValueProperty.ValueChanged += (newPath) =>
            {
                EmitChanged(GetEditedProperty(), newPath);
            };
            AddChild(_nodePathValueProperty);
        }

        public override void _Ready()
        {
            _nodePathValueProperty.RelativeToNode = GetEditedObject() as Node;

            if (GetEditedObject() is Node node)
                _nodePathValueProperty.RelativeToNode = node;
        }

        public override void UpdateProperty()
        {
            var nodePath = GetEditedObject().Get<NodePath>(GetEditedProperty());
            _nodePathValueProperty.SetValue(nodePath, false); // ValueProperty.UpdateProperty is called automatically when the Value is changed.
        }
    }
}
#endif