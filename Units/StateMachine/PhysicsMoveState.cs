using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class PhysicsMoveState : State<IMovable>, IUpdateStat
{
    private OrderQueue.Target _target;

    public delegate void MoveStateExitedEventHandler(OrderQueue.Target target);

    public event MoveStateExitedEventHandler MoveStateExited;


    [Export]
    public HashSet<string> StatNames { get; set; } = new HashSet<string>() { "Speed" };
    [Export]
    private float _tolerance = 0.2f;
    [Export]
    public float MoveSpeed = 5f;         // Units per second
    [Export]
    public float TimeToRotate = 0.5f;    // Seconds to complete rotation

    public OrderQueue.Target Target { get => _target; set => _target = value; }
    public float Tolerance { get => _tolerance; set => _tolerance = value; }

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>

    public PhysicsMoveState(){}

    public PhysicsMoveState(OrderQueue.Target target)
    {
        Target = target;
    }

    public PhysicsMoveState(OrderQueue.Target target, float tolerance)
    {
        Target = target;
        Tolerance = tolerance;
    }

    public PhysicsMoveState(Vector3 target)
    {
        Target = new OrderQueue.Target(target);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play a walking or moving animation.
    }

    public override State<IMovable> ProcessState(double delta)
    {
        CalculateAngularVelocity();
        return CalculateLinearVelocity();
    }

    private State<IMovable> CalculateLinearVelocity()
    {
        Vector3 toTarget = Target.Point - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        if (toTarget.Length() < Tolerance)
        {
            Body.Velocity = Vector3.Zero;
            Body.OrderQueue.NextTarget();
            if (Body.OrderQueue.HasTarget)
                return new IdleState(Body);
            return new PhysicsMoveState(Body.OrderQueue.currentTarget);
        }

        Body.Velocity = toTarget.Normalized() * MoveSpeed;
        return this;
    }

    private void CalculateAngularVelocity()
    {
        // 1. Current facing direction (assuming -Z is forward)
        Vector3 forward = -Body.GlobalBasis.Z.Normalized();

        // 2. Desired direction toward target
        Vector3 toTarget = (Target.Point - Body.GlobalPosition).Normalized();

        // 3. If vectors are nearly aligned, don't rotate
        float dot = forward.Dot(toTarget);
        if (dot > 1 - Tolerance)
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

    public void UpdateStat(IStat stat)
    {
        MoveSpeed = stat.CurrentValue;
    }
}
