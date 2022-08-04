using Godot;
using Array = Godot.Collections.Array;
using System;
using Fractural.Plugin;
using Fractural.Utils;
using System.Collections.Generic;
using Fractural.InspectorCSharpEvents;

#if TOOLS
namespace Fractural.Commons
{
	[Tool]
	public class Plugin : ModularPlugin
	{
		public override Array PluginModules { get; set; } = new Array();
		public override List<Control> ManagedControls { get; set; } = new List<Control>();
		public override List<ManagedControlType> ManagedControlsType { get; set; } = new List<ManagedControlType>();
		public override List<object> ManagedControlsData { get; set; } = new List<object>();

		public override void _EnterTree()
		{
			base._EnterTree();

			GD.PushWarning("Loaded FracturalCommons.");
		}

		public override void LoadPluginModules()
		{
			LoadModule(new InspectorCSharpEventsPluginModule());
		}
	}
}
#endif