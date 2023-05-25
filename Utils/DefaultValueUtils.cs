using Fractural.Utils;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractural.Utils
{
    public static class DefaultValueUtils
    {
        private static Dictionary<Type, DefaultValueProvider> _typeToDefaultProviderDict = new Dictionary<Type, DefaultValueProvider>();
        public static IReadOnlyDictionary<Type, DefaultValueProvider> TypeToDefaultProviderDict => _typeToDefaultProviderDict;
        static DefaultValueUtils()
        {
            var defaultValueProviderTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubclassOfGeneric(typeof(DefaultValueProvider<>)) && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null
                select type;

            foreach (var defaultValueProviderType in defaultValueProviderTypes)
            {
                Type valueProvideGgenericInstance = defaultValueProviderType.GetGenericParentFromTypeDefinition(typeof(DefaultValueProvider<>));
                Type valueType = valueProvideGgenericInstance.GetGenericArguments()[0];
                _typeToDefaultProviderDict[valueType] = Activator.CreateInstance(defaultValueProviderType) as DefaultValueProvider;
            }
        }

        public static object GetDefault(Type type, IEnumerable previousValues = null)
        {
            if (_typeToDefaultProviderDict.TryGetValue(type, out DefaultValueProvider provider))
                return provider.NextDefaultvalue(previousValues);
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        public static T GetDefault<T>(IEnumerable<T> previousValues = null)
        {
            return (_typeToDefaultProviderDict[typeof(T)] as DefaultValueProvider<T>).NextDefaultValue(previousValues);
        }
    }

    public abstract class DefaultValueProvider
    {
        public abstract Type ValueType { get; }
        public abstract object NextDefaultvalue(IEnumerable previousValues = null);
    }

    public abstract class DefaultValueProvider<T> : DefaultValueProvider
    {
        public override Type ValueType => typeof(T);
        public override object NextDefaultvalue(IEnumerable previousValues = null) => NextDefaultValue(previousValues?.Cast<T>());
        public abstract T NextDefaultValue(IEnumerable<T> previousValues = null);
    }

    public class StringDefaultProvider : DefaultValueProvider<string>
    {
        public override string NextDefaultValue(IEnumerable<string> previousValues = null)
        {
            int highestNumber = 0;
            if (previousValues != null)
            {
                foreach (var value in previousValues)
                    if (int.TryParse(value, out int intValue) && intValue > highestNumber)
                        highestNumber = intValue;
                highestNumber++;
            }
            return highestNumber.ToString();
        }
    }

    public class FloatDefaultProvider : DefaultValueProvider<float>
    {
        public override float NextDefaultValue(IEnumerable<float> previousValues = null)
        {
            float highestNumber = 0;
            if (previousValues != null)
            {
                foreach (var value in previousValues)
                    if (value > highestNumber)
                        highestNumber = value;
                highestNumber++;
            }
            return highestNumber;
        }
    }

    public class IntDefaultProvider : DefaultValueProvider<int>
    {
        public override int NextDefaultValue(IEnumerable<int> previousValues = null)
        {
            int highestNumber = 0;
            if (previousValues != null)
            {
                foreach (var value in previousValues)
                    if (value > highestNumber)
                        highestNumber = value;
                highestNumber++;
            }
            return highestNumber;
        }
    }

    public class BoolDefaultProvider : DefaultValueProvider<bool>
    {
        public override bool NextDefaultValue(IEnumerable<bool> previousValues = null)
        {
            bool trueTaken = false;
            if (previousValues != null)
            {
                foreach (var value in previousValues)
                {
                    if (value)
                        trueTaken = true;
                }
                if (trueTaken) return false;
            }
            return true;
        }
    }

    public class NodePathDefaultProvider : DefaultValueProvider<NodePath>
    {
        public override NodePath NextDefaultValue(IEnumerable<NodePath> previousValues = null)
        {
            return new NodePath();
        }
    }

    public class Vector2DefaultProvider : DefaultValueProvider<Vector2>
    {
        public override Vector2 NextDefaultValue(IEnumerable<Vector2> previousValues = null)
        {
            var prevValuesSet = new HashSet<Vector2>(previousValues ?? new Vector2[0]);
            for (int x = 0; x <= int.MaxValue; x++)
            {
                for (int y = 0; y <= int.MaxValue; y++)
                {
                    var vec = new Vector2(x, y);
                    if (!prevValuesSet.Contains(vec))
                        return vec;
                }
            }
            return Vector2.Zero;
        }
    }

    public class Vector2IntDefaultProvider : DefaultValueProvider<Vector2Int>
    {
        public override Vector2Int NextDefaultValue(IEnumerable<Vector2Int> previousValues = null)
        {
            var prevValuesSet = new HashSet<Vector2Int>(previousValues ?? new Vector2Int[0]);
            for (int x = 0; x <= int.MaxValue; x++)
            {
                for (int y = 0; y <= int.MaxValue; y++)
                {
                    var vec = new Vector2Int(x, y);
                    if (!prevValuesSet.Contains(vec))
                        return vec;
                }
            }
            return Vector2Int.Zero;
        }
    }

    public class Vector3DefaultProvider : DefaultValueProvider<Vector3>
    {
        public override Vector3 NextDefaultValue(IEnumerable<Vector3> previousValues = null)
        {
            var prevValuesSet = new HashSet<Vector3>(previousValues ?? new Vector3[0]);
            for (int x = 0; x <= int.MaxValue; x++)
            {
                for (int y = 0; y <= int.MaxValue; y++)
                {
                    for (int z = 0; z <= int.MaxValue; z++)
                    {
                        var vec = new Vector3(x, y, z);
                        if (!prevValuesSet.Contains(vec))
                            return vec;
                    }
                }
            }
            return Vector3.Zero;
        }
    }
}
