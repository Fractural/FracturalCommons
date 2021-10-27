using System.Diagnostics.Tracing;
using Godot;
using System;
using Fractural.Plugin.AssetsRegistry;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Plugin
{
	public abstract class PluginModule
	{
		public EditorPlugin Plugin { get; set; }
		public IAssetsRegistry AssetsRegistry { get; set; }

		public PluginModule(EditorPlugin plugin)
		{
			Plugin = plugin;
			AssetsRegistry = new EditorAssetsRegistry(plugin);
			Load();
		}

		public virtual void Load()
		{
			
		}

		public virtual void Unload()
		{
		
		}
	}
}
#endif