using System;
using Godot;

namespace Fractural.Utils
{
	/// <summary>
	/// Utils for peeking around the Godot editor, such
	/// as digging through the tree.
	/// </summary>
	public static class EditorHackUtils
	{
		public delegate bool PrintTreeCondition(Node node);
		public delegate void WalkTreeCallback(Node node);

		public static void WalkFromTop(Node node, WalkTreeCallback callback)
		{
			// Get root node
			while (node.GetParent() != null)
			{
				node = node.GetParent();
			}
			WalkTree(node, callback);
		}

		public static void WalkTree(Node node, WalkTreeCallback callback)
		{
			callback(node);
			foreach (Node child in node.GetChildren())
				WalkTree(child, callback);
		}

		public static void PrintTree(Node node)
		{
			PrintTree(node, (currNode) => true);
		}

		public static void PrintTree(Node node, PrintTreeCondition condition)
		{
			WalkTree(node, (currNode) =>
			{
				if (condition(currNode))
					GD.Print(String.Format("Found Node at: \"{0}\"", currNode.GetPath()));
			});
		}
	}
}