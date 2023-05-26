using Fractural.Utils;
using Godot;
using GDC = Godot.Collections;

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

        public GDC.Dictionary ToGDDict()
        {
            return new GDC.Dictionary
            {
                ["name"] = name,
                ["type"] = type,
                ["hint"] = hint,
                ["hint_string"] = hintString,
                ["usage"] = usage
            };
        }

        public static PropertyListItem FromGDDict(GDC.Dictionary dict)
        {
            return new PropertyListItem(
                name: dict.Get<string>("name"),
                type: dict.Get<Variant.Type>("type"),
                hint: (PropertyHint)dict.Get<int>("hint", 0),
                hintString: dict.Get<string>("hint_string", ""),
                usage: (PropertyUsageFlags)dict.Get<int>("usage", 0)
            );
        }
    };
}
