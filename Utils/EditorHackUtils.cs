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
        public delegate bool FindNodesCondition(Node node);
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

        public static void WalkTree(Node root, WalkTreeCallback callback)
        {
            callback(root);
            foreach (Node child in root.GetChildren())
                WalkTree(child, callback);
        }

        public static void PrintTree(Node root) => PrintTree(root, "", false);
        private static void PrintTree(Node root, string prefix, bool replaceLast)
        {
            GD.Print($"{prefix}--> {root.Name}");
            if (replaceLast)
                prefix = prefix.Remove(prefix.Length - 1, 1) + " ";
            if (root.GetChildCount() == 0)
                return;
            for (int i = 0; i < root.GetChildCount() - 1; i++)
                PrintTree(root.GetChild(i), $"{prefix} |", false);
            PrintTree(root.GetChild(root.GetChildCount() - 1), $"{prefix} '", true);
        }

        public static void FindNodes(Node root)
        {
            FindNodes(root, (currNode) => true);
        }

        public static void FindNodes(Node root, FindNodesCondition condition)
        {
            WalkTree(root, (currNode) =>
            {
                if (condition(currNode))
                    GD.Print(String.Format("Found Node at: \"{0}\"", currNode.GetPath()));
            });
        }
    }
}