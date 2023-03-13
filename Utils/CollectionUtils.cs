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

        public static int IndexOf<T>(this IReadOnlyList<T> list, T element)
        {
            for (int i = 0; i < list.Count; i++)
                if (EqualityComparer<T>.Default.Equals(list[i], element))
                    return i;
            return -1;
        }

        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        #region List Queue Utils
        public static T PeekFront<T>(this IList<T> list, int indexFromFront = 0)
        {
            if (list.Count < (indexFromFront + 1)) return default;
            return list[indexFromFront];
        }

        public static T PeekBack<T>(this IList<T> list, int indexFromBack = 0)
        {
            if (list.Count < (indexFromBack + 1)) return default;
            return list[list.Count - indexFromBack];
        }

        public static void PushBack<T>(this IList<T> list, T element)
        {
            list.Add(element);
        }

        public static void PushFront<T>(this IList<T> list, T element)
        {
            list.Insert(0, element);
        }

        public static T PopBack<T>(this IList<T> list)
        {
            if (list.Count == 0) return default;
            T elem = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return elem;
        }

        public static T PopFront<T>(this IList<T> list)
        {
            if (list.Count == 0) return default;
            T elem = list[0];
            list.RemoveAt(0);
            return elem;
        }
        #endregion
    }
}