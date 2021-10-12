using Godot;
using System;
using WAT;
using Fractural.SceneManagement;
using Fractural.Utils;

namespace Tests
{
	[Start(nameof(Initialize))]
	public class SceneManagerTests : WAT.Test
	{
		private SceneManager sceneManager;
		private PackedScene initialScene;
		private PackedScene targetScene;

		public void Initialize()
		{
			string relativePath = CSharpUtils.GetRelativePath<SceneManagerTests>();

			initialScene = ResourceLoader.Load<PackedScene>(relativePath + "/InitialScene.tscn");
			targetScene = ResourceLoader.Load<PackedScene>(relativePath + "/TargetScene.tscn");
		}

		[Test]
		public void TestInitialSceneLoading()
		{
			Describe("When readying");

			sceneManager = CSharpUtils.InstantiateCSharpNode<SceneManager>();
			sceneManager.IsSelfContained = true;
			sceneManager.AutoLoadInititalScene = true;
			sceneManager.InitialScene = initialScene;

			Assert.IsTrue(sceneManager.GetChildren().Count == 0, "Then the scene manager has no children at first.");

			AddChild(sceneManager);

			Assert.IsTrue(sceneManager.GetChildren().Count > 0, "Then the initial scene was created.");
		}
	}
}