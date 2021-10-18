using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EditMethodPopup : Popup
{
	private class MethodOptionData : Godot.Object
	{
		public MethodInfo MethodInfo { get; set; }

		public MethodOptionData(MethodInfo methodInfo)
		{
			MethodInfo = methodInfo;
		}
	}
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
		currentItem = item;
		var eventData = (EventLinkerTree.EventData) currentItem.GetParent().GetMeta("eventData");
		var listenerData = (EventLinkerTree.ListenerData)currentItem.GetMeta("listenerData");
		
		if (listenerData.TargetNode == null)
		{
			GD.PushWarning("Please set the target node of this event listener first!");
			this.Visible = false;
			return;
		}
		
		List<MethodInfo> compatibleListeners = GetCompatibleListeners(listenerData.TargetNode, eventData.EventInfo);

		methodOption.Clear();
		methodOption.AddItem("-- Empty --");
		methodOption.SetItemMetadata(methodOption.GetItemCount() - 1, new MethodOptionData(null));
		foreach (MethodInfo listeningMethodInfo in compatibleListeners)
		{
			methodOption.AddItem($"{listeningMethodInfo.ReturnType} {listeningMethodInfo.Name} ({string.Join(", ", listeningMethodInfo.GetParameters().Select(x => x.ParameterType.Name))})");
			methodOption.SetItemMetadata(methodOption.GetItemCount() - 1, new MethodOptionData(listeningMethodInfo));

			if (listeningMethodInfo == listenerData.MethodInfo)
				methodOption.Select(methodOption.GetItemCount() - 1);
		}
		
		this.Popup_(rect);
	}

	private void OnItemSelected(int index)
	{
		currentItem.SetText(EventLinkerTree.MethodColumn, methodOption.GetItemText(index));
		((EventLinkerTree.ListenerData) currentItem.GetMeta("listenerData")).MethodInfo = ((MethodOptionData) methodOption.GetItemMetadata(index)).MethodInfo;
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
