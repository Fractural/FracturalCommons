using Fractural.Plugin;
using Fractural.Utils;
using Godot;

#if TOOLS
namespace Tests
{
    /// <summary>
    /// Plugin to test Fractural Commons' functionality.
    /// </summary>
    [Tool]
    public class TestPlugin : ExtendedPlugin
    {
        private readonly static string[] TestStringArray = new[]
        {
            "Heyo",
            "Testing",
            "Testingggg",
            "The quick brown",
            "The quick browN Fox jumps",
            "The quick browN Fox jumps over the",
            "Lorem ipsum",
            "this",
            "is a lot of text",
            "Wow this is is alot",
            "The quick brown fox jumps over",
            "the laziest dog",
            "of all time",
            "Heyo",
            "Testing",
            "Testingggg",
            "The quick brown",
            "The quick browN Fox jumps",
            "The quick browN Fox jumps over the",
            "Lorem ipsum",
            "this",
            "is a lot of text",
            "Wow this is is alot",
            "The quick brown fox jumps over",
            "the laziest dog",
            "of all time",
            "Heyo",
            "Testing",
            "Testingggg",
            "The quick brown",
            "The quick browN Fox jumps",
            "The quick browN Fox jumps over the",
            "Lorem ipsum",
            "this",
            "is a lot of text",
            "Wow this is is alot",
            "The quick brown fox jumps over",
            "the laziest dog",
            "of all time",
            "Heyo",
            "Testing",
            "Testingggg",
            "The quick brown",
            "The quick browN Fox jumps",
            "The quick browN Fox jumps over the",
            "Lorem ipsum",
            "this",
            "is a lot of text",
            "Wow this is is alot",
            "The quick brown fox jumps over",
            "the laziest dog",
            "of all time",
            "Heyo",
            "Testing",
            "Testingggg",
            "The quick brown",
            "The quick browN Fox jumps",
            "The quick browN Fox jumps over the",
            "Lorem ipsum",
            "this",
            "is a lot of text",
            "Wow this is is alot",
            "The quick brown fox jumps over",
            "the laziest dog",
            "of all time",
            "Heyo",
            "Testing",
            "Testingggg",
            "The quick brown",
            "The quick browN Fox jumps",
            "The quick browN Fox jumps over the",
            "Lorem ipsum",
            "this",
            "is a lot of text",
            "Wow this is is alot",
            "The quick brown fox jumps over",
            "the laziest dog",
            "of all time",
        };
        public override string PluginName => "Test Plugin";

        private Control _uiControl;
        private NodeSelectDialog _nodeSelectDialog;
        private NodeSelectDialog _conditionNodeSelectDialog;
        private Label _infoText;
        private SearchEdit _searchEdit;
        private SearchDialog _searchDialog;
        private PopupMenuScrollable _popupMenuScrollable;
        private PopupSearch _popupSearch;

        protected override void Load()
        {
            _uiControl = new MarginContainer();
            _infoText = new Label();

            _nodeSelectDialog = new NodeSelectDialog();
            _nodeSelectDialog.RootNode = GetTree().Root;

            _conditionNodeSelectDialog = new NodeSelectDialog();
            _conditionNodeSelectDialog.RootNode = GetTree().Root;
            _conditionNodeSelectDialog.NodeConditionFunc = (node) => node is Button;

            _searchEdit = new SearchEdit();
            _searchEdit.RectMinSize = new Vector2(150, 0);
            _searchEdit.SearchEntries = SearchEntry.ArrayFromStringEnumerable(TestStringArray);
            _searchEdit.Connect("text_changed", this, nameof(OnSearchEditTextChanged));

            _searchDialog = new SearchDialog();
            _searchDialog.SearchEntries = SearchEntry.ArrayFromStringEnumerable(TestStringArray);
            _searchDialog.Connect(nameof(SearchDialog.EntrySelected), this, nameof(OnSearchDialogEntrySelected));
            _uiControl.AddChild(_searchDialog);

            _popupMenuScrollable = new PopupMenuScrollable();
            _popupMenuScrollable.MaxSize = new Vector2(300, 600);
            _popupMenuScrollable.AddItemRange(TestStringArray);
            _popupMenuScrollable.Connect(nameof(PopupMenuScrollable.IndexPressed), this, nameof(OnPopupMenuScrollableIndexPressed));
            _uiControl.AddChild(_popupMenuScrollable);

            _popupSearch = new PopupSearch();
            _popupSearch.SearchEntries = SearchEntry.ArrayFromStringEnumerable(TestStringArray);
            _popupSearch.Connect(nameof(PopupSearch.EntrySelected), this, nameof(OnPopupSearchEntrySelected));
            _uiControl.AddChild(_popupSearch);

            var testNodeSelectPopupButton = new Button();
            testNodeSelectPopupButton.Text = "Test Node Popup";
            testNodeSelectPopupButton.Connect("pressed", this, nameof(OnTestNodeSelectDialog));

            var testConditionNodeSelectPopupButton = new Button();
            testConditionNodeSelectPopupButton.Text = "Test Condition Node Popup";
            testConditionNodeSelectPopupButton.Connect("pressed", this, nameof(OnTestConditionNodeSelectDialog));

            var testSearchPopupButton = new Button();
            testSearchPopupButton.Text = "Test Search Popup";
            testSearchPopupButton.Connect("pressed", this, nameof(OnTestSearchDialog));

            var testPopupMenuScrollableButton = new Button();
            testPopupMenuScrollableButton.Text = "Test Popup Menu Scrollable";
            testPopupMenuScrollableButton.Connect("pressed", this, nameof(OnTestPopupMenuScrollable));

            var testPopupSearchButton = new Button();
            testPopupSearchButton.Text = "Test Popup Search";
            testPopupSearchButton.Connect("pressed", this, nameof(OnTestPopupSearch));

            var testBuildCSharpSolutionButton = new Button();
            testBuildCSharpSolutionButton.Text = "Test Build CSharp Solution";
            testBuildCSharpSolutionButton.Connect("pressed", this, nameof(OnTestBuildCSharpSolution));

            var hBox = new HBoxContainer();
            hBox.AddChild(testNodeSelectPopupButton);
            hBox.AddChild(testConditionNodeSelectPopupButton);
            hBox.AddChild(testSearchPopupButton);
            hBox.AddChild(testPopupMenuScrollableButton);
            hBox.AddChild(testPopupSearchButton);
            hBox.AddChild(testBuildCSharpSolutionButton);
            hBox.AddChild(_searchEdit);

            var vBox = new VBoxContainer();
            vBox.AddChild(_infoText);
            vBox.AddChild(hBox);

            _nodeSelectDialog.Connect(nameof(NodeSelectDialog.NodeSelected), this, nameof(OnNodeSelected));
            _conditionNodeSelectDialog.Connect(nameof(NodeSelectDialog.NodeSelected), this, nameof(OnConditionNodeSelected));

            _uiControl.AddChild(vBox);
            _uiControl.AddChild(_nodeSelectDialog);
            _uiControl.AddChild(_conditionNodeSelectDialog);
            AddManagedControlToBottomPanel(_uiControl, "Test Plugin");

            AddManagedInspectorPlugin(new TestInspectorPlugin(this));
        }

        protected override void Unload()
        {
            _uiControl.QueueFree();
        }

        private void OnTestNodeSelectDialog()
        {
            _nodeSelectDialog.SoloEditorWindowPopup(() => _nodeSelectDialog.PopupCenteredRatio());
        }

        private void OnTestConditionNodeSelectDialog()
        {
            _conditionNodeSelectDialog.SoloEditorWindowPopup(() => _conditionNodeSelectDialog.PopupCenteredRatio());
        }

        private void OnNodeSelected(Node node)
        {
            _infoText.Text = $"{nameof(NodeSelectDialog)}: Selected node \"{node}\" at root path \"{GetTree().Root.GetPathTo(node)}\"";
        }

        private void OnConditionNodeSelected(Node node)
        {
            _infoText.Text = $"{nameof(NodeSelectDialog)}: Using condition node is Control, selected node \"{node}\" at root path \"{GetTree().Root.GetPathTo(node)}\"";
        }

        private void OnTestSearchDialog()
        {
            _searchDialog.SoloEditorWindowPopup(() => _searchDialog.PopupCenteredRatio());
        }

        private void OnSearchDialogEntrySelected(string item)
        {
            _infoText.Text = $"{nameof(SearchDialog)}: Selected item \"{item}\"";
        }

        private void OnTestPopupMenuScrollable()
        {
            _popupMenuScrollable.PopupCentered(new Vector2(200, 600));
        }

        private void OnPopupMenuScrollableIndexPressed(int index)
        {
            _infoText.Text = $"{nameof(PopupMenuScrollable)}: Selected index \"{index}\" = \"{_popupMenuScrollable.Items[index]}\"";
        }

        private void OnTestPopupSearch()
        {
            _popupSearch.PopupCentered(new Vector2(200, 600));
        }

        private void OnPopupSearchEntrySelected(string item)
        {
            _infoText.Text = $"{nameof(PopupSearch)}: Selected item \"{item}\"";
        }

        private void OnSearchEditTextChanged(string text)
        {
            _infoText.Text = $"{nameof(SearchEdit)}: Text changed to \"{text}\"";
        }

        private void OnTestBuildCSharpSolution()
        {
            EditorUtils.BuildCSharpSolution();
        }
    }
}
#endif
