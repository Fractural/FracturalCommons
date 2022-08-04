using Fractural.Plugin.AssetsRegistry;
using Godot;
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

		public override void _EnterTree()
		{

		}

		public override void _ExitTree()
		{
			for (int i = ManagedControls.Count - 1; i >= 0; i--)
				DestroyManagedControl(i);
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
	}
}
#endif