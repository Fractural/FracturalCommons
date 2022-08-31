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
				return (Node)ResourceLoader.Load<CSharpScript>(filepath).New();
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

		public static void Populate<T>(this T[] arr, T value)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = value;
			}
		}

		public static R[,] Select<T, R>(this T[,] items, Func<T, R> f)
		{
			int d0 = items.GetLength(0);
			int d1 = items.GetLength(1);
			R[,] result = new R[d0, d1];
			for (int i0 = 0; i0 < d0; i0 += 1)
				for (int i1 = 0; i1 < d1; i1 += 1)
					result[i0, i1] = f(items[i0, i1]);
			return result;
		}

		public static int ReverseBits(this int target, int numberOfBits = sizeof(int) * 8)
		{
			int reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1 << i)) != 0 ? 1 << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static long ReverseBits(this long target, int numberOfBits = sizeof(long) * 8)
		{
			long reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1L << i)) != 0 ? 1L << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static uint ReverseBits(this uint target, int numberOfBits = sizeof(uint) * 8)
		{
			uint reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1U << i)) != 0 ? 1U << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static ulong ReverseBits(this ulong target, int numberOfBits = sizeof(ulong) * 8)
		{
			ulong reversed = 0;
			for (int i = 0; i < numberOfBits; i++)
			{
				// If the ith bit of x is toggled, toggle the ith bit from the right of reversed
				reversed |= (target & (1UL << i)) != 0 ? 1UL << (numberOfBits - 1 - i) : 0;
			}
			return reversed;
		}

		public static T TryGet<T>(this T[] array, int index, T defaultReturn = default(T))
		{
			TryGet<T>(array, index, out T result, defaultReturn);
			return result;
		}

		public static bool TryGet<T>(this T[] array, int index, out T result, T defaultReturn = default(T))
		{
			if (array.Length > index)
			{
				result = array[index];
				return true;
			}
			result = defaultReturn;
			return false;
		}

		public static T TryGet<T>(this List<T> list, int index, T defaultReturn = default(T))
		{
			TryGet<T>(list, index, out T result, defaultReturn);
			return result;
		}

		public static bool TryGet<T>(this List<T> list, int index, out T result, T defaultReturn = default(T))
		{
			if (list.Count > index)
			{
				result = list[index];
				return true;
			}
			result = defaultReturn;
			return false;
		}

		public static bool IsInstanceOfGenericType(this object obj, Type genericType, params Type[] genericTypeArgs)
		{
			return IsGenericType(obj.GetType(), genericType, genericTypeArgs);
		}

		public static bool IsGenericType(this Type type, Type genericType, params Type[] genericTypeArgs)
		{
			while (type != null)
			{
				if (type.IsGenericType &&
					type.GetGenericTypeDefinition() == genericType)
				{
					return Enumerable.SequenceEqual(type.GenericTypeArguments, genericTypeArgs);
				}
				type = type.BaseType;
			}
			return false;
		}
	}
}