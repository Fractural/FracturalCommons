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

        public static void Reverse<T>(this IList<T> list)
        {
            int zeroIndexedCount = list.Count - 1;
            int halfOfCount = list.Count / 2;
            for (int i = 0; i < halfOfCount; i++)
            {
                var temp = list[i];
                list[i] = list[zeroIndexedCount - i];
                list[zeroIndexedCount - i] = temp;
            }
        }

        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static T ElementAt<T>(this IList array, params int[] indices)
        {
            for (int i = 0; i < indices.Length - 1; i++)
                array = (IList)array[indices[i]];
            return (T)array[indices[indices.Length - 1]];
        }

        public static bool Contains<T>(this IList<T> list, T value) => Contains(list as IList, value);
        public static bool Contains(this IList list, object value)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i]?.Equals(value) ?? false)
                    return true;
            return false;
        }

        public static bool IndexEquals<T>(this IList<T> list, int index, T value) => IndexEquals(list as IList, index, value);
        public static bool IndexEquals(this IList list, int index, object value)
        {
            if (list.Count <= index || index < 0)
                return false;
            return list[index]?.Equals(value) ?? false;
        }

        public static T After<T>(this IList<T> list, T obj, int index)
        {
            object result = After(list as IList, obj, index);
            if (result is T instance)
                return instance;
            return default(T);
        }
        public static object After(this IList list, object obj, int index)
        {
            int objIndex = list.IndexOf(obj);
            int offsetIndex = objIndex + index;
            if (list.Count <= offsetIndex || offsetIndex < 0)
                return null;
            return list[offsetIndex];
        }

        public static IList<T> Slice<T>(this IList<T> list, int begin, int end = int.MaxValue, int step = 1)
        {
            var newList = new List<T>();
            for (int i = begin; i < list.Count && i < end; i++)
                newList.Add(list[i]);
            return newList;
        }
        public static T[] Slice<T>(this T[] list, int begin, int end = int.MaxValue, int step = 1)
        {
            if (end > list.Length)
                end = list.Length;
            var newList = new T[end - begin];
            for (int i = begin; i < end; i++)
                newList[i - begin] = list[i];
            return newList;
        }

        #region IReadonlyList<T> Queue Utils
        public static T PeekFrontReadonly<T>(this IReadOnlyList<T> list, int indexFromFront = 0)
        {
            if (indexFromFront < 0 || list.Count < (indexFromFront + 1)) return default;
            return list[indexFromFront];
        }

        public static T PeekBackReadonly<T>(this IReadOnlyList<T> list, int indexFromBack = 0)
        {
            if (indexFromBack < 0 || list.Count < (indexFromBack + 1)) return default;
            return list[list.Count - 1 - indexFromBack];
        }
        #endregion

        #region IList<T> Queue Utils
        public static T PeekFront<T>(this IList<T> list, int indexFromFront = 0)
        {
            if (indexFromFront < 0 || list.Count < (indexFromFront + 1)) return default;
            return list[indexFromFront];
        }

        public static T PeekBack<T>(this IList<T> list, int indexFromBack = 0)
        {
            if (indexFromBack < 0 || list.Count < (indexFromBack + 1)) return default;
            return list[list.Count - 1 - indexFromBack];
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

        #region IList Queue Utils
        public static T PeekFrontList<T>(this IList list, int indexFromFront = 0)
        {
            if (indexFromFront < 0 || list.Count < (indexFromFront + 1)) return default;
            return (T)list[indexFromFront];
        }

        public static T PeekBackList<T>(this IList list, int indexFromBack = 0)
        {
            if (indexFromBack < 0 || list.Count < (indexFromBack + 1)) return default;
            return (T)list[list.Count - 1 - indexFromBack];
        }

        public static void PushBackList<T>(this IList list, T element)
        {
            list.Add(element);
        }

        public static void PushFrontList<T>(this IList list, T element)
        {
            list.Insert(0, element);
        }

        public static T PopBackList<T>(this IList list)
        {
            if (list.Count == 0) return default;
            T elem = (T)list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return elem;
        }

        public static T PopFrontList<T>(this IList list)
        {
            if (list.Count == 0) return default;
            T elem = (T)list[0];
            list.RemoveAt(0);
            return elem;
        }
        #endregion
    }
}