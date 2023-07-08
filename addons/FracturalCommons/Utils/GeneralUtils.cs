using System;
using System.Collections;
using System.Linq;

namespace Fractural.Utils
{
    public static class GeneralUtils
    {
        public static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        public static int CombineHashCodes(params object[] objects)
        {
            if (objects == null || objects.Length == 0)
                return 0;
            int code = GetHashCode(objects[0]);
            for (int i = 1; i < objects.Length; i++)
                code = ((code << 5) + code) ^ GetHashCode(objects[i]);
            return code;
        }

        public static int GetHashCode(object obj)
        {
            int code = 0;
            if (obj is IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys)
                    code = CombineHashCodes(code, dictionary[key]);
            }
            else if (obj is IEnumerable enumerable)
            {
                foreach (var elem in enumerable)
                    code = CombineHashCodes(code, elem.GetHashCode());
            }
            else
            {
                code = obj.GetHashCode();
            }
            return code;
        }

        public static T TypedClone<T>(this T obj) where T : ICloneable
        {
            return (T)obj.Clone();
        }
    }
}