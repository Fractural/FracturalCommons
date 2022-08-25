using Godot;

namespace Fractural
{
    public struct PropertyListItem
    {
        public string name;
        public Variant.Type type;
        public PropertyHint hint;
        public string hintString;
        public PropertyUsageFlags usage;

        public PropertyListItem(string name, Variant.Type type, PropertyHint hint = PropertyHint.None, string hintString = "", PropertyUsageFlags usage = PropertyUsageFlags.Default)
        {
            this.name = name;
            this.type = type;
            this.hint = hint;
            this.hintString = hintString;
            this.usage = usage;
        }

        public static implicit operator Godot.Collections.Dictionary(PropertyListItem item)
        {
            var dict = new Godot.Collections.Dictionary();
            dict["name"] = item.name;
            dict["type"] = item.type;
            dict["hint_string"] = item.hintString;
            dict["usage"] = item.usage;
            return dict;
        }
    };
}
