using Godot;
using System.Collections.Generic;

public partial class ProjectileFactory : Node3D
{
    Node3DPool LaserPool { get; set; }

    public WorldCursorControl WorldCursorControl { get; set; }

    public ModelLoader ModelLoader { get; set; }

    public Node ProjectileParent { get; set; }

    public IDamageCalculator DamageCalculator { get; set; }

    public override void _Ready()
    {
        base._Ready();
        LaserPool = GetNode<Node3DPool>("LaserPool");
    }

    public IProjectile CreateLaser(Vector3 position, ITargetable target)
    {
        var projectile = GetLaser(ProjectileParent, position);
        projectile.Shoot(position, target, Vector3.Zero);
        // ConnectSignals(projectile);
        // Unit3D.LoadUnit(projectileData);
        // Unit3D.UpdateModel(ModelLoader.GetMeshInstance3D(unit.ModelName).Mesh, ModelLoader.GetCollisionShape3D(unit.ModelName).Shape);
        return projectile;
    }

    public IProjectile GetLaser(Node parent, Vector3 position)
    {
        var laser = (IProjectile)LaserPool.GetNode3D(parent, position);
        ConnectSignals(laser);
        return laser;
    }

    public IProjectile GetProjectile(string type, Vector3 position, ITargetable target)
    {
        var projectile = GetLaser(ProjectileParent, position);
        projectile.Shoot(position, target, Vector3.Zero);
        // ConnectSignals(projectile);
        // Unit3D.LoadUnit(projectileData);
        // Unit3D.UpdateModel(ModelLoader.GetMeshInstance3D(unit.ModelName).Mesh, ModelLoader.GetCollisionShape3D(unit.ModelName).Shape);
        return projectile;
    }

    void UpdateVisibility(Unit3D unit, VisibilityConroller.VisibilityStruct visibilityStruct, int PlayerID, bool visible)
    {
        unit.VisibilityConroller.UpdateVisible(visibilityStruct, PlayerID, visible);
    }

    void ConnectSignals(IProjectile projectile)
    {
        projectile.Damage -= DamageCalculator.CalculateDamage;
        projectile.Damage += DamageCalculator.CalculateDamage;
    }
}
