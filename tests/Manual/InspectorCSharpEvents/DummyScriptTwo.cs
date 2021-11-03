using Godot;
using System;
using DummyNamespace;

namespace DummyNamespaceTwo
{
    public class DummyScriptTwo : Node
    {
        public void TwoOnIntegerEvent(int integer)
        {
            GD.Print($"{nameof(TwoOnIntegerEvent)}({integer})");
        }

        public void TwoOnEmptyEvent()
        {
            GD.Print($"{nameof(TwoOnEmptyEvent)}()");
        }

        public void TwoOnEventHandlerEvent(object sender, EventArgs args)
        {
            GD.Print($"{nameof(TwoOnEmptyEvent)}({sender}, {args})");
        }

        public void TwoOnSomeArgsEventHandlerEvent(object sender, DummyScript.SomeArgs args)
        {
            GD.Print($"{nameof(TwoOnEmptyEvent)}({sender}, {args})");
        }
    }
}