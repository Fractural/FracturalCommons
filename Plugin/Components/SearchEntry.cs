using Fractural.Utils;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Fractural.Plugin
{
    public struct SearchEntry
    {
        public SearchEntry(string text, Texture icon = null)
        {
            Text = text;
            Icon = icon;
        }

        public string Text { get; set; }
        public Texture Icon { get; set; }

        public static SearchEntry[] ArrayFromStringEnumerable(IEnumerable<string> values) => values.Select(x => new SearchEntry(x)).ToArray();
    }
}