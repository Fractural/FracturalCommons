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
        public Godot.Collections.Array Build()
        {
            return new Godot.Collections.Array(Items.Select(x => x.ToGDDict()));
        }
    }
}
