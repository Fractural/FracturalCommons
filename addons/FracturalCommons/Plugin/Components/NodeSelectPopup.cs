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
    public class NodeSelectPopup : AcceptDialog
    {
        [Signal]
        public delegate void NodeSelected(Node node);

        private LineEdit searchBar;
        private Tree nodeTree;
        private Node currentNode;

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

            searchBar = new LineEdit();
            searchBar.PlaceholderText = "Filter nodes";
            searchBar.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            searchBar.RightIcon = GetIcon("Search", "EditorIcons");

            nodeTree = new Tree();
            nodeTree.SizeFlagsHorizontal = nodeTree.SizeFlagsVertical = (int)SizeFlags.ExpandFill;

            var vBox = new VBoxContainer();
            vBox.SizeFlagsHorizontal = vBox.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
            vBox.AddChild(searchBar);
            vBox.AddChild(nodeTree);

            var marginContainer = new MarginContainer();
            marginContainer.AddChild(vBox);
            AddChild(marginContainer);
            marginContainer.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);

            searchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));
            nodeTree.Connect("item_activated", this, nameof(OnItemActivated));
            nodeTree.Connect("item_selected", this, nameof(OnItemSelected));
        }

        public void Popup(Node node)
        {
            currentNode = node;
            UpdateTree();
            PopupCenteredRatio();
        }

        public void UpdateTree()
        {
            nodeTree.Clear();

            HashSet<Node> validNodes = null;
            if (searchBar.Text != "")
            {
                string lowercaseSearchText = searchBar.Text.ToLower();
                var nodes = new List<Node>();
                GetNodesRecursive(currentNode, nodes);
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
            CreateTreeRecursive(currentNode, null, validNodes);
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

            var item = nodeTree.CreateItem(parent);
            item.SetIcon(0, this.GetIconRecursive(node));
            item.SetText(0, node.Name);
            item.SetMeta("node", node);

            foreach (Node child in node.GetChildren())
                CreateTreeRecursive(child, item, validNodes);
        }

        private void OnItemActivated()
        {
            EmitSignal(nameof(NodeSelected), nodeTree.GetSelected().GetMeta("node"));
            Visible = false;
        }

        private void OnItemSelected()
        {
            if (GetOk().Disabled)
                GetOk().Disabled = false;
        }
    }
}