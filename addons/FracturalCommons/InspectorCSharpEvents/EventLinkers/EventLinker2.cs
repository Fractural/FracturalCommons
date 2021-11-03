using Godot;
using System;

public class EventLinker2 : CSharpEventLinker
{
    public override void _EnterTree()
    {
		GetNode<DummyNamespace.DummyScript>("../Dummy").EmptyActionEvent += GetNode<DummyNamespace.DummyScript>("../DummyTwo3/DummyChild").OnEmptyEvent;
		GetNode<DummyNamespace.DummyScript>("../Dummy3").IntegerActionEvent += GetNode<DummyNamespaceTwo.DummyScriptTwo>("../DummyTwo3").TwoOnIntegerEvent;
    }
}
