using Godot;
using System;

public class EventLinker1 : CSharpEventLinker
{
    public override void _EnterTree()
    {
		GetNode<DummyScript>("../Dummy").CustomEvent += GetNode<DummyScriptTwo>("../DummyTwo3").TwoCustomEventListener;
    }
}
