using Fractural.Utils;
using Godot;
using GDC = Godot.Collections;

namespace Fractural
{
    public class PropertyListItem
    {
        public string Name { get; set; }
        public Variant.Type Type { get; set; }
        public PropertyHint Hint { get; set; }
        public string HintString { get; set; }
        public PropertyUsageFlags Usage { get; set; }

        public PropertyListItem(string name, Variant.Type type, PropertyHint hint = PropertyHint.None, string hintString = "", PropertyUsageFlags usage = PropertyUsageFlags.Default)
        {
            this.Name = name;
            this.Type = type;
            this.Hint = hint;
            this.HintString = hintString;
            this.Usage = usage;
        }

        public GDC.Dictionary ToGDDict()
        {
            return new GDC.Dictionary
            {
                ["name"] = Name,
                ["type"] = Type,
                ["hint"] = Hint,
                ["hint_string"] = HintString,
                ["usage"] = Usage
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
