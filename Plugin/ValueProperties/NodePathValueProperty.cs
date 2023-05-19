using Godot;
using System;
using Fractural.Utils;
using GDC = Godot.Collections;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public class NodePathValueProperty : ValueProperty<NodePath>
    {
        public Node RelativeToNode { get; set; }
        private Node _selectRootNode;
        public Node SelectRootNode
        {
            get => _selectRootNode;
            set
            {
                _selectRootNode = value;
                _nodeSelectDialog.RootNode = value;
            }
        }
        private NodeSelectDialog.NodeConditionFuncDelegate _nodeConditionFunc;
        public NodeSelectDialog.NodeConditionFuncDelegate NodeConditionFunc
        {
            get => _nodeConditionFunc;
            set
            {
                _nodeConditionFunc = value;
                if (_nodeConditionFunc != null)
                    _nodeSelectDialog.NodeConditionFunc = value;
            }
        }

        private NodeSelectDialog _nodeSelectDialog;
        private ForwardDragDropButton _selectButton;
        private Button _clearButton;

        public NodePathValueProperty() : this(null, null) { }
        public NodePathValueProperty(Node selectRootNode = null, NodeSelectDialog.NodeConditionFuncDelegate nodeConditionFunc = null) : base()
        {
            _nodeSelectDialog = new NodeSelectDialog();
            SelectRootNode = selectRootNode;
            NodeConditionFunc = nodeConditionFunc;
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
            hBox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            AddChild(hBox);
        }

        public override void _Ready()
        {
            _clearButton.Icon = GetIcon("Clear", "EditorIcons");
        }

        public override void UpdateProperty()
        {
            var nodePath = Value;
            if (nodePath == null || nodePath.IsEmpty())
            {
                _selectButton.Icon = null;
                _selectButton.Text = "Assign...";
                return;
            }

            if (RelativeToNode != null && RelativeToNode.HasNode(nodePath))
            {
                // Path relative to RelativeNode
                var node = RelativeToNode.GetNode(nodePath);
                _selectButton.Icon = GetIcon(node.GetType().Name, "EditorIcons");
                _selectButton.Text = node.Name;
            }
            else
            {
                // Path relative to RootNode
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
            SelectPathTo(GetNode(firstDroppedNodePath));
        }

        private void SelectPathTo(Node selectedNode)
        {
            if (selectedNode == null)
                Value = new NodePath();
            else if (RelativeToNode != null)
                // Use relative path if RelativeToNode exists
                Value = RelativeToNode.GetPathTo(selectedNode);
            else
                // Use root path
                Value = _nodeSelectDialog.RootNode.GetPathTo(selectedNode);
        }

        private void OnSelectButtonPressed() => _nodeSelectDialog.SoloEditorWindowPopup(() => _nodeSelectDialog.PopupCenteredRatio());
        private void OnNodeSelected(Node node) => SelectPathTo(node);
        private void OnClearButtonPressed() => SelectPathTo(null);
    }
}
#endif