using Fractural.Plugin;
using Fractural.Utils;
using Godot;

#if TOOLS
namespace Fractural.Commons
{
    [Tool]
    public class Plugin : ExtendedPlugin
    {
        public static class Settings
        {
            public const string GenerateVersionPreprocessorDefines = "Generate Version Preprocessor Defines";
            public const string GenerateCSharpScriptsTable = "Generate CSharp Scripts Table";
        }

        public override string PluginName => "FracturalCommons";

        protected override void Load()
        {
            AddSetting(Settings.GenerateCSharpScriptsTable, Variant.Type.Bool, false);
            if (GetSetting<bool>(Settings.GenerateCSharpScriptsTable))
                CSharpScriptUtils.GenerateCSharpScriptsTable();

            AddSetting(Settings.GenerateVersionPreprocessorDefines, Variant.Type.Bool, false);
            if (GetSetting<bool>(Settings.GenerateVersionPreprocessorDefines))
                EngineUtils.GenerateVersionPreprocessorDefines();
        }
    }
}
#endif