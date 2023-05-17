using Godot;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class NodePathSelectEditorProperty : EditorProperty
    {
        private NodePathValueProperty _nodePathValueProperty;
        private Node _selectRootNode;
        private NodeSelectDialog.NodeConditionFuncDelegate _nodeCondition;

        public NodePathSelectEditorProperty() { }
        public NodePathSelectEditorProperty(Node selectRootNode, NodeSelectDialog.NodeConditionFuncDelegate nodeCondition = null)
        {
            _selectRootNode = selectRootNode;
            _nodeCondition = nodeCondition;
        }

        public override void _Ready()
        {
            _nodePathValueProperty = new NodePathValueProperty(_selectRootNode, _nodeCondition);
            _nodePathValueProperty.RelativeToNode = GetEditedObject() as Node;
            _nodePathValueProperty.ValueChanged += (newPath) =>
            {
                EmitChanged(GetEditedProperty(), newPath);
            };

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