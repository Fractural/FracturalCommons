using Fractural.Plugin;
using Godot;
using System.Collections.Generic;
using GDC = Godot.Collections;
using System.Linq;
using Fractural.Utils;

#if TOOLS
namespace Fractural.Commons
{
    public class MainPlugin : SubPlugin
    {
        public static class Settings
        {
            public static MainPlugin Plugin { get; private set; }

            public static bool GenerateVersionPreprocessorDefines => Plugin.GetSetting<bool>(nameof(GenerateCSharpScriptsTable));
            public static bool GenerateCSharpScriptsTable => Plugin.GetSetting<bool>(nameof(GenerateVersionPreprocessorDefines));

            public static void Init(MainPlugin plugin)
            {
                Plugin = plugin;
                Plugin.AddSetting(nameof(GenerateCSharpScriptsTable), Variant.Type.Bool, false);
                Plugin.AddSetting(nameof(GenerateVersionPreprocessorDefines), Variant.Type.Bool, false);
            }
        }

        public override string PluginName => "Main";

        public override void Load()
        {
            Settings.Init(this);

            if (Settings.GenerateCSharpScriptsTable)
                CSharpScriptUtils.GenerateCSharpScriptsTable();
            if (Settings.GenerateVersionPreprocessorDefines)
                EngineUtils.GenerateVersionPreprocessorDefines();
        }
    }
}
#endif