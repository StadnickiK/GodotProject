using Godot;
using System;
using System.Diagnostics;

public partial class MoveState : State<IMovable>
{
    private OrderQueue.Target _target;

    public delegate void MoveStateExitedEventHandler(OrderQueue.Target target);

    public event MoveStateExitedEventHandler MoveStateExited;

    [Export]
    private float _tolerance = 0.2f;
    [Export]
    private float Speed = 10.0f;

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>
    public MoveState(OrderQueue.Target target)
    {
        _target = target;
    }

    public MoveState(Vector3 target)
    {
        _target = new OrderQueue.Target(target);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play a walking or moving animation.
    }

    public override State<IMovable> ProcessState(double delta)
    {
        // Calculate the direction toward the target.
        Vector3 currentPosition = Body.GlobalPosition;
        Vector3 direction = _target.Point - currentPosition; 
        float distance = direction.Length();

        // If we’re close enough to the target...
        if (distance < _tolerance)
        {
            Body.OrderQueue.NextTarget();
            if (_target.TargetNode != null)
            {
                //Body.EmitSignal(nameof(Ship.SignalName.SignalEnterMapObject), Body, _target.TargetNode);
                MoveStateExited?.Invoke(_target);
                return new IdleState(Body);
            }
            // Check if there’s another target in the player's queue.
            if (Body.OrderQueue.Targets.Count > 0)
            {
                return new MoveState(Body.OrderQueue.currentTarget.Point);
            }
            else
            {
                // No more targets: transition to idle.
                Body.UpdateVelocity(new Vector3(0, Body.Velocity.Y, 0), Vector3.Zero);
                return new IdleState(Body);
            }
        }

        // Normalize the direction and update horizontal velocity.
        direction = direction.Normalized(); // * new Vector3(1,0,1); // remove y axis since the movement on campaign map is 2dss
        Body.Velocity = direction * Speed;  // Y velocity remains unchanged
        //Body.MoveAndCollide();

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
