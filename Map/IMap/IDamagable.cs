using Godot;
using System;

public interface IDamagable
{
    public void Damage();

    public StatManager StatManager { get; set; }

    public Vector3 GlobalPosition { get; set; }
}
