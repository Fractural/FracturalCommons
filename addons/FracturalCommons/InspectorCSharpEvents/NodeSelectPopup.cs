using Fractural.Plugin.AssetsRegistry;
using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[Tool]
public class NodeSelectPopup : WindowDialog
{
	[Signal]
	public delegate void NodeSelected(Node node);

	[Export]
	private NodePath searchBarPath;
	[Export]
	private NodePath nodeTreePath;
	[Export]
	private NodePath tintPath;

	private LineEdit searchBar;
	private Tree nodeTree;
	private Control tint;
    private Node currentNode;

	public override void _Ready()
	{
		if (NodeUtils.IsInEditorSceneTab(this))
			return;

		searchBar = GetNode<LineEdit>(searchBarPath);
		
		searchBar.RightIcon = GetIcon("Search", "EditorIcons");
		searchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));

		nodeTree = GetNode<Tree>(nodeTreePath);
		tint = GetNode<Control>(tintPath);
		tint.Visible = false;
		CallDeferred(nameof(DefferedReady));

		nodeTree.Connect("item_activated", this, nameof(OnItemActivated));
		Connect("popup_hide", this, nameof(OnPopupHide));
	}

	private void OnSearchBarTextChanged(string newText)
	{
		UpdateTree();
	}

	private void DefferedReady()
	{
		tint.Reparent(GetParent());
		tint.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
			if (IsInstanceValid(tint))
				tint.QueueFree();
	}

	public void Popup(Node node)
	{
		currentNode = node;
		UpdateTree();
		this.PopupCentered();
		tint.Visible = true;
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
	private void OnPopupHide()
	{
		tint.Visible = false;
	}

	private void OnItemActivated()
	{
		EmitSignal(nameof(NodeSelected), nodeTree.GetSelected().GetMeta("node"));
		this.Visible = false;
	}
}
