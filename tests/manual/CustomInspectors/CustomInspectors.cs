using Godot;
using System;

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
        public int ValueInt { get; set; }
        [Export]
        public float ValueFloat { get; set; }
        [Export]
        public bool ValueBool { get; set; }
        [Export]
        public string ValueString { get; set; }
        [Export]
        public NodePath ValueNodePath { get; set; }
    }
}