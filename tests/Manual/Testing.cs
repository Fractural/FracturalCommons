using Godot;
using System;
using Fractural.SceneManagement;
using Fractural.Utils;
using Tests;

public class Testing : Node2D
{
    public override void _Ready()
    {
        string relativePath = CSharpUtils.GetRelativePath<SceneManagerTests>();
        SceneManager sceneManager = CSharpUtils.InstantiateCSharpNode<SceneManager>();
        AddChild(sceneManager);
    }
}
