using Godot;
using System.Collections.Generic;
using GDC = Godot.Collections;

namespace Fractural
{
	public class PropertyListBuilder
	{
		public List<PropertyListItem> Items { get; } = new List<PropertyListItem>();
		public GDC.Array PropertyArray { get; private set; }

		public PropertyListBuilder() : this(new GDC.Array()) { }
		public PropertyListBuilder(GDC.Array propertyArray)
		{
			PropertyArray = propertyArray != null ? propertyArray : new GDC.Array();
		}

		public void AddItem(PropertyListItem item)
		{
			Items.Add(item);
		}

		public void AddItem(string name, Variant.Type type, PropertyHint hint = PropertyHint.None, string hintString = "", PropertyUsageFlags usage = PropertyUsageFlags.Default)
		{
			AddItem(new PropertyListItem(name, type, hint, hintString, usage));
		}

		public GDC.Array Build()
		{
			foreach (var item in Items)
				PropertyArray.Add(item.ToGDDict());
			return PropertyArray;
		}
	}
}
