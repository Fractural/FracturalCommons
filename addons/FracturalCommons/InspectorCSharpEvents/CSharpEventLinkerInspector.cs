using Godot;
using System;
using System.Reflection;

public class CSharpEventLinkerInspector : Node
{
	public EditorPlugin Plugin { get; set; }

	public void Init()
	{
		
	}

	public void Show()
	{
		var selectedNodes = Plugin.GetEditorInterface().GetSelection().GetSelectedNodes();
		//TODO
	}

	public void AttachEvent(Node node)
	{
		foreach (EventInfo eventInfo in node.GetType().GetEvents())
		{
			// TODO
			// eventInfo.Name;
		}
	}
}
