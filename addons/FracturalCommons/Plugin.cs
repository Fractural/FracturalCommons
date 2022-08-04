using Godot;
using Fractural.Plugin;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Commons
{
	[Tool]
	public class Plugin : ExtendedPlugin
	{
		public override string PluginName => "FracturalCommons";

		protected override void Load()
		{
			EngineUtils.UpdateVersionPreprocessorDefines();
		}
	}
}
#endif