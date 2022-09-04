using Godot;
using System.Linq;

namespace Fractural.Utils
{
	public static class StringUtils
	{
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