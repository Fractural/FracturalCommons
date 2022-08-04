using Fractural.Utils;
using Godot;
using System.Collections.Generic;
using System.Linq;
using Array = Godot.Collections.Array;

#if TOOLS
namespace Fractural.Plugin
{
	[Tool]
	public abstract class ModularPlugin : EditorPlugin
	{
		public enum ManagedControlType
		{
			Control,
			Dock,
			BottomPanel,
			Container,
		}

		public abstract Array PluginModules { get; set; }

		// ----- HACKY WORKAROUND ----- //
		// We have to store managed control Data as just nodes and primitives 
		// due to this Godot engine issue: https://github.com/godotengine/godot/issues/51626
		// Basically Godot cannot serialize non primtiives, NodePaths, and custom Godot.Objects.
		// However it can serialize references to Nodes.
		public abstract List<Control> ManagedControls { get; set; }
		public abstract List<ManagedControlType> ManagedControlsType { get; set; }
		public abstract List<object> ManagedControlsData { get; set; }

		public override void _EnterTree()
		{
			EngineUtils.UpdateVersionPreprocessorDefines();
			LoadPluginModules();
		}

		public override void _ExitTree()
		{
			UnloadPluginModules();
		}

		public virtual void LoadPluginModules()
		{

		}

		public void AddManagedControl(PluginModule moduleOwner, Control control)
		{
			ManagedControls.Add(control);
			ManagedControlsType.Add(ManagedControlType.Control);
			ManagedControlsData.Add(-1);
		}

		public void AddManagedControlToDock(PluginModule moduleOwner, DockSlot slot, Control control)
		{
			AddControlToDock(slot, control);

			ManagedControls.Add(control);
			ManagedControlsType.Add(ManagedControlType.Dock);
			ManagedControlsData.Add(-1);
		}

		public void AddManagedControlToBottomPanel(PluginModule moduleOwner, Control control, string title)
		{
			AddControlToBottomPanel(control, title);

			ManagedControls.Add(control);
			ManagedControlsType.Add(ManagedControlType.BottomPanel);
			ManagedControlsData.Add(-1);
		}

		public void AddManagedControlToContainer(PluginModule moduleOwner, CustomControlContainer container, Control control)
		{
			AddControlToContainer(container, control);

			ManagedControls.Add(control);
			ManagedControlsType.Add(ManagedControlType.Container);
			ManagedControlsData.Add((int) container);
		}

		public void LoadModule(PluginModule module)
		{
			PluginModules.Add(module);
			module.Init(this);
		}

		public void UnloadPluginModules()
		{
			// PluginModule is an Godot.Collections.Array in order to survive rebuilds.
			// Once rebuilt, the objects inside PluginModules are no longer PluginModules
			// and instead of generic Godot.Objects. We therefore use this check
			// to only unload the modules if it is possible.
			if (PluginModules.Count > 0 && PluginModules[0] is PluginModule)
			{
				foreach (PluginModule module in PluginModules)
					module.Unload();
			}
			PluginModules.Clear();

			// PluginModules should call DestroyManagedControl within their own
			// Unload() methods. However we also manually destroy every remaining
			// control in case we have rebuilt the solution, which prevents
			// us from calling Unload() on the modules.
			for (int i = ManagedControls.Count - 1; i >= 0; i--)
				DestroyManagedControl(i);
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
					RemoveControlFromContainer((CustomControlContainer) data, managedControl);
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

		public void UnloadModule(PluginModule module)
		{
			if (!PluginModules.Contains(module))
				return;

			PluginModules.Remove(module);
			module.Unload();
		}
	}
}
#endif