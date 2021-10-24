using Fractural.FastEvent;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoCustomResourceRegistry;

namespace Fractural
{
    [RegisteredType("ModifiableVariable", "res://addons/FracturalCommons/Misc/ModifiableVariables/icon_edit_key.svg")]
    public class ModifiableVariable<T> : Node
    {
        public class ChangeEventArgs<T>
        {
            public ChangeEventArgs(T newValue, T originalValue)
            {
                NewValue = newValue;
                OriginalValue = originalValue;
            }

            public T NewValue { get; set; }

            public T OriginalValue { get; set; }
        }

        public class ValidateEventArgs<T>
        {
            public ValidateEventArgs(T newValue, bool @override)
            {
                NewValue = newValue;
                Override = @override;
            }

            public T NewValue { get; set; }

            public bool Override { get; set; }
        }

        public Action<ValidateEventArgs<T>> OnValidate;

        public Action<ChangeEventArgs<T>> OnChange;

        public T Value
        {
            get
            {
                return valueField;
            }

            set
            {
                ValidateEventArgs<T> validateEventArgs = new ValidateEventArgs<T>(valueField, false);

                OnValidate.Invoke(validateEventArgs);

                if (!validateEventArgs.Override)
                {
                    T oldValue = valueField;
                    
                    valueField = value;

                    OnChange.Invoke(new ChangeEventArgs<T>(valueField, oldValue));
                }
            }
        }

        [Export]
        private T valueField;
    }
}