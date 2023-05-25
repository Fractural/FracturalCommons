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
            set
            {
                if (Validate != null && !Validate(value))
                {
                    UpdateProperty();
                    return;
                }
                SetValue(value, true);
            }
        }

        public Func<object, bool> Validate { get; set; }

        private bool _disabled;
        public virtual bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
                OnDisabled(value);
            }
        }

        protected virtual void OnDisabled(bool disabled) { }
        private int _settingValueCounter = 0;
        public void SetValue(object value, bool triggerValueChange = false)
        {
            _value = value;
            int currCounter = ++_settingValueCounter;
            if (IsInsideTree() && triggerValueChange)
            {
                InvokeValueChanged(_value);
                if (currCounter != _settingValueCounter)
                {
                    // We called SetValue again due to the ValueChanged event, so
                    // we bail since we want only the latest setValue to run.
                    return;
                }
            }
            UpdateProperty();
        }
        public virtual void UpdateProperty() { }
        protected virtual void InvokeValueChanged(object value) => ValueChanged?.Invoke(value);
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
            set => SetValue(value, true);
        }
        public new Func<T, bool> Validate
        {
            set
            {
                if (value != null)
                    base.Validate = (obj) => value((T)obj);
            }
        }
        public void SetValue(T value, bool triggerValueChange = false) => SetValue((object)value, triggerValueChange);
        protected void InvokeValueChanged(T value) => InvokeValueChanged((object)value);
        protected override void InvokeValueChanged(object value)
        {
            ValueChanged?.Invoke((T)value);
            base.InvokeValueChanged(value);
        }
    }
}
#endif