using Fractural;
using Godot;
using System;
using GDC = Godot.Collections;

namespace Tests
{
    [Tool]
    public class CustomInspectors : Node
    {
        [Export]
        public int TestInt { get; set; }
        [Export]
        public float TestFloat { get; set; }
        [Export]
        public bool TestBool { get; set; }
        [Export]
        public string TestString { get; set; }
        [Export]
        public NodePath TestNodePath { get; set; }
        [Export]
        public GDC.Dictionary TestDictionary { get; set; } = new GDC.Dictionary();
        [Export]
        public int ValueInt { get; set; }
        [Export]
        public float ValueFloat { get; set; }
        [Export]
        public bool ValueBool { get; set; }
        [Export]
        public string ValueString { get; set; }
        [Export]
        public NodePath ValueNodePath { get; set; }
        public GDC.Dictionary ValueStringToIntDictionary { get; set; } = new GDC.Dictionary();
        public GDC.Dictionary ValueFloatToNodePathDictionary { get; set; } = new GDC.Dictionary();
        public GDC.Dictionary ValueBoolToStringDictionary { get; set; } = new GDC.Dictionary();
        public GDC.Dictionary ValueNodePathToStringDictionary { get; set; } = new GDC.Dictionary();

        public override GDC.Array _GetPropertyList()
        {
            var builder = new PropertyListBuilder();
            builder.AddItem(
                name: nameof(ValueStringToIntDictionary),
                type: Variant.Type.Dictionary,
                hintString: HintString.GetTypedDictionary<string, int>()
            );
            builder.AddItem(
                name: nameof(ValueFloatToNodePathDictionary),
                type: Variant.Type.Dictionary,
                hintString: HintString.GetTypedDictionary<float, NodePath>()
            );
            builder.AddItem(
                name: nameof(ValueBoolToStringDictionary),
                type: Variant.Type.Dictionary,
                hintString: HintString.GetTypedDictionary<bool, string>()
            );
            builder.AddItem(
                name: nameof(ValueNodePathToStringDictionary),
                type: Variant.Type.Dictionary,
                hintString: HintString.GetTypedDictionary<NodePath, string>()
            );
            return builder.Build();
        }
    }
}