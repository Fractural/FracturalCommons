using Fractural.Plugin;
using Godot;

#if TOOLS
namespace Fractural.Commons
{
    public class EditorUtilsPlugin : SubPlugin
    {
        public static EditorUtilsPlugin Global { get; private set; }
        public override string PluginName => "Editor Utils";

        private ColorRect _tintRect;
        private Popup _currSoloPopup;

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
        }

        /// <summary>
        /// Makes a popup that has a tint
        /// </summary>
        public void SoloPopup(Popup popup, System.Action callPopupFunc = null)
        {
            if (_currSoloPopup != null)
                OnSoloPopupHide();
            _currSoloPopup = popup;
            if (callPopupFunc == null)
                _currSoloPopup.PopupCentered();
            else
                callPopupFunc();
            _tintRect.Visible = true;
            _currSoloPopup.Connect("popup_hide", this, nameof(OnSoloPopupHide));
        }

        private void OnSoloPopupHide()
        {
            if (_currSoloPopup.Visible)
                _currSoloPopup.Hide();
            _currSoloPopup.Disconnect("popup_hide", this, nameof(OnSoloPopupHide));
            _currSoloPopup = null;
            _tintRect.Visible = false;
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