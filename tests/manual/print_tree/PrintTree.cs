using Fractural.Utils;
using Godot;
using System;

namespace Tests.Manual
{
    public class PrintTree : Node2D
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            EditorHackUtils.PrintTree(this);
        }
    }
}