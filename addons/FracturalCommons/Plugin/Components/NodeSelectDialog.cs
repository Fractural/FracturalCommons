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

        public Node CurrentNode { get; set; }

        private LineEdit _searchBar;
        private Tree _nodeTree;

        public override void _Ready()
        {
            if (NodeUtils.IsInEditorSceneTab(this))
                return;

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
            CurrentNode = node;
            PopupCenteredRatio();
        }

        public void UpdateTree()
        {
            _nodeTree.Clear();

            HashSet<Node> validNodes = null;
            if (_searchBar.Text != "")
            {
                string lowercaseSearchText = _searchBar.Text.ToLower();
                var nodes = new List<Node>();
                GetNodesRecursive(CurrentNode, nodes);
                validNodes = nodes.Where(x => x.Name.ToLower().Find(lowercaseSearchText) > -1).ToHashSet();
                // We clone an array from validNodes to allow us to add whilst traversing.
                foreach (Node validNode in validNodes.ToArray())
                {
                    // Add parents of the valid nodes
                    var parent = validNode.GetParent();
                    while (parent != null)
                    {
                        validNodes.Add(parent);
                        parent = parent.GetParent();
                    }
                }
            }
            CreateTreeRecursive(CurrentNode, null, validNodes);
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

        private void CreateTreeRecursive(Node node, TreeItem parent, HashSet<Node> validNodes)
        {
            if (node == null)
                return;
            if (validNodes != null && !validNodes.Contains(node))
                return;

            var item = _nodeTree.CreateItem(parent);
            item.SetIcon(0, this.GetIconRecursive(node));
            item.SetText(0, node.Name);
            item.SetMeta("node", node);

            foreach (Node child in node.GetChildren())
                CreateTreeRecursive(child, item, validNodes);
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