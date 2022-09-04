using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GDC = Godot.Collections;

namespace Fractural.Utils
{
	/// <summary>
	/// Utility class for collections.
	/// </summary>
	public static class CollectionUtils
	{
		#region C# Collections
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

		public static string ToElementsString(this Array array)
		{
			var result = new StringBuilder();
			result.Append("[");
			for (int i = 0; i < array.Length; i++)
			{
				result.Append(array.GetValue(i).ToString());
				if (i < array.Length - 1)
					result.Append(", ");
			}
			result.Append("]");
			return result.ToString();
		}
		#endregion

		#region Godot Collections
		public static int FindIndex<T>(this GDC.Array<T> array, System.Predicate<T> predicate)
		{
			for (int i = 0; i < array.Count; i++)
				if (predicate(array[i]))
					return i;
			return -1;
		}

		public static void ForEach<T>(this GDC.Array<T> array, Action<T> action)
		{
			foreach (var element in array)
				action(element);
		}

		public static void AddRange<T>(this GDC.Array<T> array, IEnumerable<T> enumerable)
		{
			foreach (var element in enumerable)
				array.Add(element);
		}

		public static T[] ToArray<T>(this GDC.Array<T> array)
		{
			var csharpArray = new T[array.Count];
			for (int i = 0; i < array.Count; i++)
				csharpArray[i] = array[i];
			return csharpArray;
		}

		public static GDC.Dictionary ToGDDict(this object obj)
		{
			GDC.Dictionary dict = new GDC.Dictionary();
			foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				dict[prop.Name] = prop.GetValue(obj, null);
			}
			return dict;
		}
		#endregion
	}
}