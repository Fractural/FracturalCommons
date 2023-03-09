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