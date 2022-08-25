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

		public IAssetsRegistry AssetsRegistry { get; private set; } = new DefaultAssetsRegistry();

		public abstract string PluginName { get; }
		public List<EditorInspectorPlugin> ManagedInspectorPlugins { get; } = new List<EditorInspectorPlugin>();
		public List<EditorImportPlugin> ManagedImportPlugins { get; } = new List<EditorImportPlugin>();
		public List<EditorExportPlugin> ManagedExportPlugins { get; } = new List<EditorExportPlugin>();
		public List<EditorSceneImporter> ManagedSceneImportPlugins { get; } = new List<EditorSceneImporter>();
		public List<EditorSpatialGizmoPlugin> ManagedSpatialGizmoPlugins { get; } = new List<EditorSpatialGizmoPlugin>();

		public override void _EnterTree()
		{
			Load();
			GD.PushWarning($"Loaded {PluginName}");
		}

		public override void _ExitTree()
		{
			Unload();
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

		protected virtual void Load()
		{

		}

		protected virtual void Unload()
		{

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
	}
}
#endif