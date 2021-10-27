using System.Diagnostics.Tracing;
using Godot;
using System;
using Fractural.Plugin.AssetsRegistry;
using Fractural.Utils;
using Fractural.Plugin;

#if TOOLS
namespace Fractural.InspectorCSharpEvents
{
	public class InspectorCSharpEventsPluginModule : PluginModule
	{
		// TODO: Implement a dedicated C# Plugin window docker class.
		public EditorPlugin.DockSlot DockSlot = EditorPlugin.DockSlot.RightBr;
		public CSharpEventsInspector EventsInspector { get; set; }

		public InspectorCSharpEventsPluginModule(EditorPlugin plugin) : base(plugin) { }

		public override void Load()
		{
			var inspectorWindowPrefab = ResourceLoader.Load<PackedScene>("res://addons/FracturalCommons/InspectorCSharpEvents/CSharpEventLinkerInspector.tscn");
			EventsInspector = inspectorWindowPrefab.Instance<CSharpEventsInspector>();
			EventsInspector.Init(Plugin, AssetsRegistry);
			Plugin.AddControlToDock(DockSlot, EventsInspector);
		}

		public override void Unload()
		{
			Plugin.RemoveControlFromDocks(EventsInspector);
			EventsInspector.QueueFree();
		}

		private void OnGotoNode(Node node)
		{
			
		}
	}
}
#endif