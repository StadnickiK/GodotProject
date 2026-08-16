using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

public partial class PhysicsInterceptState : State<IMovable>
{

    IMovable MovableA;

    IMovable MovableB;

    [Export]
    public HashSet<string> StatNames { get; set; } = new HashSet<string>() { "Speed" };
    [Export]

    Aabb ClosestIntersection;

    ICollider3D ClosestCollider;

    Vector3 ParentSize;

    Vector3 TargetPos;

    ObstacleDetectionComponent ObstacleDetectionComponent;

    VelocityController velocityController;
       

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>

    public PhysicsInterceptState(){}

    public PhysicsInterceptState(IMovable movableA, IMovable movableB, ObstacleDetectionComponent component, VelocityController VelocityController)
    {
        MovableA = movableA;
        MovableB = movableB;
        ObstacleDetectionComponent = component;
        ParentSize = component.Parent.GetUnitSize();
        component.ClosestColliderChanged += _on_ClosestColliderChanged;
        velocityController = VelocityController;
    }

    void _on_ClosestColliderChanged(ICollider3D collider, Aabb collider3D)
    {
        ClosestCollider = collider;
        ClosestIntersection = collider3D;
    }

    private void CalculateAngularVelocity()
    {
        Vector3 vector3 = Vector3.Zero;
        if(ClosestCollider != null)
            vector3 = velocityController.GetAvoidanceAngularVelocity(Body.GlobalTransform, ClosestIntersection, ClosestCollider);
        else
            vector3 = GetInterceptAngularVelocity(Body.GlobalTransform);
        Body.AngularVelocity = vector3;
    }

    public Vector3 GetInterceptAngularVelocity(Transform3D currentTransform)
    {
        TargetPos = (MovableA.Position + MovableB.Position) / 2;
        var TimeToReachMidpoint = Body.Position.DistanceTo(TargetPos) / velocityController.Stats[GlobalStatNames.Speed].CurrentValue;

        var aPos = MovableA.Position + MovableA.Velocity * TimeToReachMidpoint;
        var bPos = MovableB.Position + MovableB.Velocity * TimeToReachMidpoint;

        TargetPos =  (aPos + bPos) / 2;

        return velocityController.GetAngularVelocity(currentTransform, TargetPos);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play a walking or moving animation.
    }

    public override void Exit()
    {
        base.Exit();
        InvokwMoveStateExited(new OrderQueue.Target(TargetPos));
    }

    public override State<IMovable> ProcessState(double delta)
    {
        CalculateAngularVelocity();
        return CalculateLinearVelocity();
    }

    private State<IMovable> CalculateLinearVelocity()
    {
        Vector3 dir = Body.GlobalTransform.Basis * Vector3.Forward;
        Vector3 toTarget = TargetPos - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        var len = toTarget.Length();
        if (len < velocityController.Stats[GlobalStatNames.Tolerance].CurrentValue)
        {
            Body.Velocity = Vector3.Zero;
            Body.OrderQueue.NextTarget();
            if (!Body.OrderQueue.HasTarget)
                return new IdleState(Body);
            return new PhysicsInterceptState(MovableA, MovableB, ObstacleDetectionComponent, velocityController);
        }
        var speed = len / 0.6f;
        speed = Mathf.Min(speed, velocityController.Stats[GlobalStatNames.Speed].CurrentValue);

        Body.Velocity = dir.Normalized() * speed;
        return this;
    }
}
