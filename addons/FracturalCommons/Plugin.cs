using Godot;
using System;
using Fractural.Plugin;
using Fractural.Utils;
using System.Collections.Generic;
using Fractural.InspectorCSharpEvents;

#if TOOLS
namespace Fractural.Commons
{
	[Tool]
    public class Plugin : EditorPlugin
    {
		public List<PluginModule> PluginModules { get; set; } = new List<PluginModule>();
		
		public override void _EnterTree()
		{
			EngineUtils.UpdateVersionPreprocessorDefines();
			LoadPluginModules();
		}

		public override void _ExitTree()
		{
			UnloadPluginModules();
		}

		public void LoadPluginModules()
		{
			PluginModules.Add(new InspectorCSharpEventsPluginModule(this));
		}

		public void UnloadPluginModules()
		{
			foreach (PluginModule module in PluginModules)
				module.Unload();
		}
	}
}
#endif