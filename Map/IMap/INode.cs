using Godot;
using System;

public interface INode
{
    public StringName Name { get; set; }

    public Node GetParent();

    public Node GetNode(NodePath path);

    public void QueueFree();

    //public T GetNode<T>(NodePath path);
}
