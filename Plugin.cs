using Fractural.Plugin;
using Fractural.Utils;
using Godot;

#if TOOLS
namespace Fractural.Commons
{
    [Tool]
    public class Plugin : ExtendedPlugin
    {
        public override string PluginName => "FracturalCommons";

        protected override void Load()
        {
            AddSubPlugin(new MainPlugin());
            AddSubPlugin(new CSharpResourceRegistryPlugin());
        }
    }
}
#endif