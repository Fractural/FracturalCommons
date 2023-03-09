using Godot;
using System;
using Fractural.Commons;

namespace Tests.Manual
{
    [RegisteredType(nameof(ResourceAlpha), "res://icon.png")]
    public class ResourceAlpha : Resource
    {
        [Export]
        public int Number { get; set; }
        [Export]
        private string text;
    }
}
