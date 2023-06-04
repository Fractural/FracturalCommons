using System;
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
            int code = objects[0].GetHashCode();
            for (int i = 1; i < objects.Length; i++)
                code = ((code << 5) + code) ^ (objects[i]?.GetHashCode() ?? 0);
            return code;
        }
    }
}