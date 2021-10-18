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
    }

    public enum ButtonID
    {
        AddListener,
        RemoveListener,
        GotoNode,
    }

    public static int NodeColumn => 0;
    public static int MethodColumn => 1;

    [Signal]
    public delegate void GotoNode(Node node);
    [Signal]
    public delegate void EventsChanged();

    public Node SceneRoot => GetNode(testTreeRootPath);
    public Node EventLinker
    {
        get
        {
            CSharpEventLinker eventLinker;
            if (SceneRoot.HasNode("EventLinker"))
                eventLinker = SceneRoot.GetNode<CSharpEventLinker>("EventLinker");
            else
            {
                eventLinker = CSharpUtils.InstantiateCSharpNode<CSharpEventLinker>();
                eventLinker.Name = "EventLinker";
                SceneRoot.AddChild(eventLinker);
            }
            if (eventLinker.GetPositionInParent() != SceneRoot.GetChildCount() - 1)
                SceneRoot.MoveChild(eventLinker, SceneRoot.GetChildCount() - 1);
            return eventLinker;
        }
    }

    [Export]
    private NodePath testTreeRootPath;
    [Export]
    private NodePath editTargetNodePopupPath;
    [Export]
    private NodePath editTargetMethodPopupPath;

    private NodeSelectPopup editTargetNodePopup;
    private MethodSelectPopup editTargetMethodPopup;

    public override void _Ready()
    {
        editTargetNodePopup = GetNode<NodeSelectPopup>(editTargetNodePopupPath);
        editTargetMethodPopup = GetNode<MethodSelectPopup>(editTargetMethodPopupPath);

        Connect(nameof(EventsChanged), this, nameof(TempTestEventChanged));
        Connect("custom_popup_edited", this, nameof(OnCustomPopupEdited));
        Connect("button_pressed", this, nameof(OnButtonPressed));
        editTargetNodePopup.Connect(nameof(NodeSelectPopup.NodeSelected), this, nameof(OnTargetNodeSelected));
        editTargetMethodPopup.Connect(nameof(MethodSelectPopup.MethodSelected), this, nameof(OnMethodSelected));

        this.HideRoot = true;
        this.Columns = 2;
        this.SetColumnExpand(NodeColumn, true);
        this.SetColumnExpand(MethodColumn, true);

        CreateTree(SceneRoot);
    }

    public void TempTestEventChanged()
    {
        GD.Print("New Source:");
        GD.Print(GenerateEventLinkerSourceText(EventLinker));
    }

	public void CreateTree(Node node)
    {
        Clear();
        TreeItem root = this.CreateItem();
        CreateTreeRecursive(node);
	}

    public string GenerateEventLinkerSourceText(Node eventLinker)
    {
        string lines = "";

        var nodeItem = this.GetRoot().GetChildren();
        while (nodeItem != null)
        {
            var nodeData = (NodeData)nodeItem.GetMeta("nodeData");
            var eventItem = nodeItem.GetChildren();
            while (eventItem != null)
            {
                var eventData = (EventData)eventItem.GetMeta("eventData");
                var listenerItem = eventItem.GetChildren();
                while (listenerItem != null)
                {
                    var listenerData = (ListenerData) listenerItem.GetMeta("listenerData");
                    if (listenerData.TargetNode != null && listenerData.MethodInfo != null)
                    {
                        lines += $"\t\tGetNode<{nodeData.Node.GetClass()}>(\"{eventLinker.GetPathTo(nodeData.Node)}\").{eventData.EventInfo.Name} += GetNode<{listenerData.TargetNode.GetClass()}>(\"{eventLinker.GetPathTo(listenerData.TargetNode)}\").{listenerData.MethodInfo.Name};\n";
                    }
                    listenerItem = listenerItem.GetNext();
				}

                eventItem = eventItem.GetNext();
            }

            nodeItem = nodeItem.GetNext();
        }

        string sourceText = 
$@"using Godot;
using System;

public class EventLinker : Node
{{
    public override void _EnterTree()
    {{
{lines}    }}
}}
";
        return sourceText;
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
        var listenerData = ((ListenerData)listenerItem.GetMeta("listenerData"));

        listenerData.TargetNode = node;

        listenerItem.SetEditable(MethodColumn, true);
        listenerItem.SetIcon(NodeColumn, this.GetIconRecursive(node));
        listenerItem.SetText(NodeColumn, node.Name);

        EmitSignal(nameof(EventsChanged));
    }

    private void OnMethodSelected(MethodSelectPopup.MethodItemData methodItemData)
    {
        var listenerItem = GetEdited();
        var listenerData = ((ListenerData)listenerItem.GetMeta("listenerData"));

        listenerData.MethodInfo = methodItemData.MethodInfo;
        listenerItem.SetText(MethodColumn, listenerData.MethodInfo != null ? listenerData.MethodInfo.Name : "-- Empty --");

        EmitSignal(nameof(EventsChanged));
    }

    private void OnCustomPopupEdited(bool arrowClicked)
	{
        if (GetEditedColumn() == NodeColumn)
            editTargetNodePopup.Popup(SceneRoot);
        else if (GetEditedColumn() == MethodColumn)
        {
            var listenerData = (ListenerData) GetEdited().GetMeta("listenerData");
            var eventData = (EventData)GetEdited().GetParent().GetMeta("eventData");
            editTargetMethodPopup.Popup(listenerData.TargetNode, eventData.EventInfo);
        }
    }

	private TreeItem CreateNodeItem(Node node)
    {
        // Recursively traverse up inheritance chain until we
        // get the original Godot base type.
       
        TreeItem item = this.CreateItem();
        item.SetIcon(NodeColumn, this.GetIconRecursive(node));
        item.SetText(NodeColumn, node.Name);
        item.AddButton(NodeColumn, GetIcon("ArrowRight", "EditorIcons"), (int)ButtonID.GotoNode);
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
        item.AddButton(MethodColumn, GetIcon("Add", "EditorIcons"), (int)ButtonID.AddListener);
        item.SetMeta("eventData", new EventData(eventInfo));
        return item;
    }

    private TreeItem CreateListenerItem(TreeItem parent)
    {
        TreeItem item = this.CreateItem(parent);
        item.SetCellMode(NodeColumn, TreeItem.TreeCellMode.Custom);
        item.SetEditable(NodeColumn, true);
        item.SetText(NodeColumn, "-- Empty --");
        item.AddButton(NodeColumn, GetIcon("ArrowRight", "EditorIcons"), (int)ButtonID.GotoNode);
        item.SetCellMode(MethodColumn, TreeItem.TreeCellMode.Custom);
        item.SetEditable(MethodColumn, false);
        item.SetText(MethodColumn, "-- Empty --");
        item.AddButton(MethodColumn, GetIcon("Remove", "EditorIcons"), (int)ButtonID.RemoveListener);
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
            case ButtonID.GotoNode:
                Node node = null;
                if (item.HasMeta("nodeData"))
                    node = ((NodeData)item.GetMeta("nodeData")).Node;
                else if (item.HasMeta("listenerData"))
                    node = ((ListenerData)item.GetMeta("listenerData")).TargetNode;
                EmitSignal(nameof(GotoNode), node);
                break;
		}
	}
}
