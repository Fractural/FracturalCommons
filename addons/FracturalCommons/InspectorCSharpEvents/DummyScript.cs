using Godot;
using System;

public class DummyScript : Node
{
    public class CustomEventArgs : EventArgs { }
    public delegate void CustomEventHandler(object sender, CustomEventArgs args);

    public event CustomEventHandler CustomEvent;
    public event EventHandler DefaultEvent;

    public event Action<int> IntegerActionEvent;
    public event Action NormalActionEvent;
    private event Action PrivateActionEvent;
    protected event Action ProtectedActionEvent;

}
