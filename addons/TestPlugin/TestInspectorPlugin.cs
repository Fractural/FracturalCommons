using Fractural.Plugin;
using Godot;

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
            GD.Print("processing: ", path, " type: ", type);
            switch ((Variant.Type)type)
            {
                case Variant.Type.Int:
                    AddPropertyEditor(path, new ValueEditorProperty(new IntegerValueProperty()));
                    return false;
                case Variant.Type.Real:
                    AddPropertyEditor(path, new ValueEditorProperty(new FloatValueProperty()));
                    return false;
                case Variant.Type.Bool:
                    AddPropertyEditor(path, new ValueEditorProperty(new BoolValueProperty()));
                    return false;
                case Variant.Type.String:
                    AddPropertyEditor(path, new ValueEditorProperty(new StringValueProperty()));
                    return false;
                case Variant.Type.NodePath:
                    AddPropertyEditor(path, new NodePathSelectEditorProperty(_plugin.GetEditorInterface().GetEditedSceneRoot()));
                    return false;
            }
            return true;
        }
    }
}
#endif
