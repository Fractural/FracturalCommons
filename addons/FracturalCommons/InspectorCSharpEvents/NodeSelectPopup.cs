using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class NodeSelectPopup : WindowDialog
{
	[Signal]
	public delegate void NodeSelected(Node node);

	[Export]
	private NodePath nodeTreePath;
	[Export]
	private NodePath tintPath;

	private Tree nodeTree;
	private Control tint;
    private TreeItem currentItem;

	public override void _Ready()
	{
		nodeTree = GetNode<Tree>(nodeTreePath);
		tint = GetNode<Control>(tintPath);
		tint.Visible = false;
		CallDeferred(nameof(DefferedReady));

		nodeTree.Connect("item_activated", this, nameof(OnItemActivated));
		Connect("popup_hide", this, nameof(OnPopupHide));
	}

	private void DefferedReady()
	{
		tint.Reparent(GetParent());
		tint.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
			tint?.QueueFree();
	}

	public void Popup(Node node)
	{
		CreateTree(node);
		this.PopupCentered();
		tint.Visible = true;
	}

	public void CreateTree(Node node)
	{
		nodeTree.Clear();
		CreateTreeRecursive(node, null);
	}

	private void CreateTreeRecursive(Node node, TreeItem parent)
	{
		if (node == null)
			return;

		var item = nodeTree.CreateItem(parent);
		item.SetIcon(0, this.GetIconRecursive(node));
		item.SetText(0, node.Name);
		item.SetMeta("node", node);

		foreach (Node child in node.GetChildren())
			CreateTreeRecursive(child, item);
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
