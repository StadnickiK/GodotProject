using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TurretControl : SimpleAttackBattery
{
    [Export]
    public string TurretPath { get; set; } = ScenePaths.Instance.TurretPath;

    // public string TurretPath { get; set; } = "res://Units/Base/turretjoint.tscn";

    PackedScene packedScene;

    public Node3D TransformParent { get; set; }

    public List<Turret> Turrets { get; set; } = new List<Turret>();

    int[] TurretIDsRange = {0, 1};

    // public List<Turretjoint> Turrets { get; set; } = new List<Turretjoint>();

    public override void _Ready()
    {
        packedScene = ResourceLoader.Load<PackedScene>(TurretPath);
        if(GetParent() is Node3D node) TransformParent = node;
    }

    public void InitTurrets(Unit unit)
    {
        TurretIDsRange[0] = Batteries.Count;
        InitBatteries(unit.ExportBatteries);
        if(unit.ModelData.Turrets != null && unit.Turrets != null)InitTurrets(unit.ModelData.Turrets, unit.Turrets.TurretModels);
        TurretIDsRange[1] = Batteries.Count;
    }

    private void InitBatteries(Array<Vector3> exportBatteries)
    {
        foreach (var item in exportBatteries)
        {
            var node = new Node3D();
            node.Position = item;
            AddChild(node);
            Batteries.Add(node);
        }
    }

    void InitTurrets(List<TurretData> turretDatas, List<TurretModel> turretModels)
    {
        foreach (var t in turretDatas)
        {
            var turret = packedScene.Instantiate<Turret>();
            AddChild(turret);
            turret.Name = "Turret "+ Turrets.Count;
            turret.TurretMesh.Mesh = t.Body.Mesh;
            turret.Transform = t.Body.Transform;
            var model = turretModels[t.TurretIndex];
            turret.StatManager.CloneStats(model);
            turret.Projectiles = model.Projectiles;
            foreach (var Barrel in t.Barrels)
            {
                var meshInstance = new MeshInstance3D();
                meshInstance.Mesh = Barrel.Mesh;
                meshInstance.Transform = Barrel.Transform;
                turret.AddChild(meshInstance);
                turret.Barrels.Add(meshInstance);
                // Batteries.Add(meshInstance);
            }
            turret.TransformParent = TransformParent;
            turret.ProjectileFactory = ProjectileFactory;
            Turrets.Add(turret);
            Batteries.Add(turret);
        }
    }

    public override void OnTargetLost(ITargetable targetable)
    {
        base.OnTargetLost(targetable);
        ResetTurretTarget();
    }

    public override void OnTargetSpotted(ITargetable targetable)
    {
        if (targetable.Controller != Controller)
        {
            base.OnTargetSpotted(targetable);
            UpdateTurretTarget(targetable);
        } 
    }
    void UpdateTurretTarget(ITargetable targetable)
    {
        foreach (var t in Turrets)
            t.UpdateTarget(targetable);
        
    }

    void ResetTurretTarget()
    {
        foreach (var t in Turrets)
            t.ResetTarget();
        
    }

    public override void BatteryFire(double delta)
    {
        base.BatteryFire(delta);   
    }

    // public override void Shoot(int batteryId)
    // {
    //     if((batteryId < TurretIDsRange[0] || batteryId == 0) && batteryId >= TurretIDsRange[1])
    //         base.Shoot(batteryId);
    //     else
    //         Turrets[batteryId].Shoot(CurrentTarget, Source);
    // }

    // public void InitTurrets(RigidBody3D parent, List<TurretData> TurretsData)
    // {
    //     foreach (var t in TurretsData)
    //     {
    //         var turret = packedScene.Instantiate<Turretjoint>();
    //         turret.GetNodes();
    //         turret.Turret.Init();
    //         turret.Turret.TurretMesh.Mesh = t.Body.Mesh;
    //         turret.Turret.Transform = t.Body.Transform;
    //         turret.Turret.Barrel.Mesh = t.Barrel.Mesh;
    //         turret.Turret.Barrel.Transform = t.Barrel.Transform;
    //         turret.Turret.ProjectileFactory = ProjectileFactory;
    //         turret.NodeA = parent.GetPath();
    //         AddChild(turret);
    //         Turrets.Add(turret);
    //     }
    // }
}
