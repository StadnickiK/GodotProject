using Godot;
using System;

public interface IMovable
{
    public OrderQueue OrderQueue { get; set; }

    public Vector3 GlobalPosition { get; set; }

    public Basis GlobalBasis { get; set; }

    public Vector3 Velocity { get; set; }

    public Vector3 AngularVelocity { get; set; }

    public void MoveToPosition(Vector3 position);
    public void MoveToTarget(OrderQueue.Target target);
    public void ClearTargets();

    public void UpdateVelocity(Vector3 linearVelocity, Vector3 angularVelocity);

    public void LookAt(Vector3 target, Nullable<Vector3> up = null, bool useModelFront = false);
}
