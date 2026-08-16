using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

public partial class PhysicsEvadeState : State<IMovable>
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
       

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>

    public PhysicsEvadeState(){}

    public PhysicsEvadeState(IMovable target, ObstacleDetectionComponent component, VelocityController VelocityController)
    {
        Movable = target;
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
        if(ClosestCollider == null)
            vector3 = velocityController.GetAngularVelocity(Body.GlobalTransform, Movable.Position);
        else if(ClosestCollider != null)
            vector3 = velocityController.GetAvoidanceAngularVelocity(Body.GlobalTransform, ClosestIntersection, ClosestCollider);
        else if(Movable != null)
            vector3 = GetEvadeAngularVelocity(Body.GlobalTransform);
        Body.AngularVelocity = vector3;
    }

    public Vector3 GetEvadeAngularVelocity(Transform3D currentTransform)
    {

        var toPursuer = Movable.Position - Body.Position;

        var pursuerSpeed = Movable.StatManager.GetStat(GlobalStatNames.Speed).CurrentValue;
        float LookAheadTime = toPursuer.Length() / (pursuerSpeed + velocityController.Stats[GlobalStatNames.Speed].CurrentValue);

        var targetPosition = Movable.Velocity + Movable.Position * LookAheadTime;

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

    float ang = 0;

    private State<IMovable> CalculateLinearVelocity()
    {
        Vector3 dir = Body.GlobalTransform.Basis * Vector3.Forward;
        Vector3 toTarget = Movable.Position - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        var len = toTarget.Length();
        if(len < 5)
            GD.Print("T pos " + Movable.Position + " B pos " + Body.GlobalPosition + " " + len + " " + ang);
        if (len < velocityController.Stats[GlobalStatNames.Tolerance].CurrentValue)
        {
            Body.Velocity = Vector3.Zero;
            Body.OrderQueue.NextTarget();
            if (!Body.OrderQueue.HasTarget)
                return new IdleState(Body);
            return new PhysicsEvadeState(Movable, ObstacleDetectionComponent, velocityController);
        }
        var speed = len / 0.6f;
        speed = Mathf.Min(speed, velocityController.Stats[GlobalStatNames.Speed].CurrentValue);

        Body.Velocity = dir.Normalized() * speed;
        return this;
    }
}
