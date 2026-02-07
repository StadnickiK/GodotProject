using Godot;
using System;

public interface IDamagable : ITypedNode<CollisionObject3D>
{
    public void Damage();

    public StatManager StatManager { get; set; }

    public Vector3 GlobalPosition { get; set; }
}
