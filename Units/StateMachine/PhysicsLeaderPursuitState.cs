using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

public partial class PhysicsLeaderPursuitState : State<IMovable>
{

    IMovable Movable;

    [Export]
    public HashSet<string> StatNames { get; set; } = new HashSet<string>() { "Speed" };
    [Export]

    Aabb ClosestIntersection;

    ICollider3D ClosestCollider;

    Vector3 ParentSize;

    ObstacleDetectionComponent ObstacleDetectionComponent;

    VelocityController velocityController;
       
    Vector3 offset;

    Vector3 targetPosition;

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>

    public PhysicsLeaderPursuitState(){}

    public PhysicsLeaderPursuitState(IMovable target, ObstacleDetectionComponent component, VelocityController VelocityController, Vector3 Offset)
    {
        Movable = target;
        ObstacleDetectionComponent = component;
        ParentSize = component.Parent.GetUnitSize();
        component.ClosestColliderChanged += _on_ClosestColliderChanged;
        velocityController = VelocityController;
        offset = Offset;
    }

    void _on_ClosestColliderChanged(ICollider3D collider, Aabb collider3D)
    {
        ClosestCollider = collider;
        ClosestIntersection = collider3D;
    }

    private void CalculateAngularVelocity()
    {
        Vector3 vector3 = Vector3.Zero;
        if(ClosestCollider == null)
            vector3 = velocityController.GetAngularVelocity(Body.GlobalTransform, Movable.Position);
        else if(ClosestCollider != null)
            vector3 = velocityController.GetAvoidanceAngularVelocity(Body.GlobalTransform, ClosestIntersection, ClosestCollider);
        else if(Movable != null)
            vector3 = GetInterceptAngularVelocity(Body.GlobalTransform);
        Body.AngularVelocity = vector3;
    }

    public Vector3 GetInterceptAngularVelocity(Transform3D currentTransform)
    {
        Vector3 upDir = new Vector3(0, 1, 0);

        var toEvader = Movable.Position - Body.Position;

        var targetSpeed = Movable.StatManager.GetStat(GlobalStatNames.Speed).CurrentValue;
        float LookAheadTime = toEvader.Length() / (targetSpeed + velocityController.Stats[GlobalStatNames.Speed].CurrentValue);

        var targetPosition = Movable.Position + offset + Movable.Velocity * LookAheadTime;

        return velocityController.GetAngularVelocity(currentTransform, targetPosition);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play a walking or moving animation.
    }

    public override void Exit()
    {
        base.Exit();
        InvokwMoveStateExited(new OrderQueue.Target(Movable));
    }

    public override State<IMovable> ProcessState(double delta)
    {
        CalculateAngularVelocity();
        return CalculateLinearVelocity();
    }

    private State<IMovable> CalculateLinearVelocity()
    {
        Vector3 dir = Body.GlobalTransform.Basis * Vector3.Forward;
        Vector3 toTarget = targetPosition - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        var len = toTarget.Length();
        if (len < velocityController.Stats[GlobalStatNames.Tolerance].CurrentValue)
        {
            Body.Velocity = Vector3.Zero;
            Body.OrderQueue.NextTarget();
            if (!Body.OrderQueue.HasTarget)
                return new IdleState(Body);
            return new PhysicsLeaderPursuitState(Movable, ObstacleDetectionComponent, velocityController, offset);
        }
        var speed = len / 0.6f;
        speed = Mathf.Min(speed, velocityController.Stats[GlobalStatNames.Speed].CurrentValue);

        Body.Velocity = dir.Normalized() * speed;
        return this;
    }
}
