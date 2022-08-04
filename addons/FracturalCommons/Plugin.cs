using Godot;
using Fractural.Plugin;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Commons
{
	[Tool]
	public class Plugin : ExtendedPlugin
	{
		public override void _EnterTree()
		{
			base._EnterTree();

			EngineUtils.UpdateVersionPreprocessorDefines();
			GD.PushWarning("Loaded FracturalCommons");
		}
	}
}
#endif