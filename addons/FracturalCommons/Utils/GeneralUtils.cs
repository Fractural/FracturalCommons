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
    }
}