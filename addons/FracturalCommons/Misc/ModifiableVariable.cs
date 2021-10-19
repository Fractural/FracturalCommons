using Fractural.FastEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractural
{
    public class ModifiableVariable<T>
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

        public FastEvent<ValidateEventArgs<T>> OnValidate { get; set; } = new FastEvent<ValidateEventArgs<T>>();

        public FastEvent<ChangeEventArgs<T>> OnChange { get; set; } = new FastEvent<ChangeEventArgs<T>>();

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

        private T valueField;
    }
}