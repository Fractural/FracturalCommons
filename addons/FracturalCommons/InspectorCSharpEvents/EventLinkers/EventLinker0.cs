using Godot;
using System;

public class EventLinker0 : CSharpEventLinker
{
    public override void _EnterTree()
    {
		GetNode<DummyScript>("../Dummy").IntegerActionEvent += GetNode<DummyScript>("../Dummy3").PropagateNotification;
    }
}
