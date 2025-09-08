using Godot;
using System;

public interface IProjectile
{
    public delegate void DamageEventHandler(IDamagable source, IDamagable to);

    public event DamageEventHandler Damage;

    public IDamagable Source { get; set; }
    public void Shoot(Vector3 from, Vector3 to);

}
