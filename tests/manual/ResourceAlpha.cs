using Godot;
using System;
using Fractural.Commons;
using Fractural.Utils;
using GDC = Godot.Collections;

namespace Tests.Manual
{
    [RegisteredType(nameof(ResourceAlpha), "res://icon.png")]
    public class ResourceAlpha : Resource
    {
        [Export]
        public int Number { get; set; }
        [Export]
        private string text;

        public GDC.Array Oof()
        {
            GD.Print("hello");
            return GDUtils.GDParams();
        }
    }
}
