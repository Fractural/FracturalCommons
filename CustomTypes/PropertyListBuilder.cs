using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Fractural
{
    public class PropertyListBuilder
    {
        public List<PropertyListItem> Items { get; } = new List<PropertyListItem>();

        public void AddItem(PropertyListItem item)
        {
            Items.Add(item);
        }

        public void AddItem(string name, Variant.Type type, PropertyHint hint = PropertyHint.None, string hintString = "", PropertyUsageFlags usage = PropertyUsageFlags.Default)
        {
            AddItem(new PropertyListItem(name, type, hint, hintString, usage));
        }

        public Godot.Collections.Array Build()
        {
            return new Godot.Collections.Array(Items.Select(x => x.ToGDDict()));
        }
    }
}
