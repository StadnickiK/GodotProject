using Godot;
using System;

public enum IMovableState
{
    Movement,
    Placement,
    Drag
}

public interface IMovable
{
    public IMovableState MovableState { get; set; }
    public OrderQueue OrderQueue { get; set; }

    public Godot.Vector3 GlobalPosition { get; set; }

    public Basis GlobalBasis { get; set; }

    public Godot.Vector3 Velocity { get; set; }

    public Godot.Vector3 AngularVelocity { get; set; }

    public void MoveToPosition(Godot.Vector3 position);
    public void MoveToTarget(OrderQueue.Target target);
    public void ClearTargets();

    public void UpdateVelocity(Godot.Vector3 linearVelocity, Godot.Vector3 angularVelocity);

    public void LookAt(Godot.Vector3 target, Nullable<Godot.Vector3> up = null, bool useModelFront = false);

    public void SetPosition(Godot.Vector3 Vector3);
}
