using Fractural.Plugin.AssetsRegistry;
using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fractural.Plugin
{
    [CSharpScript]
    [Tool]
    public class NodeSelectDialog : AcceptDialog
    {
        [Signal]
        public delegate void NodeSelected(Node node);
        public delegate bool NodeConditionFuncDelegate(Node node);

        /// <summary>
        /// Root node to build the node tree that's shown inside the dialog.
        /// </summary>
        public Node RootNode { get; set; }
        /// <summary>
        /// Function that determines whether a node should be
        /// shown in the dialog or not.
        /// </summary>
        public NodeConditionFuncDelegate NodeConditionFunc { get; set; } = (x) => true;

        private LineEdit _searchBar;
        private Tree _nodeTree;

        public override void _Ready()
        {
#if TOOLS
            if (NodeUtils.IsInEditorSceneTab(this))
                return;
#endif
            GetOk().Disabled = true;
            GetOk().Connect("pressed", this, nameof(OnItemActivated));

            var cancelButton = AddCancel("Cancel");
            cancelButton.Connect("pressed", this, nameof(OnCancelled));

            RectSize = new Vector2(400, 200);
            WindowTitle = "Select a Node";

            _searchBar = new LineEdit();
            _searchBar.PlaceholderText = "Filter nodes";
            _searchBar.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _searchBar.RightIcon = GetIcon("Search", "EditorIcons");

            _nodeTree = new Tree();
            _nodeTree.SizeFlagsHorizontal = _nodeTree.SizeFlagsVertical = (int)SizeFlags.ExpandFill;

            var vBox = new VBoxContainer();
            vBox.SizeFlagsHorizontal = vBox.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
            vBox.AddChild(_searchBar);
            vBox.AddChild(_nodeTree);

            var marginContainer = new MarginContainer();
            marginContainer.AddChild(vBox);
            AddChild(marginContainer);
            marginContainer.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);

            _searchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));
            _nodeTree.Connect("item_activated", this, nameof(OnItemActivated));
            _nodeTree.Connect("item_selected", this, nameof(OnItemSelected));

            Connect("about_to_show", this, nameof(UpdateTree));
        }

        public void Popup(Node node)
        {
            RootNode = node;
            PopupCenteredRatio();
        }

        public void UpdateTree()
        {
            _nodeTree.Clear();

            // Key:   Node
            // Value: Whether the Node (stored as the key) meets NodeConditionFunc
            Dictionary<Node, bool> validNodeAndConditionDict = new Dictionary<Node, bool>();

            string lowercaseSearchText = _searchBar.Text.ToLower();
            var nodes = new List<Node>();
            GetNodesRecursive(RootNode, nodes);
            foreach (var node in nodes)
            {
                if ((lowercaseSearchText == "" || node.Name.ToLower().Find(lowercaseSearchText) > -1) && NodeConditionFunc(node))
                    validNodeAndConditionDict.Add(node, true);
            }

            // We clone an array from validNodes to allow us to add whilst traversing.
            foreach (Node validNode in validNodeAndConditionDict.Keys.ToArray())
            {
                // Add parents of the valid nodes
                var parent = validNode.GetParent();
                while (parent != RootNode.GetParent())
                {
                    // If validNodes doesn't already contain the parent, then it must mean
                    // this parent didn't meet the condition
                    if (!validNodeAndConditionDict.ContainsKey(parent))
                        validNodeAndConditionDict.Add(parent, false);
                    parent = parent.GetParent();
                }
            }
            CreateTreeRecursive(RootNode, null, validNodeAndConditionDict);
        }

        private void OnCancelled()
        {
            Hide();
        }

        private void OnSearchBarTextChanged(string newText)
        {
            UpdateTree();
        }

        private void GetNodesRecursive(Node node, List<Node> list)
        {
            list.Add(node);
            foreach (Node child in node.GetChildren())
                GetNodesRecursive(child, list);
        }

        private void CreateTreeRecursive(Node node, TreeItem parent, Dictionary<Node, bool> validNodeAndConditionDict)
        {
            if (node == null)
                return;
            if (validNodeAndConditionDict != null && !validNodeAndConditionDict.ContainsKey(node))
                return;

            var item = _nodeTree.CreateItem(parent);
            item.SetIcon(0, this.GetIconRecursive(node));
            item.SetText(0, node.Name);
            item.SetMeta("node", node);
            // This tree item is only selectable if NodeConditionFunc returned true for this node.
            // Since validNodes must also contain the parent nodes of the valid nodes, the parent
            // nodes might not always pass the NodeConditionFunc. Hence we have to check.
            item.SetSelectable(0, validNodeAndConditionDict[node]);
            if (!validNodeAndConditionDict[node])
                item.SetCustomColor(0, new Color(GetColor("disabled_font_color", "Editor"), 0.5f));

            foreach (Node child in node.GetChildren())
                CreateTreeRecursive(child, item, validNodeAndConditionDict);
        }

        private void OnItemActivated()
        {
            EmitSignal(nameof(NodeSelected), _nodeTree.GetSelected().GetMeta("node"));
            Visible = false;
        }

        private void OnItemSelected()
        {
            if (GetOk().Disabled)
                GetOk().Disabled = false;
        }
    }
}