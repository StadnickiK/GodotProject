using Godot;
using System;
using System.Diagnostics;

public partial class PhysicsPlacementState : State<IMovable>
{
    private OrderQueue.Target _target;

    public delegate void MoveStateExitedEventHandler(OrderQueue.Target target);

    public event MoveStateExitedEventHandler MoveStateExited;

    [Export]
    private float _tolerance = 10f;
    [Export]
    public float MoveSpeed = 5f;         // Units per second
    [Export]
    public float PlacementSpeed = 10000f;   
    [Export]
    public float TimeToRotate = 0.5f;    // Seconds to complete rotation

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>
    public PhysicsPlacementState(OrderQueue.Target target)
    {
        _target = target;
        PlacementSpeed *= MoveSpeed;
    }

    public PhysicsPlacementState(Vector3 target)
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

        return this;
    }

}
