using Godot;

/// <summary>
/// Utilities used by Fractural Studios.
/// </summary>
namespace Fractural.Utils
{
	/// <summary>
	/// Utilities for Godot Nodes.
	/// </summary>
	public static class NodeUtils
	{
		/// <summary>
		/// Checks if a given node is currently in the
		/// editor scene tab.
		/// Tested in Godot v3.4.
		/// </summary>
		/// <param name="node">Node being checked</param>
		/// <returns>True if the Node has the editor's scene tab as one of its parents.</returns>
		public static bool IsInEditorSceneTab(Node node)
		{
			if (Engine.EditorHint)
			{
				while (node.GetParent() != null)
				{
					node = node.GetParent();
					if (node is Viewport && node.GetParent() is ViewportContainer && node.GetParent()?.GetParent() is Control)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Reparents "node" to have "newParent" as it's parent.
		/// </summary>
		/// <param name="node">Node being reparented</param>
		/// <param name="newParent">New parent for "node"</param>
		/// <returns>Original parent of "node"</returns>
		public static Node Reparent(this Node node, Node newParent)
		{
			Node originalParent = node.GetParent();
			if (originalParent != null)
				node.GetParent().RemoveChild(node);
			newParent.AddChild(node);
			return originalParent;
		}

		/// <summary>
		/// Checks if a Node has as parent.
		/// </summary>
		/// <param name="node">Node being checked</param>
		/// <param name="parent">Parent expected in "node"</param>
		/// <returns>True if "node" has "parent" as one of it's parents</returns>
		public static bool HasParent(this Node node, Node parent)
		{
			// None recursive solution to improve performance.
			while (node.GetParent() != null)
			{
				if (node.GetParent() == parent)
					return true;
				node = node.GetParent();
			}
			return false;
		}

		/// <summary>
		/// Gets a singleton from a SceneTree.
		/// </summary>
		/// <param name="tree">SceneTree containing the singleton</param>
		/// <param name="name">Name of the singleton.</param>
		/// <returns>Singleton if found. Otherwise returns null.</returns>
		public static Node GetSingleton(this SceneTree tree, string name)
		{
			if (tree.Root.HasNode(name))
				return tree.Root.GetNode(name);
			return null;
		}

		/// <summary>
		/// Adds a singleton to a SceneTree.
		/// </summary>
		/// <param name="tree">SceneTree that's receiving the singleton</param>
		/// <param name="singleton">Singleton being added</param>
		/// <typeparam name="T">Singleton's type</typeparam>
		public static void AddSingleton<T>(this SceneTree tree, T singleton) where T : Node
		{
			AddSingleton(tree, singleton, nameof(T));
		}

		/// <summary>
		/// Adds a singleton to a SceneTree.
		/// </summary>
		/// <param name="tree">SceneTree that's recieving the singleton</param>
		/// <param name="singleton">Singleton being added</param>
		/// <param name="name">Name of the singleton</param>
		public static void AddSingleton(this SceneTree tree, Node singleton, string name)
		{
			singleton.Name = name;
			tree.Root.AddChild(singleton);
		}

		/// <summary>
		/// Removes a singleton from a SceneTree.
		/// </summary>
		/// <param name="tree">SceneTree that's removing the singleton</param>
		/// <param name="name">Name of the singleton</param>
		public static void RemoveSingleton(this SceneTree tree, string name)
		{
			if (tree.Root.HasNode(name))
				tree.Root.GetNode(name).QueueFree();
		}
	}
}