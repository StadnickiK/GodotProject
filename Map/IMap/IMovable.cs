using Godot;
using System;

public enum IMovableState
{
    Movement,
    Placement,
    Drag
}

public interface IMovable : INode3D, ISelection, IStatManager, IMapObjectController
{
    public IMovableState MovableState { get; set; }

    public MeshInstance3D MeshInstance3D { get; set; }

    public Godot.Vector3 Velocity { get; set; }

    public VelocityController VelocityController { get; }

    public Godot.Vector3 AngularVelocity { get; set; }

    public void UpdateVelocity(Godot.Vector3 linearVelocity, Godot.Vector3 angularVelocity);

    public void LookAt(Godot.Vector3 target, Nullable<Godot.Vector3> up = null, bool useModelFront = false);

    public void SetPosition(Godot.Vector3 Vector3);
}

public interface IMovable3D : IMovable, ICollider3D
{
    
}