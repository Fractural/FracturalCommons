using Fractural.Utils;
using Godot;
using System;

// TODO: Update the CSharp version of SceneManager with the same features from the GDScript verison.

namespace Fractural.SceneManagement
{
	// Manages transitions between scenes.
	public class SceneManager : Node
	{
		[Signal]
		public delegate void OnSceneLoaded(Node loadedScene);
		[Signal]
		public delegate void OnSceneReadied(Node readiedScene);
		[Signal]
		public delegate void OnNodeAdded(Node addedNode);
		[Signal]
		public delegate void OnNodeRemoved(Node removedNode);

		[Export]
		public bool IsSelfContained { get; set; }
		[Export]
		public bool AutoLoadInititalScene { get; set; }
		[Export]
		public PackedScene InitialScene { get; set; }
		public Node CurrentScene 
		{
			get
			{
				if (IsSelfContained)
					return currentScene;
				else
					return GetTree().CurrentScene;
			}
			set
			{
				if (IsSelfContained)
					currentScene = value;
				else
					GetTree().CurrentScene = value;
			}
		}
		private Node currentScene;
		public Node Root
		{
			get
			{
				if (IsSelfContained)
					return this;
				else
					return GetTree().Root;
			}
		}

		public override void _Ready()
		{
			GetTree().Connect("node_added", this, nameof(ListenOnNodeAdded));
			GetTree().Connect("node_removed", this, nameof(ListenOnNodeRemoved));
			if (AutoLoadInititalScene)
				GotoInitialScene();
		}

		public void GotoInitialScene()
		{
			if (InitialScene != null)
				GotoScene(InitialScene);
		}

		public void GotoScene(PackedScene scene)
		{
			CurrentScene?.QueueFree();

			Node instance = scene.Instance();

			EmitSignal(nameof(OnSceneLoaded), instance);

			Root.AddChild(instance);
			CurrentScene = instance;

			EmitSignal(nameof(OnSceneReadied), instance);
		}

		public async void TransitionToScene(PackedScene scene, PackedScene transition)
		{
			SceneTransition transitionInstance = transition.Instance<SceneTransition>();
			AddChild(transitionInstance);

			transitionInstance.TransitionIn();
			
			await ToSignal(transitionInstance, nameof(SceneTransition.OnTransitionedIn));

			CurrentScene?.QueueFree();
			Node instance = scene.Instance();

			EmitSignal(nameof(OnSceneLoaded), instance);

			Root.AddChild(instance);
			CurrentScene = instance;

			await ToSignal(transitionInstance, nameof(SceneTransition.OnTransitionedOut));

			EmitSignal(nameof(OnSceneReadied), instance);
		}

		public void GotoScene(string scene_path)
		{
			GotoScene(ResourceLoader.Load<PackedScene>(scene_path));
		}
		
		private void ListenOnNodeAdded(Node addedNode)
		{
			if (IsSelfContained && !addedNode.HasParent(this))
				return;
			
			EmitSignal(nameof(OnNodeAdded), addedNode);
		}

		private void ListenOnNodeRemoved(Node removedNode)
		{
			if (IsSelfContained && !removedNode.HasParent(this))
				return;
			
			EmitSignal(nameof(OnNodeAdded), removedNode);
		}
	}
}