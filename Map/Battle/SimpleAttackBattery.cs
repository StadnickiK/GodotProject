using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;


public interface ISimpleAttackBattery : ITargetSpottedListener
{
    public ITargetable CurrentTarget { get; set; }

    public IDamagable Source { get; set; }
}

public partial class SimpleAttackBattery : Node3D, ISimpleAttackBattery, IMapObjectController, IProjectileFactory
{
    // public OrderQueue OrderQueue { get; set; }

    public ITargetable CurrentTarget { get; set; }

    public IDamagable Source { get; set; }

    [Export]
    public double Reload { get; set; } = 3;

    [Export]
    double shotDelay = 0.1;

    double currentTime = 0;

    double batteryTimer = 0;

    [Export]
    public Godot.Collections.Array<Vector3> Batteries { get; set; } = [new Vector3(0,0,0) ];

    int currentBattery = 0;

    public Player Controller { get; set; }
    public ProjectileFactory ProjectileFactory { get; set; }

    public override void _Ready()
    {
        // OrderQueue = GetNode<OrderQueue>("OrderQueue");
        ProcessMode = ProcessModeEnum.Disabled;
    }
    public void OnTargetLost(ITargetable targetable)
    {
        if (targetable.Controller.PlayerID != Controller.PlayerID)
        {
            CurrentTarget = null;
            ProcessMode = ProcessModeEnum.Disabled;
        }
    }

    public void OnTargetSpotted(ITargetable targetable)
    {
        if (CurrentTarget == null && !targetable.Controller.Equals(Controller))
        {
            CurrentTarget = targetable;
            ProcessMode = ProcessModeEnum.Inherit;
        }
    }

    void Shoot()
    {
        var projectile = ProjectileFactory.CreateLaser(GlobalPosition + Batteries[currentBattery], CurrentTarget.GlobalTransform.Origin);
        projectile.Source = Source;
    }

    void Shoot(ITargetable target)
    {
        CurrentTarget = target;
        Shoot();
    }

    public override void _Process(double delta)
    {
        if (currentTime >= Reload)
        {
            if (batteryTimer >= shotDelay * currentBattery) {
                if (currentBattery == Batteries.Count-1)
                {
                    Shoot();
                    currentBattery = 0;
                    currentTime = batteryTimer;
                    batteryTimer = 0;
                }
                else
                {
                    Shoot();
                    currentBattery++;
                }
            } else {
                batteryTimer += delta;
            }
        }
            else
            {
                currentTime += delta;
            }
    }
}
