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
    public string Projectile { get; set; } = "Laser";

    [Export]
    double shotDelay = 0.1;
    [Export]
    double shotDelayMinRandom = -0.1;

    [Export]
    double shotDelayMaxRandom = 0.1;

    double currentTime = 0;

    double batteryTimer = 0;

    public List<Node3D> Batteries { get; set; } = new List<Node3D>();

    int currentBattery = 0;

    public Player Controller { get; set; }
    public ProjectileFactory ProjectileFactory { get; set; }

    public override void _Ready()
    {
        // OrderQueue = GetNode<OrderQueue>("OrderQueue");
        ProcessMode = ProcessModeEnum.Disabled;
        shotDelay += new Random().NextDouble() * (shotDelayMaxRandom - shotDelayMinRandom) + shotDelayMinRandom;
    }
    public virtual void OnTargetLost(ITargetable targetable)
    {
        if (targetable.Controller.PlayerID != Controller.PlayerID)
        {
            CurrentTarget = null;
            ProcessMode = ProcessModeEnum.Disabled;
        }
    }

    public virtual void OnTargetSpotted(ITargetable targetable)
    {
        if (CurrentTarget == null && !Controller.Equals(targetable.Controller))
        {
            CurrentTarget = targetable;
            ProcessMode = ProcessModeEnum.Inherit;
        }
    }

    public virtual void Shoot(int batteryId)
    {
        var projectile = ProjectileFactory.CreateLaser(Batteries[batteryId].GlobalPosition, CurrentTarget);
        projectile.Source = Source;
        //Batteries[currentBattery].AddChild(projectile.GetAsNode);
    }

    public virtual void BatteryFire(double delta)
    {
        if (currentTime >= Reload && CurrentTarget != null)
        {
            if (batteryTimer >= shotDelay * currentBattery) {
                if (currentBattery == Batteries.Count-1)
                {
                    Shoot(currentBattery);
                    currentBattery = 0;
                    currentTime = batteryTimer;
                    batteryTimer = 0;
                }
                else
                {
                    Shoot(currentBattery);
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

    public override void _Process(double delta)
    {
        BatteryFire(delta);
    }
}
