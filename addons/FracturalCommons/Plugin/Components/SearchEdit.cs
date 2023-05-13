using Fractural.Utils;
using Godot;

namespace Fractural.Plugin
{
    [CSharpScript]
    [Tool]
    public class SearchEdit : LineEdit
    {
        [Export]
        public string[] SearchEntries { get; set; } = new string[0];
        [Export]
        public int Limit { get; set; } = 5;
        [Export]
        public bool CaseSensitive { get; set; } = false;

        private PopupMenu _searchEntriesPopupMenu;

        public override void _Ready()
        {
            if (NodeUtils.IsInEditorSceneTab(this))
                return;
            _searchEntriesPopupMenu = new PopupMenu();
            _searchEntriesPopupMenu.PopupExclusive = false;
            AddChild(_searchEntriesPopupMenu);

            _searchEntriesPopupMenu.Connect("index_pressed", this, nameof(OnSearchMenuIndexPressed));
            GetViewport().Connect("gui_focus_changed", this, nameof(OnFocusChanged));
            Connect("text_changed", this, nameof(OnTextChanged));
        }

        private void OnTextChanged(string newText)
        {
            if (SearchEntries.Length == 0)
                return;
            UpdateSearchEntries();
            PopupResults();
        }

        private void UpdateSearchEntries()
        {
            _searchEntriesPopupMenu.Clear();
            int index = 0;
            foreach (var entry in SearchEntries)
            {
                if (Text != "" && ((CaseSensitive && entry.Find(Text) < 0) || entry.ToLower().Find(Text.ToLower()) < 0))
                    continue;
                _searchEntriesPopupMenu.AddItem(entry);
                index++;
                if (index >= Limit)
                    return;
            }
        }

        private bool _searchEntriesFocused = false;
        private bool _searchEdiFocused = false;

        private void OnFocusChanged(Control control)
        {
            if (control == this)
                PopupResults();
            else
                _searchEntriesPopupMenu.Hide();
        }

        private void OnSearchMenuIndexPressed(int index)
        {
            Text = _searchEntriesPopupMenu.GetItemText(index);
            EmitSignal("text_changed", Text);
        }

        private void PopupResults()
        {
            UpdateSearchEntries();
            var globalRect = GetGlobalRect();
            globalRect.Position += new Vector2(0, globalRect.Size.y);

            _searchEntriesPopupMenu.FocusMode = FocusModeEnum.None;
            _searchEntriesPopupMenu.Popup_(globalRect);
        }

    }
}