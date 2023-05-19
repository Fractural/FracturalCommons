using Fractural.Plugin;
using Fractural.Utils;
using Godot;
using System;
using static Godot.EditorPlugin;

#if TOOLS
namespace Fractural.Commons
{
    public class EditorUtilsPlugin : SubPlugin
    {
        public static EditorUtilsPlugin Global { get; private set; }
        public override string PluginName => "Editor Utils";

        private ColorRect _tintRect;
        private WindowDialog _currSoloWindow;
        private Button _buildButton;

        public override async void Load()
        {
            if (Global != null)
                GD.PushWarning($"{nameof(EditorUtilsPlugin)} global already exists!");
            Global = this;
            _tintRect = new ColorRect();
            _tintRect.Color = new Color(Colors.Black, 0.5f);

            await ToSignal(Plugin.GetTree(), "idle_frame");

            Plugin.GetTree().Root.AddChild(_tintRect);
            Plugin.GetTree().Root.MoveChild(_tintRect, 0);
            _tintRect.SetAnchorsAndMarginsPreset(Control.LayoutPreset.Wide);
            _tintRect.Visible = false;

            // Find the build button by inserting a dummy node into the toolbar container to get a reference to the toolbar container.
            // Then we look through the toolbar container's children for the build button.
            var dummyNode = new Control();
            Plugin.AddControlToContainer(CustomControlContainer.Toolbar, dummyNode);
            foreach (var child in dummyNode.GetParent().GetChildren())
                if (child is Button button && button.Text == "Build")
                {
                    _buildButton = button;
                    break;
                }
            Plugin.RemoveControlFromContainer(CustomControlContainer.Toolbar, dummyNode);
            dummyNode.QueueFree();
        }

        /// <summary>
        /// Makes a popup that has a tint
        /// </summary>
        public void SoloWindowPopup(WindowDialog window, System.Action callPopupFunc = null)
        {
            if (_currSoloWindow != null)
                OnSoloWindowHide();
            _currSoloWindow = window;
            if (callPopupFunc == null)
                _currSoloWindow.PopupCentered();
            else
                callPopupFunc();
            _tintRect.Visible = true;
            _currSoloWindow.Connect("popup_hide", this, nameof(OnSoloWindowHide));
        }

        private void OnSoloWindowHide()
        {
            if (_currSoloWindow.Visible)
                _currSoloWindow.Hide();
            _currSoloWindow.Disconnect("popup_hide", this, nameof(OnSoloWindowHide));
            _currSoloWindow = null;
            _tintRect.Visible = false;
        }

        public void BuildCSharpSolution()
        {
            _buildButton.EmitSignal("pressed");
        }

        public override void Unload()
        {
            if (Global == this)
                Global = null;
            _tintRect.QueueFree();
        }
    }
}
#endif