using Godot;

namespace Fractural
{
    public class PropertyListItem
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

        public Godot.Collections.Dictionary ToGDDict()
        {
            return new Godot.Collections.Dictionary
            {
                ["name"] = name,
                ["type"] = type,
                ["hint_string"] = hintString,
                ["usage"] = usage
            };
        }
    };
}
