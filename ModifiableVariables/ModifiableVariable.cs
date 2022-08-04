using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoCustomResourceRegistry;

namespace Fractural
{
    [RegisteredType("ModifiableVariable", "res://addons/FracturalCommons/ModifiableVariables/icon_edit_key.svg")]
    public class ModifiableVariable<T> : Node
    {
        public class ChangeEventArgs
        {
            public ChangeEventArgs(T newValue, T originalValue)
            {
                NewValue = newValue;
                OriginalValue = originalValue;
            }

            public T NewValue { get; set; }

            public T OriginalValue { get; set; }
        }

        public class ValidateEventArgs
        {
            public ValidateEventArgs(T newValue, bool @override)
            {
                NewValue = newValue;
                Override = @override;
            }

            public T NewValue { get; set; }

            public bool Override { get; set; }
        }

        public Action<ValidateEventArgs> OnValidate;

        public Action<ChangeEventArgs> OnChange;

        public T Value
        {
            get
            {
                return valueField;
            }

            set
            {
                ValidateEventArgs validateEventArgs = new ValidateEventArgs(valueField, false);

                OnValidate.Invoke(validateEventArgs);

                if (!validateEventArgs.Override)
                {
                    T oldValue = valueField;
                    
                    valueField = value;

                    OnChange.Invoke(new ChangeEventArgs(valueField, oldValue));
                }
            }
        }

        [Export]
        private T valueField;

        /// <summary>
        /// Initializes a modifiable variable with a value.
        /// </summary>
        /// <param name="value"></param>
        public void Init(T value)
        {
            valueField = value;
		}
    }
}