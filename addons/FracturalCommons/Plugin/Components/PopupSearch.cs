using Fractural.Utils;
using Godot;

namespace Fractural.Plugin
{
    [CSharpScript]
    [Tool]
    public class PopupSearch : PopupPanel
    {
        [Signal]
        public delegate void EntrySelected(string entry);

        [Export]
        public Vector2 MaxSize { get; set; } = new Vector2(300, 600);
        [Export]
        public string[] SearchEntries { get; set; } = new string[0];
        [Export]
        public bool CaseSensitive { get; set; } = false;
        [Export]
        public int ItemListLineHeight { get; set; } = 24;
        [Export]
        public int ItemListContentOffset { get; set; } = 8;

        private LineEdit _searchBar;
        private ItemList _searchEntriesItemList;
        private int _vboxSeparation;
        private StyleBox _panel;

        public override void _Ready()
        {
            if (NodeUtils.IsInEditorSceneTab(this))
                return;

            _searchBar = new LineEdit();
            _searchBar.PlaceholderText = "Filter";
            _searchBar.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            _searchBar.RightIcon = GetIcon("Search", "EditorIcons");
            _searchBar.Connect("text_changed", this, nameof(OnSearchBarTextChanged));
            _searchBar.RectMinSize = new Vector2(200, _searchBar.RectMinSize.y);

            _searchEntriesItemList = new ItemList();
            _searchEntriesItemList.Connect("item_activated", this, nameof(OnItemActivated));
            _searchEntriesItemList.SizeFlagsHorizontal = _searchEntriesItemList.SizeFlagsVertical = (int)SizeFlags.ExpandFill;

            var vBox = new VBoxContainer();
            vBox.AddChild(_searchBar);
            vBox.AddChild(_searchEntriesItemList);
            _vboxSeparation = vBox.GetConstant("separation");

            _panel = GetStylebox("panel");
            AddChild(vBox);
            Connect("about_to_show", this, nameof(OnPopup));
        }

        private void OnSearchBarTextChanged(string newText) => UpdateSearchEntries();
        private void OnItemActivated(int index)
        {
            EmitSignal(nameof(EntrySelected), _searchEntriesItemList.GetItemText(index));
            Hide();
        }

        private void UpdateSearchEntries()
        {
            _searchEntriesItemList.Clear();
            foreach (var entry in SearchEntries)
            {
                if (_searchBar.Text != "" && ((CaseSensitive && entry.Find(_searchBar.Text) < 0) || entry.ToLower().Find(_searchBar.Text.ToLower()) < 0))
                    continue;
                _searchEntriesItemList.AddItem(entry);
            }

            // Find sizing to fit content or hit max height
            int contentHeight = _searchEntriesItemList.GetItemCount() * ItemListLineHeight;
            var popupSize = new Vector2(RectSize.x, contentHeight + _searchBar.RectSize.y + _vboxSeparation + _panel.ContentMarginTop + _panel.ContentMarginBottom + ItemListContentOffset);
            if (MaxSize.y > 0 && popupSize.y > MaxSize.y)
                popupSize.y = MaxSize.y;
            if (MaxSize.x > 0 && popupSize.x > MaxSize.x)
                popupSize.x = MaxSize.x;
            RectSize = popupSize;
        }

        private void OnPopup()
        {
            _searchBar.GrabFocus();
            _searchEntriesItemList.RectMinSize = Vector2.Zero;

            // Reset entries
            UpdateSearchEntries();
        }
    }
}