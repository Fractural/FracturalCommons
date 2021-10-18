using Godot;
using System;
using System.Reflection;
using System.Linq;
using Fractural.Utils;

public class EventLinkerTree : Tree
{
    public enum ButtonID
    {
        AddListener,
        RemoveListener,
	}

    public static int NodeColumn => 0;
    public static int MethodColumn => 1;

    [Export]
    private NodePath testTreeRootPath;
    [Export]
    private NodePath editMethodPopupPath;
    [Export]
    private NodePath editTargetNodePopupPath;

    private EditMethodPopup editMethodPopup;
    private EditTargetNodePopup editTargetNodePopup;
    
    public override void _Ready()
    {
        editMethodPopup = GetNode<EditMethodPopup>(editMethodPopupPath);
        editTargetNodePopup = GetNode<EditTargetNodePopup>(editTargetNodePopupPath);

        Connect("custom_popup_edited", this, nameof(OnCustomPopupEdited));
        Connect("button_pressed", this, nameof(OnButtonPressed));

        this.HideRoot = true;
        this.Columns = 3;
        this.SetColumnExpand(0, true);
        this.SetColumnExpand(1, true);
        this.SetColumnExpand(2, false);
        this.SetColumnMinWidth(2, 28);

        CreateTree(GetNode(testTreeRootPath));
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

	private void OnCustomPopupEdited()
	{
        var editedItem = GetEdited();
        if (GetEditedColumn() == MethodColumn)
            editMethodPopup.Popup(GetCustomPopupRect(), editedItem);
        else if (GetEditedColumn() == NodeColumn)
            editTargetNodePopup.Popup(editedItem);
    }

    private void OnItemSelected()
    {
        editMethodPopup.Visible = false;
    }

	public TreeItem CreateNodeItem(Node node)
    {
        // Recursively traverse up inheritance chain until we
        // get the original Godot base type.
       
        TreeItem item = this.CreateItem();
        item.SetIcon(0, this.GetIconRecursive(node));
        item.SetText(0, node.Name);

        foreach (EventInfo eventInfo in node.GetType().GetEvents())
            CreateEventItem(item, eventInfo);

        return item;
    }

    public TreeItem CreateEventItem(TreeItem parent, EventInfo eventInfo)
    {
        TreeItem item = this.CreateItem(parent);
        item.SetIcon(0, GetIcon("Signals", "EditorIcons"));
        item.SetText(0, $"{eventInfo.Name} ({string.Join(", ", eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType.Name))})");
        item.AddButton(2, GetIcon("Add", "EditorIcons"), (int)ButtonID.AddListener);
        return item;
    }

    public TreeItem CreateListenerItem(TreeItem parent)
    {
        TreeItem item = this.CreateItem(parent);
        item.SetText(0, "-- Empty --");
        item.SetCellMode(0, TreeItem.TreeCellMode.Custom);
        item.SetEditable(0, true);
        item.SetText(1, "-- Empty --");
        item.SetCellMode(1, TreeItem.TreeCellMode.Custom);
        item.SetEditable(1, true);
        item.AddButton(2, GetIcon("Remove", "EditorIcons"), (int)ButtonID.RemoveListener);
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
}
