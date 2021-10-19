using Godot;
using System;
using System.Reflection;
using System.Linq;
using Fractural.Utils;
using System.Collections.Generic;
using Fractural.Plugin.AssetsRegistry;
using System.Text.RegularExpressions;

[Tool]
public class CSharpEventsInspector : Control
{
    public class NodeData : Godot.Reference
    {
        public NodeData() { }
        public NodeData(Node node)
        {
            Node = node;
        }

        public Node Node { get; set; }
    }

    public class EventData : Godot.Reference
    {
        public EventData() { }
        public EventData(NodeData nodeData, string eventName)
        {
            NodeData = nodeData;
            EventName = eventName;
        }

        public NodeData NodeData { get; set; }
        public string EventName { get; set; }
        public EventInfo EventInfo
        {
            get => EditorUtils.GetRealType(NodeData.Node).GetEvent(EventName);
            set => EventName = value.Name;
        }
    }

    public class ListenerData : Godot.Reference
    {
        public ListenerData() { }
        public ListenerData(EventData eventData)
        {
            EventData = eventData;
        }

        public EventData EventData { get; set; }
		public Node TargetNode { get; set; }
        public string TargetMethodName { get; set; }
        public string[] TargetMethodParameterTypes { get; set; }
        public MethodInfo MethodInfo
        {
            get => EditorUtils.GetRealType(TargetNode).GetMethod(TargetMethodName, TargetMethodParameterTypes.Select(typeString => Type.GetType(typeString)).ToArray());
            set
            {
                TargetMethodName = value.Name;
                TargetMethodParameterTypes = value.GetParameters().Select(x => x.ParameterType.Name).ToArray();
			}
        }
    }

    // Hacky class used to detect whenever Godot builds C#
    // When Godot builds a C# solution, all Godot.Objects
    // are reserialized. This means they use the default
    // constructor.
    public class BuildCanary : Godot.Object
    {
        public static bool HasBuilt { get; set; } = false;

        public BuildCanary()
        {
            HasBuilt = true;
        }

        public BuildCanary(int number)
        {

        }
    }

    public enum ButtonID
    {
        AddListener,
        RemoveListener,
        GotoNode,
    }

    public static int NodeColumn => 0;
    public static int MethodColumn => 1;

    public string EventLinkerScriptsFolder => "res://addons/FracturalCommons/InspectorCSharpEvents/EventLinkers/";
    public Node SceneRoot => plugin.GetTree().EditedSceneRoot;
    public Node EventLinker => SceneRoot?.GetNodeOrNull("EventLinker");

    [Export]
    private NodePath createEventLinkerContainerPath;
    [Export]
    private NodePath createEventLinkerButtonPath;
    [Export]
    private NodePath eventSearchBarPath;
    [Export]
    private NodePath nodeSearchBarPath;
    [Export]
    private NodePath refreshButtonPath;
    [Export]
    private NodePath followSelectionTogglePath;
    [Export]
    private NodePath treeNodePath;
    [Export]
    private NodePath editTargetNodePopupPath;
    [Export]
    private NodePath editTargetMethodPopupPath;

    private Control createEventLinkerContainer;
    private Button createEventLinkerButton;
    private LineEdit eventSearchBar;
    private LineEdit nodeSearchBar;
    private Button refreshButton;
    private Button followSelectionToggle;
    private Tree tree;
    private NodeSelectPopup editTargetNodePopup;
    private MethodSelectPopup editTargetMethodPopup;

    private EditorSelection selection;
    private EditorPlugin plugin;
    private IAssetsRegistry assetsRegistry;

    private BuildCanary buildCanary = new BuildCanary(0);

    public override void _Ready()
    {
        if (NodeUtils.IsInEditorSceneTab(this))
            return;
        if (plugin == null)
        {
            GD.PrintErr("Expected plugin to be set for CSharpEventsInspector!");
            return;
        }

        createEventLinkerContainer = GetNode<Control>(createEventLinkerContainerPath);
        createEventLinkerButton = GetNode<Button>(createEventLinkerButtonPath);
        eventSearchBar = GetNode<LineEdit>(eventSearchBarPath);
        nodeSearchBar = GetNode<LineEdit>(nodeSearchBarPath);
        refreshButton = GetNode<Button>(refreshButtonPath);
        followSelectionToggle = GetNode<Button>(followSelectionTogglePath);
        tree = GetNode<Tree>(treeNodePath);
        selection = plugin.GetEditorInterface().GetSelection();
        editTargetNodePopup = GetNode<NodeSelectPopup>(editTargetNodePopupPath);
        editTargetMethodPopup = GetNode<MethodSelectPopup>(editTargetMethodPopupPath);

        createEventLinkerButton.Icon = GetIcon("ScriptCreate", "EditorIcons");
        eventSearchBar.RightIcon = GetIcon("Search", "EditorIcons");
        nodeSearchBar.RightIcon = GetIcon("Search", "EditorIcons");
        followSelectionToggle.Icon = GetIcon("ToolSelect", "EditorIcons");
        refreshButton.Icon = GetIcon("Reload", "EditorIcons");

        createEventLinkerButton.Connect("pressed", this, nameof(CreateEventLinker));
        eventSearchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));
        nodeSearchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));
        refreshButton.Connect("pressed", this, nameof(OnRefreshButtonPressed));
        followSelectionToggle.Connect("pressed", this, nameof(OnFollowSelectionTogglePressed));
        plugin.Connect("scene_changed", this, nameof(OnEditorSceneChanged));
        selection.Connect("selection_changed", this, nameof(OnSelectionChanged));
        // Disabled temporarily since they are more trouble than they're worth
        //GetTree().Connect("node_added", this, nameof(OnNodeAdded));
        //GetTree().Connect("node_renamed", this, nameof(OnNodeRenamed));
        tree.Connect("custom_popup_edited", this, nameof(OnCustomPopupEdited));
        tree.Connect("button_pressed", this, nameof(OnButtonPressed));
        editTargetNodePopup.Connect(nameof(NodeSelectPopup.NodeSelected), this, nameof(OnTargetNodeSelected));
        editTargetMethodPopup.Connect(nameof(MethodSelectPopup.MethodSelected), this, nameof(OnMethodSelected));

        tree.HideRoot = true;
        tree.Columns = 2;
        tree.SetColumnExpand(NodeColumn, true);
        tree.SetColumnExpand(MethodColumn, true);

        createEventLinkerContainer.AddConstantOverride("margin_top", (int) (createEventLinkerContainer.GetConstant("margin_top") * assetsRegistry.Scale));
        createEventLinkerContainer.AddConstantOverride("margin_bottom", (int)(createEventLinkerContainer.GetConstant("margin_bottom") * assetsRegistry.Scale));
        createEventLinkerContainer.AddConstantOverride("margin_left", (int)(createEventLinkerContainer.GetConstant("margin_left") * assetsRegistry.Scale));
        createEventLinkerContainer.AddConstantOverride("margin_right", (int)(createEventLinkerContainer.GetConstant("margin_right") * assetsRegistry.Scale));
        editTargetNodePopup.RectSize *= assetsRegistry.Scale;
        editTargetMethodPopup.RectSize *= assetsRegistry.Scale;

        TryUpdateVisuals();
    }

	public void Init(EditorPlugin editorPlugin, IAssetsRegistry assetsRegistry)
    {
        this.assetsRegistry = assetsRegistry;
        this.plugin = editorPlugin;
    }

	public override void _Process(float delta)
	{
        if (BuildCanary.HasBuilt)
        {
            buildCanary = new BuildCanary(0);
            BuildCanary.HasBuilt = false;
            TryUpdateVisuals();
        }
	}

    public void CreateEventLinker()
    {
        if (SceneRoot == null)
        {
            GD.PrintErr("Expected SceneRoot to not be null!");
            return;
        }
        var eventLinker = EventLinker;
        if (eventLinker != null && eventLinker.GetScript() != null)
        {
            GD.PrintErr("EventLinker already exists!");
            return;
        }
        if (eventLinker == null)
        {
            eventLinker = new Node();
            eventLinker.Name = "EventLinker";
            SceneRoot.AddChild(eventLinker);
            eventLinker.Owner = SceneRoot;
        }
        File file = new File();
        var nextFreeName = GetNextFreeScriptName();
        file.Open(EventLinkerScriptsFolder + nextFreeName + ".cs", File.ModeFlags.Write);
        file.StoreString($@"using Godot;
using System;

public class {nextFreeName} : CSharpEventLinker
{{
}}");
        file.Close();
        eventLinker.SetScript(ResourceLoader.Load<CSharpScript>(EventLinkerScriptsFolder + nextFreeName + ".cs"));

        if (eventLinker.GetPositionInParent() != SceneRoot.GetChildCount() - 1)
            SceneRoot.MoveChild(eventLinker, SceneRoot.GetChildCount() - 1);
        createEventLinkerContainer.Visible = false;

        TryUpdateVisuals();
    }

	public void TryUpdateVisuals()
    {
        if (tree == null)
            return;

        tree.Clear();
        TreeItem root = tree.CreateItem();

        if (SceneRoot == null)
            return;
        
        if (EventLinker == null)
        {
            createEventLinkerContainer.Visible = true;
            return;
        }
        else
            createEventLinkerContainer.Visible = false;

        if (followSelectionToggle.Pressed && selection.GetSelectedNodes().Count > 0)
            TryCreateNodeItem((Node) selection.GetSelectedNodes()[0]);
        else
            CreateTreeRecursive(SceneRoot);
        
        LoadSavedListeners();
    }

    public void LoadSavedListeners()
    {
        File file = new File();
        file.Open(((CSharpScript)EventLinker.GetScript()).ResourcePath, File.ModeFlags.Read);
        var sourceCodeText = file.GetAsText();
        file.Close();
        var result = Regex.Matches(sourceCodeText, @"GetNode<.+>\(""(.+)""\)\.([a-zA-Z_0-9]+) \+= GetNode<.+>\(""(.+)""\)\.([a-zA-Z_0-9]+);");
        foreach (Match match in result)
        {
            var sourceNode = EventLinker.GetNodeOrNull(match.Groups[1].Value);
            if (sourceNode == null)
                continue;
            var targetNode = EventLinker.GetNodeOrNull(match.Groups[3].Value);
            if (targetNode == null)
                continue;
            var eventInfo = EditorUtils.GetRealType(sourceNode).GetEvent(match.Groups[2].Value);
            if (eventInfo == null)
                continue;
            var methodInfo = EditorUtils.GetRealType(targetNode).GetMethod(match.Groups[4].Value);
            if (methodInfo == null)
                continue;

            CreateListenerItemFromSave(sourceNode, targetNode, eventInfo, methodInfo);
        }
    }

    public void CreateListenerItemFromSave(Node sourceNode, Node targetNode, EventInfo eventInfo, MethodInfo methodInfo)
    {
        TreeItem sourceNodeTreeItem = tree.GetRoot().GetChildren();
        while (sourceNodeTreeItem != null && ((NodeData)sourceNodeTreeItem.GetMeta("nodeData")).Node != sourceNode)
            sourceNodeTreeItem = sourceNodeTreeItem.GetNext();
        if (sourceNodeTreeItem == null)
            return;

        TreeItem eventTreeItem = sourceNodeTreeItem.GetChildren();
        while (eventTreeItem != null && ((EventData)eventTreeItem.GetMeta("eventData")).EventInfo != eventInfo)
            eventTreeItem = eventTreeItem.GetNext();
        if (eventTreeItem == null)
            return;

        TreeItem listenerItem = CreateListenerItem(eventTreeItem);
        var listenerData = (ListenerData) listenerItem.GetMeta("listenerData");
        listenerData.MethodInfo = methodInfo;
        listenerData.TargetNode = targetNode;

        listenerItem.SetEditable(MethodColumn, true);
        listenerItem.SetIcon(NodeColumn, this.GetIconRecursive(targetNode));
        listenerItem.SetText(NodeColumn, targetNode.Name);
        listenerItem.SetText(MethodColumn, listenerData.TargetMethodName != "" ? listenerData.TargetMethodName : "-- Empty --");
    }

    public void TryUpdateEventLinkerScript()
    {
        if (EventLinker == null)
            return;

        File file = new File();
        file.Open(((CSharpScript)EventLinker.GetScript()).ResourcePath, File.ModeFlags.Write);
        file.StoreString(
            GenerateEventLinkerSourceText(EventLinker, ((CSharpScript) EventLinker.GetScript()).ResourcePath.GetFileName())
        );
        file.Close();
	}

    public string GenerateEventLinkerSourceText(Node eventLinker, string eventLinkerScriptName)
    {
        string lines = "";

        var nodeItem = tree.GetRoot().GetChildren();
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
                    if (listenerData.TargetNode != null && listenerData.TargetMethodName != "")
                    {
                        lines += $"\t\tGetNode<{EditorUtils.GetRealType(nodeData.Node).FullName}>(\"{eventLinker.GetPathTo(nodeData.Node)}\").{eventData.EventInfo.Name} += GetNode<{EditorUtils.GetRealType(listenerData.TargetNode).FullName}>(\"{eventLinker.GetPathTo(listenerData.TargetNode)}\").{listenerData.TargetMethodName};\n";
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

public class {eventLinkerScriptName} : CSharpEventLinker
{{
    public override void _EnterTree()
    {{
{lines}    }}
}}
";
        return sourceText;
    }

    private void OnSearchBarTextChanged(string newText)
    {
        TryUpdateVisuals();
    }

    private void OnFollowSelectionTogglePressed()
    {
        TryUpdateVisuals();
    }

    private void OnSelectionChanged()
    {
        if (followSelectionToggle.Pressed)
            TryUpdateVisuals();
    }

    private void OnRefreshButtonPressed()
    {
        TryUpdateVisuals();
        TryUpdateEventLinkerScript();
    }

    private void OnNodeAdded(Node node)
    {
        if (!NodeUtils.IsInEditorSceneTab(node))
            return;

        TryUpdateVisuals();
    }

    private void OnNodeRenamed(Node node)
    {
        if (!NodeUtils.IsInEditorSceneTab(node))
            return;

        TryUpdateVisuals();
    }

    private void OnEditorSceneChanged(Node newScene)
    {
        TryUpdateVisuals();
    }

    private string GetNextFreeScriptName()
    {
        File file = new File();
        int num = 0;
        while (file.FileExists(EventLinkerScriptsFolder + $"EventLinker{num}.cs"))
            num++;
        return $"EventLinker{num}";
    }

    private void CreateTreeRecursive(Node node)
	{
        if (node == null)
            return;
        
        TryCreateNodeItem(node);
        
        foreach (Node child in node.GetChildren())
            CreateTreeRecursive(child);
	}

    private void OnTargetNodeSelected(Node node)
    {
        if (EventLinker == null)
        {
            TryUpdateVisuals();
            return;
		}
        
        var listenerItem = tree.GetEdited();
        var listenerData = ((ListenerData)listenerItem.GetMeta("listenerData"));

        listenerData.TargetNode = node;

        listenerItem.SetEditable(MethodColumn, true);
        listenerItem.SetIcon(NodeColumn, this.GetIconRecursive(node));
        listenerItem.SetText(NodeColumn, node.Name);
    }

    private void OnMethodSelected(MethodSelectPopup.MethodItemData methodItemData)
    {
        if (EventLinker == null)
        {
            TryUpdateVisuals();
            return;
        }

        var listenerItem = tree.GetEdited();
        var listenerData = ((ListenerData)listenerItem.GetMeta("listenerData"));

        listenerData.TargetMethodName = methodItemData.MethodName;
        listenerData.TargetMethodParameterTypes = methodItemData.MethodParamterTypes;
        listenerItem.SetText(MethodColumn, listenerData.TargetMethodName != "" ? listenerData.TargetMethodName : "-- Empty --");

        TryUpdateEventLinkerScript();
    }

    private void OnCustomPopupEdited(bool arrowClicked)
	{
        if (EventLinker == null)
        {
            CallDeferred(nameof(TryUpdateVisuals));
            return;
        }

        if (tree.GetEditedColumn() == NodeColumn)
            editTargetNodePopup.Popup(SceneRoot);
        else if (tree.GetEditedColumn() == MethodColumn)
        {
            var listenerData = (ListenerData) tree.GetEdited().GetMeta("listenerData");
            var eventData = (EventData) tree.GetEdited().GetParent().GetMeta("eventData");

            editTargetMethodPopup.Popup(listenerData.TargetNode, eventData.EventInfo);
        }
    }

	private TreeItem TryCreateNodeItem(Node node)
    {
        // Recursively traverse up inheritance chain until we
        // get the original Godot base type.
        var events = EditorUtils.GetRealType(node).GetEvents();

        if (events.Length == 0)
            return null;
        if (nodeSearchBar.Text != "" && node.Name.ToLower().Find(nodeSearchBar.Text.ToLower()) < 0)
            return null;

        TreeItem item = tree.CreateItem();
        item.SetIcon(NodeColumn, this.GetIconRecursive(node));
        item.SetText(NodeColumn, node.Name);
        item.AddButton(NodeColumn, GetIcon("ArrowRight", "EditorIcons"), (int)ButtonID.GotoNode);
        item.SetMeta("nodeData", new NodeData(node));

        foreach (EventInfo eventInfo in events)
            CreateEventItem(item, eventInfo);

        return item;
    }

    private TreeItem CreateEventItem(TreeItem parent, EventInfo eventInfo)
    {
        if (eventSearchBar.Text != "" && eventInfo.Name.ToLower().Find(eventSearchBar.Text.ToLower()) < 0)
            return null;

        TreeItem item = tree.CreateItem(parent);
        item.SetIcon(NodeColumn, GetIcon("Signals", "EditorIcons"));
        item.SetText(NodeColumn, $"{eventInfo.Name}");
        item.AddButton(MethodColumn, GetIcon("Add", "EditorIcons"), (int)ButtonID.AddListener);
        item.SetMeta("eventData", new EventData((NodeData)parent.GetMeta("nodeData"), eventInfo.Name));
        return item;
    }

    private TreeItem CreateListenerItem(TreeItem parent)
    {
        TreeItem item = tree.CreateItem(parent);
        item.SetCellMode(NodeColumn, TreeItem.TreeCellMode.Custom);
        item.SetEditable(NodeColumn, true);
        item.SetText(NodeColumn, "-- Empty --");
        item.AddButton(NodeColumn, GetIcon("ArrowRight", "EditorIcons"), (int)ButtonID.GotoNode);
        item.SetCellMode(MethodColumn, TreeItem.TreeCellMode.Custom);
        item.SetEditable(MethodColumn, false);
        item.SetText(MethodColumn, "-- Empty --");
        item.AddButton(MethodColumn, GetIcon("Remove", "EditorIcons"), (int)ButtonID.RemoveListener);
        item.SetMeta("listenerData", new ListenerData((EventData) parent.GetMeta("eventData")));
        return item;
	}
    
    private void OnButtonPressed(TreeItem item, int column, int id)
    {
        if (EventLinker == null)
        {
            TryUpdateVisuals();
            return;
        }

        switch ((ButtonID) id)
        {
            case ButtonID.AddListener:
                CreateListenerItem(item);
                break;
            case ButtonID.RemoveListener:
                item.Free();
                TryUpdateEventLinkerScript();
                break;
            case ButtonID.GotoNode:
                Node node = null;
                if (item.HasMeta("nodeData"))
                    node = ((NodeData)item.GetMeta("nodeData")).Node;
                else if (item.HasMeta("listenerData"))
                    node = ((ListenerData)item.GetMeta("listenerData")).TargetNode;
                var selection = plugin.GetEditorInterface().GetSelection();
                plugin.GetEditorInterface().EditNode(node);
                break;
		}
	}
}
