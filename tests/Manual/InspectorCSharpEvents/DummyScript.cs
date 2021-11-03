using Godot;
using System;

namespace DummyNamespace
{
    public class DummyScript : Node
    {
        public class SomeArgs : EventArgs
        {
            public int IntegerValue { get; set; }
            public string StringValue { get; set; }

            public override string ToString()
            {
                return $"{IntegerValue}, {StringValue}";
            }
        }

        public event Action<int> IntegerActionEvent;
        public event Action EmptyActionEvent;

        public event EventHandler EventHandlerEvent;
        public event EventHandler<SomeArgs> SomeArgsEventHandlerEvent;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            IntegerActionEvent?.Invoke(10);
            EmptyActionEvent?.Invoke();
            EventHandlerEvent?.Invoke(this, new EventArgs());
            SomeArgsEventHandlerEvent?.Invoke(this, new SomeArgs() { IntegerValue = 430, StringValue = "asdf" });
        }

        public void OnIntegerEvent(int integer)
        {
            GD.Print($"{nameof(OnIntegerEvent)}({integer})");
        }

        public void OnEmptyEvent()
        {
            GD.Print($"{nameof(OnEmptyEvent)}()");
        }

        public void OnEventHandlerEvent(object sender, EventArgs args)
        {
            GD.Print($"{nameof(OnEmptyEvent)}({sender}, {args})");
        }

        public void OnSomeArgsEventHandlerEvent(object sender, SomeArgs args)
        {
            GD.Print($"{nameof(OnEmptyEvent)}({sender}, {args})");
        }
    }
}
