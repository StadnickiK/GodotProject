using Godot;
using System;
using System.Diagnostics;

public partial class MoveState : State
{
    private TargetManager<Node3D>.Target _target;

    [Export]
    private float _tolerance = 0.2f;
    [Export]
    private float Speed = 10.0f;

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>
    public MoveState(TargetManager<Node3D>.Target target)
    {
        _target = target;
    }

    public MoveState(Vector3 target)
    {
        _target = new TargetManager<Node3D>.Target(target);
    }

    public override void Enter(Ship body)
    {
        base.Enter(body);
        // Optionally: play a walking or moving animation.
    }

    public override State ProcessState(double delta)
    {
        // Calculate the direction toward the target.
        Vector3 currentPosition = Body.GlobalPosition;
        Vector3 direction = _target.Point - currentPosition;
        float distance = direction.Length();

        // If we’re close enough to the target...
        if (distance < _tolerance)
        {
            if(_target.TargetNode != null)
                Body.EmitSignal(nameof(Ship.SignalName.SignalEnterMapObject), Body, _target.TargetNode);
            // Check if there’s another target in the player's queue.
            if (Body.targetManager.Targets.Count > 0)
            {
                Body.targetManager.NextTarget();
                return new MoveState(Body.targetManager.currentTarget.Point);
            }
            else
            {
                // No more targets: transition to idle.
                Body.Velocity = new Vector3(0, Body.Velocity.Y, 0);
                return new IdleState();
            }
        }

        // Normalize the direction and update horizontal velocity.
        direction = direction.Normalized();
        Body.Velocity = direction * Speed;  // Y velocity remains unchanged
        Body.MoveAndSlide();

        // Optionally rotate to face the movement direction.
        // We only adjust the horizontal rotation (X and Z axes).
        Body.LookAt(_target.Point);
        
        return this;
    }

    //  public override void _Process(float delta)
    //  {
    //      
    //  }

}
