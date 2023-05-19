using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Fractural.Utils
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Looks in all loaded assemblies for the given type.
        /// </summary>
        /// <param name="fullName">The full name of the type.</param>
        /// <returns> The <see cref="Type"/> found; null if not found. </returns>
        public static Type FindTypeFullName(string fullName)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName.Equals(fullName));
        }

        /// <summary>
        /// Looks in all loaded assemblies for the given type
        /// </summary>
        /// <param name="name">The name of the type. Doesn't have to include the namespace.</param>
        /// <returns> The <see cref="Type"/> found; null if not found. </returns>
        public static Type FindTypeName(string name)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name.Equals(name));
        }

        /// <summary>
        /// Checks if a <paramref name="type"/> is a subclass of some <paramref name="genericBaseType"/>. <paramref name="genericArgs"/> can be optionally 
        /// passed in to check for a <paramref name="genericBaseType"/> with specific type arguments.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericBaseType"></param>
        /// <param name="genericArgs"></param>
        /// <returns></returns>
        public static bool IsSubclassOfGeneric(this Type type, Type genericBaseType, params Type[] genericArgs)
        {
            Type genericTypeInstance = GetGenericParentFromTypeDefinition(type, genericBaseType);
            return genericTypeInstance != null && (genericArgs.Length == 0 || Enumerable.SequenceEqual(type.GenericTypeArguments, genericArgs));
        }

        /// <summary>
        /// Finds and returns the parent class of <paramref name="type"/> that implmements the <paramref name="genericBaseType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericBaseType"></param>
        /// <returns></returns>
        public static Type GetGenericParentFromTypeDefinition(this Type type, Type genericBaseType)
        {
            while (type != typeof(object) && type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBaseType)
                    return type;
                type = type.BaseType;
            }
            return null;
        }
    }
}