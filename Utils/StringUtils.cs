using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDC = Godot.Collections;

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

        public static string Format(this string text, GDC.Dictionary dictionary)
        {
            var charArray = text.ToCharArray();
            Queue<string> nonVariableSegments = new Queue<string>();
            Queue<string> foundVariables = new Queue<string>();

            StringBuilder currentString = new StringBuilder();
            for (int i = 0; i < charArray.Length; i++)
            {
                // If we encountered an '{' and the next character isn't an '{', then
                // we've found the start of a variable.
                if (charArray[i] == '{')
                {
                    if ((i < charArray.Length - 1 && charArray[i + 1] == '{'))
                    {
                        currentString.Append('{');
                        i += 1;
                    }
                    else
                    {
                        // First segmenet is always nonVariable (even if it's empty)
                        nonVariableSegments.Enqueue(currentString.ToString());
                        // Reset the current string segmenet in preparation for reading the variable
                        currentString.Clear();
                    }
                }
                else if (charArray[i] == '}')
                {
                    if ((i < charArray.Length - 1 && charArray[i + 1] == '}'))
                    {
                        currentString.Append('}');
                        i += 1;
                    }
                    else
                    {
                        foundVariables.Enqueue(currentString.ToString().Trim());
                        currentString.Clear();
                    }
                }
                else
                {
                    currentString.Append(charArray[i]);
                }
            }
            nonVariableSegments.Enqueue(currentString.ToString());

            StringBuilder result = new StringBuilder();
            while (nonVariableSegments.Count > 0 || foundVariables.Count > 0)
            {
                if (nonVariableSegments.Count > 0)
                    result.Append(nonVariableSegments.Dequeue());
                if (foundVariables.Count > 0)
                {
                    object value = dictionary.Get<object>(foundVariables.Dequeue());
                    if (value != null)
                        result.Append(value.ToString());
                }
            }
            return result.ToString();
        }
    }
}