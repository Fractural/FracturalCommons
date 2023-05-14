﻿using Fractural.Commons;
using Godot;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fractural.Utils
{
    public static class EditorUtils
    {
        public static Texture GetIconRecursive<T>(this Control controlNode)
        {
            return GetIconRecursive(controlNode, typeof(T));
        }

        public static Texture GetIconRecursive(this Control controlNode, object obj)
        {
            Type godotBaseType = obj.GetType();
            while (godotBaseType.Namespace?.Split(".").FirstOrDefault() != nameof(Godot) && godotBaseType != null)
                godotBaseType = godotBaseType.BaseType;
            return controlNode.GetIcon(godotBaseType.Name, "EditorIcons");
        }

        public static Type GetCSharpType(this Node node)
        {
            var attachedCSharpScript = (CSharpScript)node.GetScript();
            if (attachedCSharpScript != null)
                return GetCSharpType(attachedCSharpScript.ResourcePath);
            return node.GetType();
        }

        public static Type GetCSharpType(string filePath)
        {
            File file = new File();
            if (file.Open(filePath, File.ModeFlags.Read) == Error.Ok)
            {
                Regex regex = new Regex(@"namespace ([\w.]+)");
                var match = regex.Match(file.GetAsText());
                file.Close();
                var namespacePrefix = "";
                if (match.Success && match.Groups.Count > 1)
                {
                    // First matching group is the namespace of this class
                    namespacePrefix = match.Groups[1].Value + ".";
                }
                var type = Type.GetType(namespacePrefix + filePath.GetFileName());
                if (type != null)
                    return type;
            }
            return null;
        }

#if TOOLS
        public static void SoloEditorWindowPopup(this WindowDialog window, Action callPopupFunc = null)
        {
            EditorUtilsPlugin.Global.SoloWindowPopup(window, callPopupFunc);
        }

        public static void BuildCSharpSolution()
        {
            EditorUtilsPlugin.Global.BuildCSharpSolution();
        }
#endif
    }
}
