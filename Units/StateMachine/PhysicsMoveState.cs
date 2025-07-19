using Godot;
using System;
using System.Diagnostics;

public partial class PhysicsMoveState : State
{
    private OrderQueue.Target _target;

    public delegate void MoveStateExitedEventHandler(OrderQueue.Target target);

    public event MoveStateExitedEventHandler MoveStateExited;

    [Export]
    private float _tolerance = 0.2f;
     [Export]
    public float MoveSpeed = 5f;         // Units per second
    [Export]
    public float TimeToRotate = 0.5f;    // Seconds to complete rotation

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>
    public PhysicsMoveState(OrderQueue.Target target)
    {
        _target = target;
    }

    public PhysicsMoveState(Vector3 target)
    {
        _target = new OrderQueue.Target(target);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play a walking or moving animation.
    }

    public override State ProcessState(double delta)
    {
        CalculateAngularVelocity();
        CalculateLinearVelocity();

        return this;
    }

    private void CalculateLinearVelocity()
    {
        Vector3 toTarget = _target.Point - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        if (toTarget.Length() < _tolerance)
        {
            Body.Velocity = Vector3.Zero;
            return;
        }

        Body.Velocity = toTarget.Normalized() * MoveSpeed;
    }

    private void CalculateAngularVelocity()
    {
        // 1. Current facing direction (assuming -Z is forward)
        Vector3 forward = -Body.GlobalBasis.Z.Normalized();

        // 2. Desired direction toward target
        Vector3 toTarget = (_target.Point - Body.GlobalPosition).Normalized();

        // 3. If vectors are nearly aligned, don't rotate
        float dot = forward.Dot(toTarget);
        if (dot > 1 - _tolerance)
        {
            Body.AngularVelocity = Vector3.Zero;
            return;
        }

        // 4. Compute rotation axis
        Vector3 rotationAxis = forward.Cross(toTarget).Normalized();

        // 5. Angle between current direction and target direction
        float angle = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));

        // 6. Angular velocity: how fast to rotate per second
        Body.AngularVelocity = (rotationAxis * angle) / Mathf.Max(TimeToRotate, 0.001f);
    }

}
