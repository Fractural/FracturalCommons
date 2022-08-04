using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fractural.Utils
{
	/// <summary>
	/// Utility class for C# related functions.
	/// </summary>
	public static class CSharpUtils
	{
		public static Dictionary<string, string> CSharpScriptsDict { get; } = new Dictionary<string, string>();

		static CSharpUtils()
		{
			List<string> scripts = FileUtils.GetDirFiles("res://", true, new string[] { "cs" }, new string[] { ".mono" });
			foreach (string path in scripts)
				if (!CSharpScriptsDict.ContainsKey(path.GetFileName()))
					CSharpScriptsDict.Add(path.GetFileName(), path);
		}

		public static string GetRelativePath<T>()
		{
			if (CSharpScriptsDict.TryGetValue(typeof(T).Name, out string filepath))
				return filepath.GetBaseDir();
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
				return (Node) ResourceLoader.Load<CSharpScript>(filepath).New();
			return null;
		}

		public static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}

		public static string TrimSuffix(this string str, string trimmedString)
		{
			int lastIndex = str.LastIndexOf(trimmedString);
			if (lastIndex < 0)
				return str;
			return str.Substring(0, lastIndex);
		}

		public static string TrimPrefix(this string str, string trimmedString)
		{
			int index = str.IndexOf(trimmedString);
			if (index < 0)
				return str;
			return str.Substring(index + trimmedString.Length);
		}

		public static string GetFileName(this string str)
		{
			string fileName = str.GetFile();
			int periodIndex = fileName.Find(".");
			if (periodIndex < 0)
				return fileName;
			return fileName.Substring(0, periodIndex);
		}

		/// <summary>
		/// Gets the extension of a file path (excluding the period).
		/// </summary>
		/// <param name="filePath">File path as a string</param>
		/// <returns>Extension of the file (excluding the period)</returns>
		public static string GetExtension(this string filePath)
		{
			return filePath.Split('.').Last();
		}
	}
}