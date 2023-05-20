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
}
