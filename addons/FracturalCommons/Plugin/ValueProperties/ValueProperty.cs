using Fractural.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expression = System.Linq.Expressions.Expression;

#if TOOLS
namespace Fractural.Plugin
{
    [Tool]
    public abstract class ValueProperty : VBoxContainer
    {
        #region Static
        private static Dictionary<Type, Type> _typeToValuePropertyDict = new Dictionary<Type, Type>();
        public static IReadOnlyDictionary<Type, Type> TypeToValuePropertyDict => _typeToValuePropertyDict;
        public static ValueProperty CreateValueProperty(Type type) => InstanceUtils.CreateInstance(_typeToValuePropertyDict[type]) as ValueProperty;
        public static ValueProperty<T> CreateValueProperty<T>() => CreateValueProperty(typeof(T)) as ValueProperty<T>;

        static ValueProperty()
        {
            var valuePropertyTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubclassOfGeneric(typeof(ValueProperty<>)) && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null
                select type;

            foreach (var valuePropertyType in valuePropertyTypes)
            {
                Type valuePropertyGenericInstance = valuePropertyType.GetGenericParentFromTypeDefinition(typeof(ValueProperty<>));
                Type valuePropertyValueType = valuePropertyGenericInstance.GetGenericArguments()[0];
                _typeToValuePropertyDict[valuePropertyValueType] = valuePropertyType;
            }
        }
        #endregion

        public event Action<object> ValueChanged;
        public event Action<string, object> MetaChanged;
        public delegate void SetBottomEditorDelegate(Control control);
        public SetBottomEditorDelegate SetBottomEditor { get; set; }

        private object _value;
        public virtual object Value
        {
            get => _value;
            set => SetValue(value, true);
        }

        public void SetValue(object value, bool triggerValueChange = false)
        {
            if (_value != value)
            {
                _value = value;
                if (IsInsideTree() && triggerValueChange)
                    InvokeValueChanged(_value);
                UpdateProperty();
            }
        }
        public virtual void UpdateProperty() { }
        protected void InvokeValueChanged(object value) => ValueChanged?.Invoke(value);
        public new void SetMeta(string key, object value) => SetMeta(key, value, true);
        public void SetMeta(string key, object value, bool triggerMetaChanged)
        {
            if (triggerMetaChanged)
                MetaChanged?.Invoke(key, value);
            base.SetMeta(key, value);
        }

        public ValueProperty()
        {
            SetBottomEditor = (node) =>
            {
                if (node != null)
                {
                    node.Reparent(this);
                    MoveChild(node, GetChildCount() - 1);
                }
            };
        }
    }

    [Tool]
    public abstract class ValueProperty<T> : ValueProperty
    {
        public new event Action<T> ValueChanged;
        public new T Value
        {
            get => (T)base.Value;
            set
            {
                if ((base.Value == null && value != null) || !base.Value.Equals(value))
                    ValueChanged?.Invoke(value);
                base.Value = value;
            }
        }
        public void SetValue(T value, bool triggerPropertyUpdate = false) => base.SetValue(value, triggerPropertyUpdate);
        protected void InvokeValueChanged(T value) => base.InvokeValueChanged(value);
    }
}
#endif