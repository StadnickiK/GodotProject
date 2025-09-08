using Godot;
using System;

public interface IDamagable
{
    public void Damage();

    public StatManager StatManager { get; set; }
}
