using Godot;
using System;
using System.Collections.Generic;
using GDC = Godot.Collections;

namespace Fractural.Utils
{
    /// <summary>
    /// Utility class for C# scripts in Godot
    /// </summary>
    public static class CSharpScriptUtils
    {
        public static Dictionary<string, string> CSharpScriptsDict { get; } = new Dictionary<string, string>();
        public const string CSHARP_SCRIPTS_TABLE_PATH = "res://CSharpScriptsTable.json";

        public static void GenerateCSharpScriptsTable(bool overwrite = false)
        {
            File file = new File();
            if (file.FileExists(CSHARP_SCRIPTS_TABLE_PATH) && !overwrite)
                return;
            List<string> scripts = FileUtils.GetDirFiles("res://", true, new string[] { "cs" }, new string[] { ".mono" });
            file.Open(CSHARP_SCRIPTS_TABLE_PATH, File.ModeFlags.Write);
            file.StoreString(JSON.Print(scripts));
            file.Close();
        }

        static CSharpScriptUtils()
        {
            File file = new File();
            if (file.Open(CSHARP_SCRIPTS_TABLE_PATH, File.ModeFlags.Read) == Error.Ok)
            {
                var scripts = (GDC.Array)JSON.Parse(file.GetAsText()).Result;
                foreach (string path in scripts)
                    if (!CSharpScriptsDict.ContainsKey(path.GetFileName()))
                        CSharpScriptsDict.Add(path.GetFileName(), path);
            }
            else
            {
                GD.PushWarning("CSharpScriptUtils requires the CSharp scripts table to be generated. Please enable the option in the FracturalCommons project settings and then restart the plugin once in the editor to generate this file.");
            }
        }

        public static string GetPath<T>()
        {
            if (CSharpScriptsDict.TryGetValue(typeof(T).Name, out string filepath))
                return filepath;
            return null;
        }

        public static T InstantiateCSharpNode<T>() where T : Node
        {
            if (CSharpScriptsDict.TryGetValue(typeof(T).Name, out string filepath))
                return (T)ResourceLoader.Load<CSharpScript>(filepath).New();
            return null;
        }

        public static Node InstantiateCSharpNode(Type type)
        {
            if (CSharpScriptsDict.TryGetValue(type.Name, out string filepath))
                return (Node)ResourceLoader.Load<CSharpScript>(filepath).New();
            return null;
        }
    }
}