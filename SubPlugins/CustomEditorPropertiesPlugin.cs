using Fractural.Plugin;
using Fractural.Utils;
using Godot;
using System;
using System.Linq;

#if TOOLS
namespace Fractural.Commons
{
    public class CustomEditorPropertiesPlugin : SubPlugin
    {
        public override string PluginName => "Custom Editor Properties";

        public override void Load()
        {
            Plugin.AddManagedInspectorPlugin(new CustomEditorPropertiesInspectorPlugin(Plugin));
        }
    }

    public class CustomEditorPropertiesInspectorPlugin : EditorInspectorPlugin
    {
        private EditorPlugin _plugin;

        public CustomEditorPropertiesInspectorPlugin() { }
        public CustomEditorPropertiesInspectorPlugin(EditorPlugin plugin)
        {
            _plugin = plugin;
        }

        public override bool CanHandle(Godot.Object @object)
        {
            return true;
        }

        public override bool ParseProperty(Godot.Object @object, int type, string path, int hint, string hintText, int usage)
        {
            var hintArgs = hintText.Split(",");
            if (!TryParseDictionary(@object, type, path, hint, hintText, usage, hintArgs))
                return true;
            return false;
        }

        private bool TryParseDictionary(Godot.Object @object, int type, string path, int hint, string hintText, int usage, string[] hintArgs)
        {
            if (type == (int)Variant.Type.Dictionary && hintArgs.Contains(HintString.TypedDictionary))
            {
                var args = hintArgs.After(HintString.TypedDictionary, 1).Split(":");
                if (args.Length != 2)
                {
                    GD.PushError($"{nameof(CustomEditorPropertiesInspectorPlugin)}: Expected {nameof(HintString.TypedDictionary)} to have Key:Value as second hint string argument.");
                    return true;
                }
                string key = args[0].Trim();
                string value = args[1].Trim();
                Type keyType = ReflectionUtils.FindTypeName(key);
                if (keyType == null)
                {
                    GD.PushError($"{nameof(CustomEditorPropertiesInspectorPlugin)}: Could not find key type \"{key}\" for {nameof(HintString.TypedDictionary)}.");
                    return true;
                }
                Type valueType = ReflectionUtils.FindTypeName(value);
                if (valueType == null)
                {
                    GD.PushError($"{nameof(CustomEditorPropertiesInspectorPlugin)}: Could not find value type \"{value}\" for {nameof(HintString.TypedDictionary)}.");
                    return true;
                }
                AddPropertyEditor(path, new ValueEditorProperty(new DictionaryValueProperty(keyType, valueType, _plugin.GetEditorInterface().GetEditedSceneRoot(), @object as Node)));
                return false;
            }
            return true;
        }
    }

}
#endif