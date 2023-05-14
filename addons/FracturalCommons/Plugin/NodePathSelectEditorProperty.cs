using Godot;
using System;
using Fractural.Utils;
using GDC = Godot.Collections;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class NodePathSelectEditorProperty : EditorProperty
    {
        public delegate bool NodeConditionFuncDelegate(Node node, Godot.Object editedObject);
        private NodeSelectDialog _nodeSelectDialog;
        private ForwardDragDropButton _selectButton;
        private Button _clearButton;

        public NodePathSelectEditorProperty() { }
        public NodePathSelectEditorProperty(Node currentNode, NodeConditionFuncDelegate nodeConditionFunc)
        {
            _nodeSelectDialog = new NodeSelectDialog();
            _nodeSelectDialog.CurrentNode = currentNode;
            _nodeSelectDialog.NodeConditionFunc = (node) => nodeConditionFunc(node, GetEditedObject());
            _nodeSelectDialog.Connect(nameof(NodeSelectDialog.NodeSelected), this, nameof(OnNodeSelected));
            AddChild(_nodeSelectDialog);

            _selectButton = new ForwardDragDropButton();
            _selectButton.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _selectButton.ClipText = true;
            _selectButton.Connect("pressed", this, nameof(OnSelectButtonPressed));
            _selectButton.CanDropDataFunc = CanDropData;
            _selectButton.DropDataFunc = DropData;

            _clearButton = new Button();
            _clearButton.Connect("pressed", this, nameof(OnClearButtonPressed));

            var hBox = new HBoxContainer();
            hBox.AddChild(_selectButton);
            hBox.AddChild(_clearButton);

            AddChild(hBox);
        }

        public override void _Ready()
        {
            _clearButton.Icon = GetIcon("Clear", "EditorIcons");
        }

        public override void UpdateProperty()
        {
            var nodePath = GetEditedObject().Get<NodePath>(GetEditedProperty());
            if (nodePath == null || nodePath.IsEmpty())
            {
                _selectButton.Icon = null;
                _selectButton.Text = "Assign...";
                return;
            }

            if (((Node)GetEditedObject()).HasNode(nodePath))
            {
                var node = ((Node)GetEditedObject()).GetNode(nodePath);
                _selectButton.Icon = GetIcon(node.GetType().Name, "EditorIcons");
                _selectButton.Text = node.Name;
            }
            else
            {
                _selectButton.Icon = null;
                _selectButton.Text = nodePath.ToString();
            }
        }

        public override bool CanDropData(Vector2 position, object data)
        {
            var dataDict = data as GDC.Dictionary;
            if (dataDict.Get<string>("type") == "nodes")
            {
                var droppedNodePaths = dataDict.Get<GDC.Array>("nodes");
                var firstDroppedNode = GetNode(droppedNodePaths.ElementAt<NodePath>(0));
                if (_nodeSelectDialog.NodeConditionFunc(firstDroppedNode))
                    return true;
            }
            return false;
        }

        public override void DropData(Vector2 position, object data)
        {
            var dataDict = data as GDC.Dictionary;
            var firstDroppedNodePath = dataDict.Get<GDC.Array>("nodes").ElementAt<NodePath>(0);
            if (GetEditedObject() is Node node)
                EmitChanged(GetEditedProperty(), node.GetPathTo(GetNode(firstDroppedNodePath)));
            else
                EmitChanged(GetEditedProperty(), firstDroppedNodePath);
        }

        private void OnSelectButtonPressed() => _nodeSelectDialog.SoloEditorWindowPopup(() => _nodeSelectDialog.PopupCenteredRatio());
        private void OnNodeSelected(Node node) => EmitChanged(GetEditedProperty(), ((Node)GetEditedObject()).GetPathTo(node));
        private void OnClearButtonPressed() => EmitChanged(GetEditedProperty(), new NodePath());
    }
}
#endif