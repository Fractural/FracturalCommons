using Godot;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class NodePathSelectEditorProperty : EditorProperty
    {
        private NodePathValueProperty _nodePathValueProperty;

        public NodePathSelectEditorProperty() { }
        public NodePathSelectEditorProperty(Node selectRootNode, NodeSelectDialog.NodeConditionFuncDelegate nodeCondition)
        {
            _nodePathValueProperty = new NodePathValueProperty(selectRootNode, nodeCondition);
            _nodePathValueProperty.ValueChanged += (newPath) =>
            {
                EmitChanged(GetEditedProperty(), newPath);
            };
        }

        public override void _Ready()
        {
            _nodePathValueProperty = new NodePathValueProperty();
            if (GetEditedObject() is Node node)
                _nodePathValueProperty.RelativeToNode = node;
        }

        public override void UpdateProperty()
        {
            var nodePath = GetEditedObject().Get<NodePath>(GetEditedProperty());
            _nodePathValueProperty.Value = nodePath; // ValueProperty.UpdateProperty is called automatically when the Value is changed.
        }
    }
}
#endif