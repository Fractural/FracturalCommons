using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EditMethodPopup : Popup
{
	[Export]
	private NodePath methodOptionPath;

	private OptionButton methodOption;
    private TreeItem currentItem;

	public override void _Ready()
	{
		methodOption = GetNode<OptionButton>(methodOptionPath);

		methodOption.Connect("item_selected", this, nameof(OnItemSelected));
	}

	public void Popup(Rect2 rect, TreeItem item)
    {
        var methodInfo = (MethodInfo) currentItem.GetMeta("methodInfo");
		var targetNode = (Node) currentItem.GetMeta("targetNode");
		var eventInfo = (EventInfo) currentItem.GetParent().GetMeta("eventInfo");
		if (targetNode == null)
		{
			this.Visible = false;
			return;
		}

		currentItem = item;
		List<MethodInfo> compatibleListeners = GetCompatibleListeners(targetNode, eventInfo);

		methodOption.Clear();
		foreach (MethodInfo listenerInfo in compatibleListeners)
		{
			methodOption.AddItem($"{listenerInfo.ReturnType} {listenerInfo.Name} ({string.Join(", ", listenerInfo.GetParameters().Select(x => x.ParameterType.Name))})");
			methodOption.SetItemMetadata(methodOption.GetItemCount() - 1, listenerInfo);
		}

		this.Popup_(rect);
	}

	private void OnItemSelected(int index)
	{
		currentItem.SetText(EventLinkerTree.MethodColumn, methodOption.GetItemText(index));
		currentItem.SetMeta("methodInfo", methodOption.GetItemMetadata(index));
		this.Visible = false;
	}

	private List<MethodInfo> GetCompatibleListeners(Node node, EventInfo eventInfo) 
	{
		List<MethodInfo> compatibleListeners = new List<MethodInfo>();
		foreach (MethodInfo methodInfo in node.GetType().GetMethods())
			if (IsSameParameterSignature(methodInfo.GetParameters(), eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters()))
				compatibleListeners.Add(methodInfo);
		return compatibleListeners;
	}

	private bool IsSameParameterSignature(ParameterInfo[] parametersOne, ParameterInfo[] parametersTwo)
	{
		if (parametersOne.Length != parametersTwo.Length)
			return false;
		for (int i = 0; i < parametersOne.Length; i++)
		{
			if (parametersOne[i].ParameterType != parametersTwo[i].ParameterType)
				return false;
		}
		return true;
	}
}
