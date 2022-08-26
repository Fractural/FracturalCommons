﻿using Fractural.Plugin.AssetsRegistry;
using Godot;
using Godot.Collections;
using System.Collections.Generic;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public abstract class ExtendedPlugin : EditorPlugin
    {
        public IAssetsRegistry AssetsRegistry { get; private set; } = new DefaultAssetsRegistry();
        public abstract string PluginName { get; }

        public override void _EnterTree()
        {
            Load();
            GD.PushWarning($"Loaded {PluginName}");
        }

        public override void _ExitTree()
        {
            Unload();
            UnloadManagedObjects();
            UnloadSubPlugins();
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

        public List<Control> ManagedControls { get; set; } = new List<Control>();
        public List<ManagedControlType> ManagedControlsType { get; set; } = new List<ManagedControlType>();
        public List<object> ManagedControlsData { get; set; } = new List<object>();

        public List<EditorInspectorPlugin> ManagedInspectorPlugins { get; } = new List<EditorInspectorPlugin>();
        public List<EditorImportPlugin> ManagedImportPlugins { get; } = new List<EditorImportPlugin>();
        public List<EditorExportPlugin> ManagedExportPlugins { get; } = new List<EditorExportPlugin>();
        public List<EditorSceneImporter> ManagedSceneImportPlugins { get; } = new List<EditorSceneImporter>();
        public List<EditorSpatialGizmoPlugin> ManagedSpatialGizmoPlugins { get; } = new List<EditorSpatialGizmoPlugin>();

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
            ManagedControls.Add(control);
            ManagedControlsType.Add(ManagedControlType.Control);
            ManagedControlsData.Add(-1);
        }

        public void AddManagedControlToDock(DockSlot slot, Control control)
        {
            AddControlToDock(slot, control);

            ManagedControls.Add(control);
            ManagedControlsType.Add(ManagedControlType.Dock);
            ManagedControlsData.Add(-1);
        }

        public void AddManagedControlToBottomPanel(Control control, string title)
        {
            AddControlToBottomPanel(control, title);

            ManagedControls.Add(control);
            ManagedControlsType.Add(ManagedControlType.BottomPanel);
            ManagedControlsData.Add(-1);
        }

        public void AddManagedControlToContainer(CustomControlContainer container, Control control)
        {
            AddControlToContainer(container, control);

            ManagedControls.Add(control);
            ManagedControlsType.Add(ManagedControlType.Container);
            ManagedControlsData.Add((int)container);
        }

        public void DestroyManagedControl(Control managedControl)
        {
            int index = ManagedControls.IndexOf(managedControl);
            if (index < 0)
                return;
            DestroyManagedControl(index);
        }

        public void DestroyManagedControl(int index)
        {
            var managedControl = ManagedControls[index];
            var type = ManagedControlsType[index];
            var data = ManagedControlsData[index];

            switch (type)
            {
                case ManagedControlType.BottomPanel:
                    RemoveControlFromBottomPanel(managedControl);
                    break;
                case ManagedControlType.Container:
                    RemoveControlFromContainer((CustomControlContainer)data, managedControl);
                    break;
                case ManagedControlType.Dock:
                    RemoveControlFromDocks(managedControl);
                    break;
                case ManagedControlType.Control:
                    break;
            }
            managedControl.Free();

            ManagedControls.RemoveAt(index);
            ManagedControlsType.RemoveAt(index);
            ManagedControlsData.RemoveAt(index);
        }
        #endregion

        #region Settings
        protected object GetSetting(string title)
        {
            return ProjectSettings.GetSetting($"{PluginName}/{title}");
        }

        protected void AddSetting(string title, Variant.Type type, object value, PropertyHint hint = PropertyHint.None, string hintString = "")
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
        #endregion

        #region SubPlugins
        public List<SubPlugin> SubPlugins { get; } = new List<SubPlugin>();

        public void AddSubPlugin(SubPlugin subPlugin)
        {
            SubPlugins.Add(subPlugin);
            subPlugin.Load();
        }

        private void UnloadSubPlugins()
        {
            foreach (var subPlugin in SubPlugins)
                subPlugin.Unload();
        }

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
        public override void Edit(Object @object) => SubPlugins.ForEach(x => x.Edit(@object));
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
            var breakpoints = new List<string>();
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
                    state[plugin.GetPluginName()] = pluginState;
            }
            if (state.Keys.Count > 0)
                return state;
            return null;
        }
        public override void SetState(Dictionary state)
        {
            foreach (var plugin in SubPlugins)
                if (state.Contains(plugin.GetPluginName()))
                    plugin.SetState((Dictionary)state[plugin.GetPluginName()]);
        }
        public override void GetWindowLayout(ConfigFile layout) => SubPlugins.ForEach(x => x.GetWindowLayout(layout));
        public override void SetWindowLayout(ConfigFile layout) => SubPlugins.ForEach(x => x.SetWindowLayout(layout));
        public override bool Handles(Object @object)
        {
            foreach (var plugin in SubPlugins)
                if (plugin.Handles(@object))
                    return true;
            return false;
        }
        public override void MakeVisible(bool visible) => SubPlugins.ForEach(x => x.MakeVisible(visible));
        public override void SaveExternalData() => SubPlugins.ForEach(x => x.SaveExternalData());
        #endregion
    }
}
#endif