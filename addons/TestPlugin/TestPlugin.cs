using Fractural.Plugin;
using Fractural.Utils;
using Godot;
using System;

#if TOOLS
namespace TestPlugin
{
    /// <summary>
    /// Plugin to test Fractural Commons' functionality.
    /// </summary>
    [Tool]
    public class TestPlugin : ExtendedPlugin
    {
        public override string PluginName => "Test Plugin";

        private Control _uiControl;
        private NodeSelectPopup _nodeSelectPopup;
        private Label _infoText;
        private SearchEdit _searchEdit;

        protected override void Load()
        {
            _uiControl = new MarginContainer();
            _nodeSelectPopup = new NodeSelectPopup();
            _infoText = new Label();
            _searchEdit = new SearchEdit();

            var hBox = new HBoxContainer();
            var testNodeSelectPopupButton = new Button();
            testNodeSelectPopupButton.Text = "Test Node Popup";
            testNodeSelectPopupButton.Connect("pressed", this, nameof(OnTestNodeSelectPopup));
            hBox.AddChild(testNodeSelectPopupButton);
            hBox.AddChild(_searchEdit);

            var vBox = new VBoxContainer();
            _searchEdit.SearchEntries = new[]
            {
                "Heyo",
                "Testing",
                "Testingggg",
                "The quick brown",
                "The quick browN Fox jumps",
                "The quick browN Fox jumps over the",
            };
            vBox.AddChild(_infoText);
            vBox.AddChild(hBox);

            _nodeSelectPopup.Connect(nameof(NodeSelectPopup.NodeSelected), this, nameof(OnNodeSelected));

            _uiControl.AddChild(vBox);
            _uiControl.AddChild(_nodeSelectPopup);
            AddManagedControlToBottomPanel(_uiControl, "Test Plugin");
        }

        protected override void Unload()
        {
            _uiControl.QueueFree();
        }

        private void OnTestNodeSelectPopup()
        {
            _nodeSelectPopup.SoloEditorPopup(() => _nodeSelectPopup.Popup(GetTree().Root));
        }

        private void OnNodeSelected(Node node)
        {
            _infoText.Text = $"{nameof(NodeSelectPopup)}: Found node \"{node}\" at root path \"{GetTree().Root.GetPathTo(node)}\"";
        }
    }
}
#endif
