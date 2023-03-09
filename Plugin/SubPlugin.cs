using Godot;
using Godot.Collections;

#if TOOLS
namespace Fractural.Plugin
{
    public abstract class SubPlugin : Godot.Reference
    {
        public ExtendedPlugin Plugin { get; private set; }
        public virtual string PluginName => "";

        public void Init(ExtendedPlugin plugin)
        {
            Plugin = plugin;
            Print($"Loaded successfully");
        }

        public void Print(string text, PrintMode mode = PrintMode.Text)
        {
            text = $"{PluginName}: {text}";
            Plugin.Print(text, mode);
        }

        public virtual void _Process(float delta) { }
        public virtual void _PhysicsProcess(float delta) { }
        public virtual void ApplyChanges() { }
        public virtual bool Build() => true;
        public virtual void Clear() { }
        public virtual void DisablePlugin() { }
        public virtual void Edit(Object @object) { }
        public virtual void EnablePlugin() { }
        public virtual void ForwardCanvasDrawOverViewport(Control overlay) { }
        public virtual void ForwardCanvasForceDrawOverViewport(Control overlay) { }
        public virtual bool ForwardCanvasGuiInput(InputEvent @event) => false;
        public virtual void ForwardSpatialDrawOverViewport(Control overlay) { }
        public virtual void ForwardSpatialForceDrawOverViewport(Control overlay) { }
        public virtual bool ForwardSpatialGuiInput(Camera camera, InputEvent @event) => false;
        public virtual string[] GetBreakpoints() => null;
        public virtual Dictionary GetState() => null;
        public virtual void SetState(Dictionary state) { }
        public virtual void GetWindowLayout(ConfigFile layout) { }
        public virtual void SetWindowLayout(ConfigFile layout) { }
        public virtual bool Handles(Object @object) => false;
        public virtual void MakeVisible(bool visible) { }
        public virtual void SaveExternalData() { }
        public virtual void Load() { }
        public virtual void Unload() { }

        public object GetSetting(string title)
        {
            return Plugin.GetSetting($"{PluginName}/{title}");
        }

        public T GetSetting<T>(string title)
        {
            return (T)GetSetting(title);
        }

        public void AddSetting(string title, Variant.Type type, object value, PropertyHint hint = PropertyHint.None, string hintString = "")
        {
            title = SettingPath(title);
            Plugin.AddSetting(title, type, value, hint, hintString);
        }

        private string SettingPath(string title) => $"{PluginName}/{title}";
    }
}
#endif