using Godot;
using System;

public partial class GuidedMissile : RayCast3D, IProjectile
{

    [Export]
    public int Speed { get; set; } = 100;
    
    [Export]
    public double RotationSpeed { get; set; } = 1.1;

    public IDamagable Source { get; set; }

    public event IProjectile.DamageEventHandler Damage;

    Target target;

    Vector3 Velocity { get; set; } = Vector3.Zero;

    public float DragFactor { get; set; } = 1;

    public void Shoot(Vector3 from, Vector3 to, Vector3 direction)
    {
        Velocity = direction.Normalized() * Speed;
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var direction = Vector3.Forward.Rotated(Vector3.Up, Rotation.Y).Normalized();

        if (target != null)
            direction = GlobalPosition.DirectionTo(target.GlobalPosition);
        var desiredVelocity = direction * Speed;

        var change = (desiredVelocity - Velocity) * DragFactor * (float)delta;
        Velocity += change;

        GlobalPosition += Velocity;
        LookAt(GlobalPosition + Velocity);
    }

}
