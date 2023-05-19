using Fractural.Plugin;
using Fractural.Utils;
using Godot;
using System;

#if TOOLS
namespace Tests
{
    public class TestInspectorPlugin : EditorInspectorPlugin
    {
        private EditorPlugin _plugin;

        public TestInspectorPlugin() { }
        public TestInspectorPlugin(EditorPlugin plugin)
        {
            _plugin = plugin;
        }

        public override bool CanHandle(Godot.Object @object)
        {
            return @object is CustomInspectors;
        }

        public override bool ParseProperty(Godot.Object @object, int type, string path, int hint, string hintText, int usage)
        {
            if (path.ToLower().StartsWith("value"))
                switch ((Variant.Type)type)
                {
                    case Variant.Type.Int:
                        AddPropertyEditor(path, new ValueEditorProperty(new IntegerValueProperty()));
                        return true;
                    case Variant.Type.Real:
                        AddPropertyEditor(path, new ValueEditorProperty(new FloatValueProperty()));
                        return true;
                    case Variant.Type.Bool:
                        AddPropertyEditor(path, new ValueEditorProperty(new BoolValueProperty()));
                        return true;
                    case Variant.Type.String:
                        AddPropertyEditor(path, new ValueEditorProperty(new StringValueProperty()));
                        return true;
                    case Variant.Type.NodePath:
                        AddPropertyEditor(path, new NodePathSelectEditorProperty(_plugin.GetEditorInterface().GetEditedSceneRoot()));
                        return true;
                }
            return false;
        }
    }
}
#endif
