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
		public static void SearchTree(Node parent, PrintTreeCondition condition)
		{
			if (condition(parent))
				GD.Print(String.Format("Found Node at: \"{0}\"", parent.GetPath()));
			foreach (Node child in parent.GetChildren())
				SearchTree(child, condition);
		}
	}
}