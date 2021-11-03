using Godot;
using System;

public class EventLinker3 : CSharpEventLinker
{
    public override void _EnterTree()
    {
		GetNode<DummyNamespace.DummyScript>("../Dummy").IntegerActionEvent += GetNode<DummyNamespace.DummyScript>("../DummyTwo3/DummyChild").OnIntegerEvent;
    }
}
