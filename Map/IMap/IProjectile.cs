using Godot;
using System;

public interface IProjectile : INode, ISavingNode
{
    public delegate void DamageEventHandler(IDamagable source, IDamagable to);

    public event DamageEventHandler Damage;

    public IDamagable Source { get; set; }

    public CollisionObject3D CollisionObject3D { get; set; }
    public void Shoot(Vector3 from, Vector3 to, Vector3 direction);

    public void Shoot(Vector3 from, ITargetable to, Vector3 direction);

    public void Show();

    public void Hide();
}
