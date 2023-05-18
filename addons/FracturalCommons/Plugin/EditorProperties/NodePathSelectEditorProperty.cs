using Godot;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class NodePathSelectEditorProperty : EditorProperty
    {
        private NodePathValueProperty _nodePathValueProperty;

        public NodePathSelectEditorProperty(Node selectRootNode, NodeSelectDialog.NodeConditionFuncDelegate nodeCondition = null)
        {
            _nodePathValueProperty = new NodePathValueProperty(selectRootNode, nodeCondition);
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