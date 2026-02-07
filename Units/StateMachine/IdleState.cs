using Godot;
using System;

public partial class IdleState : State<IMovable>
{
    //public IdleState(){}

    public IdleState(IMovable body)
    {
        Body = body;
    }
    
    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play an idle animation.
    }

    public override State<IMovable> ProcessState(double delta)
    {
        // Stop horizontal movement (preserving the Y velocity).
        Body.Velocity = new Vector3(0, 0, 0);
        Body.AngularVelocity = new Vector3(0, 0, 0);
        return this;
    }
}
