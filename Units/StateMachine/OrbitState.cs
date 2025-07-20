using Godot;
using System;

public partial class OrbitState : State<IMovable>
{
    public override void Enter(IMovable body)
    {
        base.Enter(body);
        // Optionally: play an idle animation.
    }

    public override void Exit(){
        base.Exit();
    }

    public override State<IMovable> ProcessState(double delta)
    {
        // Stop horizontal movement (preserving the Y velocity).
        Body.Velocity = new Vector3(0, Body.Velocity.Y, 0);
        return this;
    }
}
