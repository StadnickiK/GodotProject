using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

public partial class PhysicsMoveState : State<IMovable>
{
    private OrderQueue.Target _target;
    public OrderQueue.Target Target { get => _target; set => _target = value; }

    VelocityController velocityController;

    NavAgent3d NavAgent3D;

    // bool TargetReached = false;

    float Tolerance;

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>

    public PhysicsMoveState(){}

    public PhysicsMoveState(OrderQueue.Target target)
    {
        Target = target;
    }

    public PhysicsMoveState(OrderQueue.Target target, NavAgent3d agent3D, VelocityController VelocityController)
    {
        velocityController = VelocityController; 
        Target = target;
        // agent3D.TargetReached += _on_NavigationFinished;
        NavAgent3D = agent3D;
    }

    // void _on_NavigationFinished()
    // {
    //     TargetReached = true;
    // }

    public PhysicsMoveState(Vector3 target)
    {
        Target = new OrderQueue.Target(target);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        if(Target != null)
            if (Target.TargetNode != null){
                if (Target.TargetNode is IMapObjectController controller)
                    if (controller.Controller.PlayerID != body.Controller.PlayerID)
                    {
                        if(body.StatManager.HasStat(GlobalStatNames.Range)) Tolerance = velocityController.Stats[GlobalStatNames.Range].CurrentValue;
                    }
            }
            else
            {
                Tolerance = velocityController.Stats[GlobalStatNames.Tolerance].CurrentValue;
            }
        // Optionally: play a walking or moving animation.
    }

    public override void Exit()
    {
        base.Exit();
        InvokwMoveStateExited(Target);
    }

    public override State<IMovable> ProcessState(double delta)
    {
        CalculateAngularVelocity();
        return CalculateLinearVelocity();
    }

    private State<IMovable> CalculateLinearVelocity()
    {
        Vector3 dir = Body.GlobalTransform.Basis * Vector3.Forward;
        Vector3 toTarget = Target.Point - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        var len = toTarget.Length();
        if(len < NavAgent3D.TargetDesiredDistance)
        {
            //var p = NavAgent3D.GetNextPathPosition();
            GD.Print("T pos " + Target.Point + " B pos " + Body.GlobalPosition + " " + len);
        }
            
        if (NavAgent3D.IsNavigationFinished())
        {
            Body.Velocity = Vector3.Zero;
            Body.OrderQueue.NextTarget();
            if (!Body.OrderQueue.HasTarget)
                return new IdleState(Body);
            NavAgent3D.TargetPosition = Body.OrderQueue.currentTarget.Point;
            return new PhysicsMoveState(Body.OrderQueue.currentTarget);
        }
        
        var speed = len / 0.6f;
        speed = Mathf.Min(speed, velocityController.Stats[GlobalStatNames.Speed].CurrentValue);
        var vel = dir.Normalized() * speed;
        Body.Velocity = vel;
        //Body.Velocity = velocityController.GetSteering(vel);
        return this;
    }

    private void CalculateAngularVelocity()
    {
        Vector3 vector3 = Vector3.Zero;
        var p = NavAgent3D.GetNextPathPosition();
        // vector3 = velocityController.GetAngularVelocity(Body.GlobalTransform, Target.Point);
        vector3 = velocityController.GetAngularVelocity(Body.GlobalTransform, p);
        // if(ClosestCollider == null && Target != null)
        //     vector3 = velocityController.GetAngularVelocity(Body.GlobalTransform, Target.Point);
        // else
        //     vector3 = velocityController.GetAvoidanceAngularVelocity(Body.GlobalTransform, ClosestIntersection, ClosestCollider);
        Body.AngularVelocity = vector3;
    }

    // public void UpdateStat(IStat stat)
    // {
    //     velocityController.Stats[GlobalStatNames.Speed].CurrentValue = stat.CurrentValue;
    // }
}
