using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fractural.Utils
{
    /// <summary>
    /// Utility class for collections.
    /// </summary>
    public static class CollectionUtils
    {
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

        public static bool TryGet<T>(this IReadOnlyList<T> array, int index, out T result, T defaultReturn = default(T))
        {
            if (array.Count > index)
            {
                result = array[index];
                return true;
            }
            result = defaultReturn;
            return false;
        }

        public static T TryGet<T>(this IReadOnlyList<T> list, int index, T defaultReturn = default(T))
        {
            TryGet<T>(list, index, out T result, defaultReturn);
            return result;
        }

        public static int Count(this IEnumerable enumerable)
        {
            int count = 0;
            foreach (var elem in enumerable)
                count++;
            return count;
        }

        public static string ToElementsString(this IEnumerable enumerable)
        {
            StringBuilder builder = new StringBuilder();
            int count = enumerable.Count();
            int index = 0;
            builder.Append("[");
            foreach (var elem in enumerable)
            {
                builder.Append(elem.ToString());
                if (index < count - 1)
                    builder.Append(", ");
                index++;
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}