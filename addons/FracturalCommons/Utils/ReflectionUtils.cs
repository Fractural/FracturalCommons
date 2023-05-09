using System;
using System.Linq;

namespace Fractural.Utils
{
    public class ReflectionUtils
    {
        /// <summary>
        /// Looks in all loaded assemblies for the given type.
        /// </summary>
        /// <param name="fullName">The full name of the type.</param>
        /// <returns> The <see cref="Type"/> found; null if not found. </returns>
        private static Type FindTypeFullName(string fullName)
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
        private static Type FindTypeName(string name)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name.Equals(name));
        }
    }
}