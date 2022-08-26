﻿using Godot;
using Godot.Collections;

#if TOOLS
namespace Fractural.Plugin
{
	public abstract class SubPlugin : Godot.Reference
	{
		public EditorPlugin Plugin { get; private set; }
		public virtual string PluginName => "";

		public void Init(EditorPlugin plugin)
		{
			Plugin = plugin;
		}

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
	}
}
#endif