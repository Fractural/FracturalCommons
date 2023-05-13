using Fractural.Utils;
using Godot;
using System.Collections.Generic;

namespace Fractural.Plugin
{
    [CSharpScript]
    [Tool]
    public class PopupMenuScrollable : PopupPanel
    {
        [Signal]
        public delegate void IndexPressed(int index);

        [Export]
        public Vector2 MaxSize { get; set; }
        public IReadOnlyList<string> Items => _items;
        [Export]
        private List<string> _items = new List<string>();
        [Export]
        public int ItemListLineHeight { get; set; } = 24;
        [Export]
        public int ItemListContentOffset { get; set; } = 8;

        private ItemList _itemList;
        private StyleBox _panel;

        public override void _Ready()
        {
            if (NodeUtils.IsInEditorSceneTab(this))
                return;

            _itemList = new ItemList();
            _itemList.Connect("item_activated", this, nameof(OnItemActivated));
            foreach (string item in Items)
                _itemList.AddItem(item);

            _panel = GetStylebox("panel");
            AddChild(_itemList);
            Connect("about_to_show", this, nameof(OnPopup));
        }

        public void Clear()
        {
            _items.Clear();
            if (IsInsideTree())
                UpdateSizing();
        }

        public void AddItemRange(IEnumerable<string> labels)
        {
            foreach (var label in labels)
                AddItem(label);
        }

        public void AddItem(string label, int idx = -1)
        {
            if (idx < 0)
                idx = _items.Count;
            _items.Insert(idx, label);
            if (IsInsideTree())
            {
                _itemList.AddItem(label);
                UpdateSizing();
            }
        }

        public void RemoveItem(int idx)
        {
            _items.RemoveAt(idx);
            if (IsInsideTree())
            {
                _itemList.RemoveItem(idx);
                UpdateSizing();
            }
        }

        public string GetItemText(int idx)
        {
            return _items[idx];
        }

        private void OnItemActivated(int index)
        {
            EmitSignal(nameof(IndexPressed), index);
            Hide();
        }

        private void UpdateSizing()
        {
            // Find sizing to fit content or hit max height
            int contentHeight = _itemList.GetItemCount() * ItemListLineHeight;
            var popupSize = new Vector2(RectSize.x, contentHeight + _panel.ContentMarginTop + _panel.ContentMarginBottom + ItemListContentOffset);
            if (MaxSize.y > 0 && popupSize.y > MaxSize.y)
                popupSize.y = MaxSize.y;
            if (MaxSize.x > 0 && popupSize.x > MaxSize.x)
                popupSize.x = MaxSize.x;
            RectSize = popupSize;
        }

        private async void OnPopup()
        {
            await ToSignal(GetTree(), "idle_frame");
            UpdateSizing();
        }
    }
}