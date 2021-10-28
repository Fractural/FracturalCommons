using Godot;
using System;
using Fractural.Plugin.AssetsRegistry;

#if TOOLS
namespace Fractural.Plugin
{
	public abstract class PluginModule : Godot.Reference
	{
		public ModularPlugin Plugin { get; set; }
		public IAssetsRegistry AssetsRegistry { get; set; }

		public void Init(ModularPlugin plugin)
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