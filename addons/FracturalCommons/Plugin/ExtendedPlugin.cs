using Fractural.Plugin.AssetsRegistry;
using Fractural.Utils;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using GDC = Godot.Collections;

#if TOOLS
namespace Fractural.Plugin
{
    public enum PrintMode
    {
        Text,
        Warning,
        Error,
    }

    [Tool]
    public abstract class ExtendedPlugin : EditorPlugin, ISerializationListener
    {
        public IAssetsRegistry AssetsRegistry { get; protected set; } = new DefaultAssetsRegistry();
        public abstract string PluginName { get; }
        public virtual Texture PluginIcon { get; }

        public override string GetPluginName() => PluginName;
        public override Texture GetPluginIcon() => PluginIcon;

        public override void _EnterTree()
        {
            try
            {
                Load();
                Print("Loaded succesfully");
            }
            catch (Exception e)
            {
                Print("Error loading " + e);
            }
        }

        public override void _ExitTree()
        {
            try
            {
                Unload();
                UnloadManagedObjects();
                UnloadSubPlugins();
                Print("Unloaded succesfully\n");
            }
            catch (Exception e)
            {
                Print("Error unloading " + e);
            }
        }

        protected virtual void Load() { }

        protected virtual void Unload() { }

        #region Managed Objects
        public enum ManagedControlType
        {
            Control,
            Dock,
            BottomPanel,
            Container,
        }

        public class ManagedControl : Godot.Reference
        {
            public Control control;
            public ManagedControlType type;
            public Godot.Collections.Dictionary data;

            public ManagedControl() { }
            public ManagedControl(Control control, ManagedControlType type, Dictionary data = null)
            {
                this.control = control;
                this.type = type;
                this.data = data;
            }
        }

        public GDC.Array<ManagedControl> ManagedControls { get; set; } = new GDC.Array<ManagedControl>();

        public GDC.Array<EditorInspectorPlugin> ManagedInspectorPlugins { get; } = new GDC.Array<EditorInspectorPlugin>();
        public GDC.Array<EditorImportPlugin> ManagedImportPlugins { get; } = new GDC.Array<EditorImportPlugin>();
        public GDC.Array<EditorExportPlugin> ManagedExportPlugins { get; } = new GDC.Array<EditorExportPlugin>();
        public GDC.Array<EditorSceneImporter> ManagedSceneImportPlugins { get; } = new GDC.Array<EditorSceneImporter>();
        public GDC.Array<EditorSpatialGizmoPlugin> ManagedSpatialGizmoPlugins { get; } = new GDC.Array<EditorSpatialGizmoPlugin>();

        private void UnloadManagedObjects()
        {
            for (int i = ManagedControls.Count - 1; i >= 0; i--) DestroyManagedControl(i);
            foreach (var plugin in ManagedInspectorPlugins) RemoveInspectorPlugin(plugin);
            foreach (var plugin in ManagedImportPlugins) RemoveImportPlugin(plugin);
            foreach (var plugin in ManagedExportPlugins) RemoveExportPlugin(plugin);
            foreach (var plugin in ManagedSceneImportPlugins) RemoveSceneImportPlugin(plugin);
            foreach (var plugin in ManagedSpatialGizmoPlugins) RemoveSpatialGizmoPlugin(plugin);
            ManagedInspectorPlugins.Clear();
            ManagedImportPlugins.Clear();
            ManagedExportPlugins.Clear();
            ManagedSceneImportPlugins.Clear();
            ManagedSpatialGizmoPlugins.Clear();
        }

        public void AddManagedInspectorPlugin(EditorInspectorPlugin plugin)
        {
            ManagedInspectorPlugins.Add(plugin);
            AddInspectorPlugin(plugin);
        }

        public void AddManagedImportPlugin(EditorImportPlugin plugin)
        {
            ManagedImportPlugins.Add(plugin);
            AddImportPlugin(plugin);
        }

        public void AddManagedExportPlugin(EditorExportPlugin plugin)
        {
            ManagedExportPlugins.Add(plugin);
            AddExportPlugin(plugin);
        }

        public void AddManagedSceneImportPlugin(EditorSceneImporter importer)
        {
            ManagedSceneImportPlugins.Add(importer);
            AddSceneImportPlugin(importer);
        }

        public void AddManagedSpatialGizmoPlugin(EditorSpatialGizmoPlugin plugin)
        {
            ManagedSpatialGizmoPlugins.Add(plugin);
            AddSpatialGizmoPlugin(plugin);
        }

        public void AddManagedControl(Control control)
        {
            ManagedControls.Add(new ManagedControl(control, ManagedControlType.Control));
        }

        public void AddManagedControlToDock(DockSlot slot, Control control)
        {
            AddControlToDock(slot, control);

            ManagedControls.Add(new ManagedControl(control, ManagedControlType.Dock));
        }

        public void AddManagedControlToBottomPanel(Control control, string title)
        {
            AddControlToBottomPanel(control, title);

            ManagedControls.Add(new ManagedControl(control, ManagedControlType.BottomPanel));
        }

        public void AddManagedControlToContainer(CustomControlContainer container, Control control)
        {
            AddControlToContainer(container, control);

            ManagedControls.Add(new ManagedControl(control, ManagedControlType.Container,
                new Dictionary
                {
                    ["custom_control_container"] = container
                }
            ));
        }

        public void DestroyManagedControl(Control managedControl)
        {
            int index = ManagedControls.FindIndex(x => x.control == managedControl);
            if (index < 0)
                return;
            DestroyManagedControl(index);
        }

        public void DestroyManagedControl(int index)
        {
            var managedControl = ManagedControls[index];

            switch (managedControl.type)
            {
                case ManagedControlType.BottomPanel:
                    RemoveControlFromBottomPanel(managedControl.control);
                    break;
                case ManagedControlType.Container:
                    RemoveControlFromContainer(
                        managedControl.data.Get<CustomControlContainer>("custom_control_container"),
                        managedControl.control
                    );
                    break;
                case ManagedControlType.Dock:
                    RemoveControlFromDocks(managedControl.control);
                    break;
                case ManagedControlType.Control:
                    break;
            }
            managedControl.control.QueueFree();

            ManagedControls.RemoveAt(index);
        }
        #endregion

        #region Settings
        public object GetSetting(string title)
        {
            return ProjectSettings.GetSetting($"{PluginName}/{title}");
        }

        public T GetSetting<T>(string title)
        {
            return (T)GetSetting(title);
        }

        public void AddSetting(string title, Variant.Type type, object value, PropertyHint hint = PropertyHint.None, string hintString = "")
        {
            title = SettingPath(title);
            if (!ProjectSettings.HasSetting(title))
                ProjectSettings.SetSetting(title, value);
            var info = new Dictionary
            {
                ["name"] = title,
                ["type"] = type,
                ["hint"] = hint,
                ["hint_string"] = hintString,
            };
            ProjectSettings.AddPropertyInfo(info);
        }

        private string SettingPath(string title) => $"{PluginName}/{title}";

        public void Print(string text, PrintMode mode = PrintMode.Text)
        {
            text = $"{PluginName}: {text}";
            switch (mode)
            {
                case PrintMode.Text:
                    GD.Print(text);
                    break;
                case PrintMode.Warning:
                    GD.PushWarning(text);
                    break;
                case PrintMode.Error:
                    GD.PushError(text);
                    break;
            }
        }
        #endregion

        #region SubPlugins
        public GDC.Array<SubPlugin> SubPlugins { get; set; } = new GDC.Array<SubPlugin>();

        public void AddSubPlugin(SubPlugin subPlugin)
        {
            SubPlugins.Add(subPlugin);
            subPlugin.Init(this);
            subPlugin.Load();
        }

        private void UnloadSubPlugins()
        {
            foreach (var subPlugin in SubPlugins)
                subPlugin.Unload();
            SubPlugins.Clear();
            SubPlugins = new Array<SubPlugin>();
        }

        public override void _Process(float delta) => SubPlugins.ForEach(x => x._Process(delta));
        public override void _PhysicsProcess(float delta) => SubPlugins.ForEach(x => x._PhysicsProcess(delta));
        public override void ApplyChanges() => SubPlugins.ForEach(x => x.ApplyChanges());
        public override bool Build()
        {
            foreach (var plugin in SubPlugins)
                if (!plugin.Build())
                    return false;
            return true;
        }
        public override void Clear() => SubPlugins.ForEach(x => x.Clear());
        public override void EnablePlugin() => SubPlugins.ForEach(x => x.EnablePlugin());
        public override void DisablePlugin() => SubPlugins.ForEach(x => x.DisablePlugin());
        public override void Edit(Godot.Object @object) => SubPlugins.ForEach(x => x.Edit(@object));
        public override void ForwardCanvasDrawOverViewport(Control overlay) => SubPlugins.ForEach(x => x.ForwardCanvasDrawOverViewport(overlay));
        public override void ForwardCanvasForceDrawOverViewport(Control overlay) => SubPlugins.ForEach(x => x.ForwardCanvasForceDrawOverViewport(overlay));
        public override bool ForwardCanvasGuiInput(InputEvent @event)
        {
            foreach (var plugin in SubPlugins)
                if (plugin.ForwardCanvasGuiInput(@event))
                    return true;
            return false;
        }
        public override void ForwardSpatialDrawOverViewport(Control overlay) => SubPlugins.ForEach(x => x.ForwardSpatialDrawOverViewport(overlay));
        public override void ForwardSpatialForceDrawOverViewport(Control overlay) => SubPlugins.ForEach(x => x.ForwardSpatialForceDrawOverViewport(overlay));
        public override bool ForwardSpatialGuiInput(Camera camera, InputEvent @event)
        {
            foreach (var plugin in SubPlugins)
                if (plugin.ForwardSpatialGuiInput(camera, @event))
                    return true;
            return false;
        }
        public override string[] GetBreakpoints()
        {
            var breakpoints = new GDC.Array<string>();
            foreach (var plugin in SubPlugins)
            {
                var pluginBreakpoints = plugin.GetBreakpoints();
                if (pluginBreakpoints != null && pluginBreakpoints.Length > 0)
                    breakpoints.AddRange(pluginBreakpoints);
            }
            return breakpoints.ToArray();
        }
        public override Dictionary GetState()
        {
            var state = new Dictionary();
            foreach (var plugin in SubPlugins)
            {
                var pluginState = plugin.GetState();
                if (pluginState != null && pluginState.Keys.Count > 0)
                    state[plugin.PluginName] = pluginState;
            }
            if (state.Keys.Count > 0)
                return state;
            return null;
        }
        public override void SetState(Dictionary state)
        {
            foreach (var plugin in SubPlugins)
                if (state.Contains(plugin.PluginName))
                    plugin.SetState((Dictionary)state[plugin.PluginName]);
        }
        public override void GetWindowLayout(ConfigFile layout) => SubPlugins.ForEach(x => x.GetWindowLayout(layout));
        public override void SetWindowLayout(ConfigFile layout) => SubPlugins.ForEach(x => x.SetWindowLayout(layout));
        public override bool Handles(Godot.Object @object)
        {
            foreach (var plugin in SubPlugins)
                if (plugin.Handles(@object))
                    return true;
            return false;
        }
        public override void MakeVisible(bool visible) => SubPlugins.ForEach(x => x.MakeVisible(visible));
        public override void SaveExternalData() => SubPlugins.ForEach(x => x.SaveExternalData());

        public void OnBeforeSerialize()
        {
            _ExitTree();
        }

        public void OnAfterDeserialize()
        {
            _EnterTree();
        }
        #endregion
    }
}
#endif