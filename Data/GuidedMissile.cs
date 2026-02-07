using Godot;
using System;

public partial class GuidedMissile : RayCast3D, IProjectile
{

    [Export]
    public int Speed { get; set; } = 100;
    
    [Export]
    public double RotationSpeed { get; set; } = 1.1;

    [Export]
    public double Duration { get; set; } = 30;

    public IDamagable Source { get; set; }

    public event IProjectile.DamageEventHandler Damage;

    public Node GetAsNode { get { return this; }}

    ITargetable target;

    Vector3 Velocity { get; set; } = Vector3.Zero;

    public float DragFactor { get; set; } = 1;
    public CollisionObject3D CollisionObject3D { get; set; }


    double time = 0;

    public event Node3DPool.SaveNodeEventHandler SaveNode;

    public void Shoot(Vector3 from, ITargetable to, Vector3 direction)
    {
        time = 0;
        GlobalPosition = from;
        target =  to;
        Velocity = direction.Normalized() * Speed;
        Show();
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

        if (IsColliding() || time > Duration){
            if (GetCollider() is IDamagable damagable)
            {
                Damage?.Invoke(Source, damagable);
            }
        InvokeSaveNode();
        }
       
    }

    public void InvokeSaveNode()
    {
        SaveNode?.Invoke(this);
    }

    public void Shoot(Vector3 from, Vector3 to, Vector3 direction)
    {
        throw new NotImplementedException();
    }

    public void BeforeSave()
    {
        
    }

}
