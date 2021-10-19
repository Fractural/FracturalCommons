using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[Tool]
public class MethodSelectPopup : WindowDialog
{
	public class MethodItemData : Godot.Object
	{
		public MethodItemData() { }
		public MethodItemData(MethodInfo methodInfo)
		{
			MethodInfo = methodInfo;
		}

		public MethodInfo MethodInfo { get; set; }
	}

	[Signal]
	public delegate void MethodSelected(MethodItemData itemData);

	[Export]
	private NodePath searchBarPath;
	[Export]
	private NodePath nodeItemListPath;
	[Export]
	private NodePath tintPath;

	private LineEdit searchBar;
	private ItemList nodeItemList;
	private Control tint;
	private TreeItem currentItem;

	private List<MethodInfo> currentCompatibleListeners;

	public override void _Ready()
	{
		if (NodeUtils.IsInEditorSceneTab(this))
			return;

		searchBar = GetNode<LineEdit>(searchBarPath);
		nodeItemList = GetNode<ItemList>(nodeItemListPath);
		tint = GetNode<Control>(tintPath);

		searchBar.RightIcon = GetIcon("Search", "EditorIcons");
		searchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));

		tint.Visible = false;
		CallDeferred(nameof(DefferedReady));
		
		nodeItemList.Connect("item_activated", this, nameof(OnItemActivated));
		Connect("popup_hide", this, nameof(OnPopupHide));
	}

	private void OnSearchBarTextChanged(string newText)
	{
		UpdateMethodList();
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

	public void Popup(Node node, EventInfo eventInfo)
	{
		currentCompatibleListeners = GetCompatibleListeners(node, eventInfo);
		UpdateMethodList();
		this.PopupCentered();
		tint.Visible = true;
	}

	public void UpdateMethodList()
	{
		nodeItemList.Clear();
		foreach (MethodInfo listener in currentCompatibleListeners)
		{
			if (searchBar.Text != "" && listener.Name.ToLower().Find(searchBar.Text.ToLower()) < 0)
				continue;
			nodeItemList.AddItem(listener.Name);
			nodeItemList.SetItemMetadata(nodeItemList.GetItemCount() - 1, new MethodItemData(listener));
		}
	}

	private void OnPopupHide()
	{
		nodeItemList.Clear();
		tint.Visible = false;
	}

	private void OnItemActivated(int index)
	{
		EmitSignal(nameof(MethodSelected), (MethodItemData) nodeItemList.GetItemMetadata(index));
		this.Visible = false;
	}

	private List<MethodInfo> GetCompatibleListeners(Node godotObj, EventInfo eventInfo)
	{
		if (godotObj == null)
			GD.Print("GetCompLis GodotObj null!");
		if (eventInfo == null)
			GD.Print("getcomplis eventinfo null!");
		var methods = EditorUtils.GetRealType(godotObj).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => !m.IsSpecialName).OrderBy(x => x.Name);
		List<MethodInfo> compatibleListeners = new List<MethodInfo>();
		// GD.Print("----");
		// GD.Print($"Event '{eventInfo.Name}': Return type: {eventInfo.EventHandlerType.GetMethod("Invoke").ReturnType.Name} Parameters: {string.Join(", ", eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType.Name))}");
		foreach (MethodInfo methodInfo in methods)
		{
			// GD.Print($"Method '{methodInfo.Name}': Return type: {methodInfo.ReturnType.Name} Parameters: {string.Join(", ", methodInfo.GetParameters().Select(x => x.ParameterType.Name))}");
			if (eventInfo.EventHandlerType.GetMethod("Invoke").ReturnType.Equals(methodInfo.ReturnType)
				&& IsSameParameterSignature(methodInfo.GetParameters(), eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters()))
				compatibleListeners.Add(methodInfo);

		}
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
