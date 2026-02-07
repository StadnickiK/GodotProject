using Godot;
using System;

public interface INode
{
    public StringName Name { get; set; }

    public Node GetAsNode { get; }

    

    public Node GetParent();

    public Node GetNode(NodePath path);

    public void QueueFree();

    //public T GetNode<T>(NodePath path);
}

public interface ITypedNode<T> : INode
{
    public T GetAsSpecificNode { get; }
}
