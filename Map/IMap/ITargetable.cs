using Godot;
using System;

public interface ITargetable : IVisible, IMapObjectController
{
    public Vector3 GlobalPosition { get; set; }

    public Transform3D GlobalTransform { get; set; }
}
