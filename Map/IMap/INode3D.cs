using Godot;
using System;

public interface INode3D : INode
{
    public Vector3 GlobalPosition { get; set; }

    public Vector3 Position { get; set; }

    public Vector3 GlobalRotation { get; set; }

    public Transform3D GlobalTransform { get; set; }

    public Basis GlobalBasis { get; set; }
}
