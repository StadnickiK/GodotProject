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

    public IProjectile CreateLaser(Vector3 position, Vector3 target)
    {
        var projectile = GetLaser(ProjectileParent, position);
        projectile.Shoot(position, target, Vector3.Zero);
        // ConnectSignals(projectile);
        // Unit3D.LoadUnit(projectileData);
        // Unit3D.UpdateModel(ModelLoader.GetMeshInstance3D(unit.ModelName).Mesh, ModelLoader.GetCollisionShape3D(unit.ModelName).Shape);
        return projectile;
    }

    public Laser GetLaser(Node parent, Vector3 position)
    {
        var laser = (Laser)LaserPool.GetNode3D(parent, position);
        ConnectSignals(laser);
        return laser;
    }

    public Unit3D CreateProjectile(Node parent, Vector3 position, Player controller)
    {
        var Unit3D = (Unit3D)LaserPool.GetNode3D(parent, position);
        //ConnectSignals(Unit3D);
        UpdateVisibility(
            Unit3D,
            new VisibilityConroller.VisibilityStruct()
            {
                Visibility = VisibilityConroller.VisibilityState.Unexplored,
                Visible = controller.IsLocal
            },
            controller.PlayerID,
            controller.IsLocal
            );
        Unit3D.Controller = controller;
        return Unit3D;
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
