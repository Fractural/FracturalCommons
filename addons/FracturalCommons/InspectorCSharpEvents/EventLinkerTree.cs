using Godot;
using System;
using System.Reflection;
using System.Linq;
using Fractural.Utils;
using System.Collections.Generic;

public class EventLinkerTree : Tree
{
    public class NodeData : Godot.Object
    {
        public NodeData() { }
		public NodeData(Node node)
		{
			Node = node;
		}

		public Node Node { get; set; }
	}

    public class EventData : Godot.Object
    {
        public EventData() { }
		public EventData(EventInfo info)
		{
			EventInfo = info;
		}

		public EventInfo EventInfo { get; set; }
        public List<ListenerData> ListenersData { get; set; }
    }

    public class ListenerData : Godot.Object
    {
        public ListenerData() { }
		public ListenerData(Node targetNode, MethodInfo info)
		{
			TargetNode = targetNode;
			MethodInfo = info;
		}

		public Node TargetNode { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public List<MethodInfo> Options { get; set; }
	}
    
    public enum ButtonID
    {
        AddListener,
        RemoveListener,
	}

    public static int NodeColumn => 0;
    public static int MethodColumn => 1;

    public Node SceneRoot => GetNode(testTreeRootPath);

    [Export]
    private NodePath testTreeRootPath;
    [Export]
    private NodePath editMethodPopupPath;
    [Export]
    private NodePath editTargetNodePopupPath;

    private EditMethodPopup editMethodPopup;
    private NodeSelectPopup editTargetNodePopup;
    
    public override void _Ready()
    {
        editMethodPopup = GetNode<EditMethodPopup>(editMethodPopupPath);
        editTargetNodePopup = GetNode<NodeSelectPopup>(editTargetNodePopupPath);

        Connect("custom_popup_edited", this, nameof(OnCustomPopupEdited));
        Connect("button_pressed", this, nameof(OnButtonPressed));
        Connect("item_edited", this, nameof(OnItemEdited));
        editTargetNodePopup.Connect(nameof(NodeSelectPopup.NodeSelected), this, nameof(OnTargetNodeSelected));

        this.HideRoot = true;
        this.Columns = 3;
        this.SetColumnExpand(NodeColumn, true);
        this.SetColumnExpand(MethodColumn, true);
        this.SetColumnExpand(2, false);
        this.SetColumnMinWidth(2, 28);

        CreateTree(SceneRoot);
    }

    public void CreateTree(Node node)
    {
        Clear();
        TreeItem root = this.CreateItem();
        CreateTreeRecursive(node);
	}

	private void CreateTreeRecursive(Node node)
	{
        if (node == null)
            return;

        CreateNodeItem(node);

        foreach (Node child in node.GetChildren())
            CreateTreeRecursive(child);
	}

    private void OnTargetNodeSelected(Node node)
    {
        var listenerItem = GetEdited();
        var eventData = (EventData)listenerItem.GetParent().GetMeta("eventData");
        var listenerData = ((ListenerData)listenerItem.GetMeta("listenerData"));

        listenerData.TargetNode = node;
        listenerData.Options = GetCompatibleListeners(listenerData.TargetNode, eventData.EventInfo);

        listenerItem.SetEditable(MethodColumn, true);
        listenerItem.SetIcon(NodeColumn, this.GetIconRecursive(node));
        listenerItem.SetText(NodeColumn, node.Name);

        var text = "-- Empty --";
        foreach (MethodInfo info in listenerData.Options)
            text += $",{info.ReturnType} {info.Name} ({string.Join(", ", info.GetParameters().Select(x => x.ParameterType.Name))})";
        listenerItem.SetText(MethodColumn, text);
    }

    private void OnCustomPopupEdited(bool arrowClicked)
	{
        var editedItem = GetEdited();
        if (GetEditedColumn() == MethodColumn)
            editMethodPopup.Popup(GetCustomPopupRect(), editedItem);
        else if (GetEditedColumn() == NodeColumn)
            editTargetNodePopup.Popup(SceneRoot);
    }

    private void OnItemEdited()
    {
        var editedItem = GetEdited();
        if (GetEditedColumn() == MethodColumn)
        {
            var listenerData = (ListenerData)editedItem.GetMeta("listenerData");
            var selectedIndex = (int)editedItem.GetRange(MethodColumn);
            listenerData.MethodInfo = selectedIndex == 0 ? null : listenerData.Options[selectedIndex - 1];
		}
	}

    private void OnItemSelected()
    {
        editMethodPopup.Visible = false;
    }

	private TreeItem CreateNodeItem(Node node)
    {
        // Recursively traverse up inheritance chain until we
        // get the original Godot base type.
       
        TreeItem item = this.CreateItem();
        item.SetIcon(NodeColumn, this.GetIconRecursive(node));
        item.SetText(NodeColumn, node.Name);
        item.SetMeta("nodeData", new NodeData(node));

        foreach (EventInfo eventInfo in node.GetType().GetEvents())
            CreateEventItem(item, eventInfo);

        return item;
    }

    private TreeItem CreateEventItem(TreeItem parent, EventInfo eventInfo)
    {
        TreeItem item = this.CreateItem(parent);
        item.SetIcon(NodeColumn, GetIcon("Signals", "EditorIcons"));
        item.SetText(NodeColumn, $"{eventInfo.Name} ({string.Join(", ", eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType.Name))})");
        item.AddButton(2, GetIcon("Add", "EditorIcons"), (int)ButtonID.AddListener);
        item.SetMeta("eventData", new EventData(eventInfo));
        return item;
    }

    private TreeItem CreateListenerItem(TreeItem parent)
    {
        TreeItem item = this.CreateItem(parent);
        item.SetCellMode(NodeColumn, TreeItem.TreeCellMode.Custom);
        item.SetEditable(NodeColumn, true);
        item.SetText(NodeColumn, "-- Empty --");
        item.SetCellMode(MethodColumn, TreeItem.TreeCellMode.Range);
        item.SetEditable(MethodColumn, false);
        item.SetText(MethodColumn, "-- Empty --");
        item.AddButton(2, GetIcon("Remove", "EditorIcons"), (int)ButtonID.RemoveListener);
        item.SetMeta("listenerData", new ListenerData());
        return item;
	}
    
    private void OnButtonPressed(TreeItem item, int column, int id)
    {
        switch ((ButtonID) id)
        {
            case ButtonID.AddListener:
                CreateListenerItem(item);
                break;
            case ButtonID.RemoveListener:
                item.Free();
                break;
		}
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
