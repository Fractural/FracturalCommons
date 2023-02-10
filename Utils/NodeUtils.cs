using Godot;
using System;
using System.Collections.Generic;

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
		/// Change all the children of node to be owned by the node.
		/// </summary>
		/// <param name="node"></param>
		public static void OwnSubtree(this Node node)
		{
			Queue<Node> nodes = new Queue<Node>();
			nodes.Enqueue(node);
			while (nodes.Count > 0)
			{
				var queuedNode = nodes.Dequeue();
				if (queuedNode != node && queuedNode.Owner != node)
					queuedNode.Owner = node;
				foreach (Node child in queuedNode.GetChildren())
					nodes.Enqueue(child);
			}
		}

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
		public static Node Reparent(this Node node, Node newParent, bool keepGlobalTransform = true)
		{
			Node originalParent = node.GetParent();
			Transform2D globalTransform2D = default;
			Transform globalTransform = default;
			if (keepGlobalTransform)
			{
				if (node is Node2D node2D)
					globalTransform2D = node2D.GlobalTransform;
				else if (node is Spatial spatial)
					globalTransform = spatial.GlobalTransform;
			}
			if (originalParent != null)
				node.GetParent().RemoveChild(node);
			newParent.AddChild(node);
			if (keepGlobalTransform)
			{
				if (node is Node2D node2D)
					node2D.GlobalTransform = globalTransform2D;
				else if (node is Spatial spatial)
					spatial.GlobalTransform = globalTransform;
			}
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

		/// <summary>
		/// Gets the first ancestor of type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Specified type to search for</typeparam>
		/// <param name="node">Root node</param>
		/// <param name="includeRoot">Should the root be included in the search</param>
		/// <returns>Ancestor is found. Null otherwise.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T GetAncestor<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			if (includeRoot && node is T rootCasted)
				return rootCasted;
			node = node.GetParent();
			while (node != null)
			{
				if (node is T casted)
					return casted;
				node = node.GetParent();
			}
			return default(T);
		}

		/// <summary>
		/// Gets all ancestors of type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Specified type to search for</typeparam>
		/// <param name="node">Root node</param>
		/// <param name="includeRoot">Should the root be included in the search</param>
		/// <returns>Ancestor is found. Null otherwise.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T[] GetAncestors<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			List<T> results = new List<T>();
			if (includeRoot && node is T rootCasted)
				results.Add(rootCasted);
			node = node.GetParent();
			while (node != null)
			{
				if (node is T casted)
					results.Add(casted);
				node = node.GetParent();
			}
			return results.ToArray();
		}

		/// <summary>
		/// Gets the first descendant of type <typeparamref name="T"/> using a breadth first search.
		/// </summary>
		/// <typeparam name="T">Specified type to search for</typeparam>
		/// <param name="node">Root node</param>
		/// <param name="includeRoot">Should the root be included in the search</param>
		/// <returns>Ancestor is found. Null otherwise.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T GetDescendant<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			Queue<Node> nodes = new Queue<Node>();
			if (includeRoot)
				nodes.Enqueue(node);
			else
			{
				foreach (Node child in node.GetChildren())
					nodes.Enqueue(child);
			}

			do
			{
				var currNode = nodes.Dequeue();
				if (currNode is T castedResult) return castedResult;
				foreach (Node child in currNode.GetChildren())
					nodes.Enqueue(child);
			} while (nodes.Count > 0);
			return default(T);
		}

		/// <summary>
		/// Gets all descendants of a type. By default, it includes the root node in the search.
		/// </summary>
		/// <typeparam name="T">Specified type to search for</typeparam>
		/// <param name="node">Root node</param>
		/// <param name="includeRoot">Should the root node be included in the search</param>
		/// <returns>List of descendants of type T</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T[] GetDescendants<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			List<T> results = new List<T>();
			Queue<Node> nodes = new Queue<Node>();
			if (includeRoot)
				nodes.Enqueue(node);
			else
			{
				foreach (Node child in node.GetChildren())
					nodes.Enqueue(child);
			}

			do
			{
				var currNode = nodes.Dequeue();
				if (currNode is T castedResult) results.Add(castedResult);
				foreach (Node child in currNode.GetChildren())
					nodes.Enqueue(child);
			} while (nodes.Count > 0);
			return results.ToArray();
		}

		/// <summary>
		/// Gets the first instance of type <typeparamref name="T"/> that is a sibiling of <paramref name="node"/>.
		/// </summary>
		/// <typeparam name="T">Type of node that is being queried for.</typeparam>
		/// <param name="node">Target node to query.</param>
		/// <param name="includeRoot">Option to include the <paramref name="node"/> in the checks. This is true by default.</param>
		/// <returns>Node of type <typeparamref name="T"/> that is <paramref name="node"/> or a sibling of <paramref name="node"/></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static T GetSibiling<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			if (node.GetParent() == null) throw new ArgumentException("Expected node to have parent.");
			if (includeRoot && node is T castedRoot)
				return castedRoot;
			else
			{
				foreach (Node child in node.GetParent().GetChildren())
					if (child is T castedChild) return castedChild;
			}
			return default(T);
		}

		/// <summary>
		/// Gets all instances of type <typeparamref name="T"/> that is a sibiling of <paramref name="node"/>.
		/// </summary>
		/// <typeparam name="T">Type of node that is being queried for.</typeparam>
		/// <param name="node">Target node to query.</param>
		/// <param name="includeRoot">Option to include the <paramref name="node"/> in the checks. This is true by default.</param>
		/// <returns>Node of type <typeparamref name="T"/> that is <paramref name="node"/> or a sibling of <paramref name="node"/></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static T[] GetSiblings<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			if (node.GetParent() == null) throw new ArgumentException("Expected node to have parent.");
			List<T> results = new List<T>();
			if (includeRoot && node is T castedRoot)
				results.Add(castedRoot);
			foreach (Node child in node.GetParent().GetChildren())
				if (child is T castedChild) results.Add(castedChild);
			return results.ToArray();
		}

		/// <summary>
		/// Gets the first instance of type <typeparamref name="T"/> that is a child of <paramref name="node"/>.
		/// </summary>
		/// <typeparam name="T">Type of node that is being queried for.</typeparam>
		/// <param name="node">Target node to query.</param>
		/// <param name="includeRoot">Option to include the <paramref name="node"/> in the checks. This is true by default.</param>
		/// <returns>Node of type <typeparamref name="T"/> that is <paramref name="node"/> or an immediate child of <paramref name="node"/></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T GetImmediateChild<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			if (includeRoot && node is T castedRoot)
				return castedRoot;
			else
			{
				foreach (Node child in node.GetChildren())
					if (child is T castedChild) return castedChild;
			}
			return default(T);
		}

		/// <summary>
		/// Gets all instances of type <typeparamref name="T"/> that are children of <paramref name="node"/>.
		/// </summary>
		/// <typeparam name="T">Type of node that is being queried for.</typeparam>
		/// <param name="node">Target node to query.</param>
		/// <param name="includeRoot">Option to include the <paramref name="node"/> in the checks. This is true by default.</param>
		/// <returns>Node of type <typeparamref name="T"/> that is <paramref name="node"/> or an immediate child of <paramref name="node"/></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T[] GetImmediateChildren<T>(this Node node, bool includeRoot = true)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			List<T> results = new List<T>();
			Queue<Node> nodes = new Queue<Node>();
			if (includeRoot && node is T castedRoot)
				results.Add(castedRoot);
			foreach (Node child in node.GetChildren())
				if (child is T castedChild) results.Add(castedChild);
			return results.ToArray();
		}

		public static bool HasImmediateChild<T>(this Node node, bool includeRoot = true)
		{
			return GetImmediateChild<T>(node, includeRoot) != null;
		}

		public static bool HasSibiling<T>(this Node node, bool includeRoot = true)
		{
			return GetSibiling<T>(node, includeRoot) != null;
		}

		public static bool HasDescendant<T>(this Node node, bool includeRoot = true)
		{
			return GetDescendant<T>(node, includeRoot) != null;
		}

		public static bool HasAncestor<T>(this Node node, bool includeRoot = true)
		{
			return GetAncestor<T>(node, includeRoot) != null;
		}

		public static bool TryGetImmediateChild<T>(this Node node, out T result, bool includeRoot = true)
		{
			result = GetImmediateChild<T>(node, includeRoot);
			return result != null;
		}

		public static bool TryGetSibling<T>(this Node node, out T result, bool includeRoot = true)
		{
			result = GetSibiling<T>(node, includeRoot);
			return result != null;
		}

		public static bool TryGetDescendant<T>(this Node node, out T result, bool includeRoot = true)
		{
			result = GetDescendant<T>(node, includeRoot);
			return result != null;
		}

		public static bool TryGetAncestor<T>(this Node node, out T result, bool includeRoot = true)
		{
			result = GetAncestor<T>(node, includeRoot);
			return result != null;
		}

		public static T AddImmediateChild<T>(this Node node) where T : Node, new()
		{
			T child = new T();
			node.AddChild(child);
			return child;
		}

		public static T GetOrAddImmediateChild<T>(this Node node) where T : Node, new()
		{
			T child = GetImmediateChild<T>(node);
			if (child == null)
				child = AddImmediateChild<T>(node);
			return child;
		}
	}
}
