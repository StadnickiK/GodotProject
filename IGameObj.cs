using Godot;
using System;
public interface IGameObj<T>{
    [Signal]
    public delegate void SelectObject(T unit);

    [Signal]
    public delegate void SelectTargetObject(T target);

    protected T self;

    protected void _ConnectSignal();
    protected void _LoadScenes();
    protected void _LoadNodes();
}