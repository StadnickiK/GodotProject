using Godot;
using System;
using System.Collections.Generic;

public partial class TurretControl : SimpleAttackBattery
{
    [Export]
    public string TurretPath { get; set; } = "res://Units/Base/turret.tscn";

    // public string TurretPath { get; set; } = "res://Units/Base/turretjoint.tscn";

    PackedScene packedScene;

    public List<Turret> Turrets { get; set; } = new List<Turret>();

    // public List<Turretjoint> Turrets { get; set; } = new List<Turretjoint>();

    public override void _Ready()
    {
        packedScene = ResourceLoader.Load<PackedScene>(TurretPath);
    }


    public void InitTurrets(RigidBody3D parent, List<TurretData> TurretsData)
    {
        foreach (var t in TurretsData)
        {
            var turret = packedScene.Instantiate<Turret>();
            AddChild(turret);
            turret.TurretMesh.Mesh = t.Body.Mesh;
            turret.Transform = t.Body.Transform;
            foreach (var Barrel in t.Barrels)
            {
                var meshInstance = new MeshInstance3D();
                meshInstance.Mesh = Barrel.Mesh;
                meshInstance.Transform = Barrel.Transform;
                turret.AddChild(meshInstance);
                turret.Barrels.Add(meshInstance);
            }

            turret.ProjectileFactory = ProjectileFactory;
            Turrets.Add(turret);
        }
    }

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
