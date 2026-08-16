using Godot;
using System;

public partial class GuidedMissile : Area3D, IProjectile
{

    [Export]
    public int Speed { get; set; } = 100;
    
    [Export]
    public double RotationSpeed { get; set; } = 1.1;

    [Export]
    public double Lifetime { get; set; } = 30;

    public string ExplosionPath { get; set; } = ScenePaths.Instance.ExplosionPath;

    public IDamagable Source { get; set; }

    public event IProjectile.DamageEventHandler Damage;

    public Node GetAsNode { get { return this; }}

    ITargetable target;

    Vector3 Velocity { get; set; } = Vector3.Zero;

    public float DragFactor { get; set; } = 1;
    public CollisionObject3D CollisionObject3D { get; set; }


    double time = 0;

    public event Node3DPool.SaveNodeEventHandler SaveNode;

    PackedScene packedScene;

    public override void _Ready()
    {
        packedScene = ResourceLoader.Load<PackedScene>(ExplosionPath);
    }


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
        if(time >= Lifetime)
        {
            Death();
        }
        time += delta;
       
    }

    public void _on_body_entered(Node node)
    {
        if(node != Source && node != CollisionObject3D)
        {
            if (node is IDamagable damagable)
            {
                Damage?.Invoke(Source, damagable);
                
            }
            Death();
            time = Lifetime + 1;
            //InvokeSaveNode();
        }
    }

    void Death()
    {
        var explosion = packedScene.Instantiate<Explosion>();
        var transform = explosion.Transform;
        transform.Origin = GlobalPosition;
        explosion.Transform = transform;
        explosion.Emitting = true;
        explosion.OneShot = true;
        explosion.Restart();
        explosion.Finished += explosion._on_finished;
        GetParent().AddChild(explosion);
        InvokeSaveNode();
        Hide();
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
