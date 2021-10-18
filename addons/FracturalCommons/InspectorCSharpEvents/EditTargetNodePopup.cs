using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EditTargetNodePopup : Control
{
	[Export]
	private NodePath testTreePath;
	[Export]
	private NodePath nodeSelectPopupPath;

	private NodeSelectPopup nodeSelectPopup;
	private TreeItem currentItem;

	public override void _Ready()
	{
		nodeSelectPopup = GetNode<NodeSelectPopup>(nodeSelectPopupPath);

		nodeSelectPopup.Connect(nameof(NodeSelectPopup.NodeSelected), this, nameof(OnNodeSelected));
	}

	public void Popup(TreeItem item)
	{
		currentItem = item;
		nodeSelectPopup.Popup(GetNode(testTreePath));
	}
	
	private void OnNodeSelected(Node node)
	{
		((EventLinkerTree.ListenerData) currentItem.GetMeta("listenerData")).TargetNode = node;
		currentItem.SetIcon(EventLinkerTree.NodeColumn, this.GetIconRecursive(node));
		currentItem.SetText(EventLinkerTree.NodeColumn, node.Name);
		currentItem.SetEditable(EventLinkerTree.MethodColumn, true);
	}
}
