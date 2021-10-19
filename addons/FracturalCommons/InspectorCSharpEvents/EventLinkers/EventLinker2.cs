using Godot;
using System;

public class EventLinker2 : CSharpEventLinker
{
    public override void _EnterTree()
    {
		GetNode<DummyScript>("../Dummy").IntegerActionEvent += GetNode<DummyScriptTwo>("../DummyTwo").TwoIntegerMethod;
    }
}
