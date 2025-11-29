using Godot;
using System.Collections.Generic;

public partial class UnitFactory : NodeFactoryBase<Unit3D>, IProjectileFactory
{
    Node3DPool ArmyPool { get; set; }

    public WorldCursorControl WorldCursorControl { get; set; }

    public ModelLoader ModelLoader { get; set; }
    public ProjectileFactory ProjectileFactory { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ArmyPool = GetNode<Node3DPool>("UnitPool");
    }

    public Unit3D CreateUnit(Node parent, Vector3 position, Player controller, Unit unit)
    {
        var Unit3D = CreateUnit(parent, position, controller);
        ConnectSignals(Unit3D);
        Unit3D.LoadUnit(unit);
        Unit3D.ProjectileFactory = ProjectileFactory;
        Unit3D.UpdateModel(ModelLoader.Models[unit.ModelName]);
        return Unit3D;
    }

    public Unit3D CreateUnit(Node parent, Vector3 position, string name)
    {
        var Unit3D = (Unit3D)ArmyPool.GetNode3D(parent, position, name);
        ConnectSignals(Unit3D);
        return Unit3D;
    }

    public Unit3D CreateUnit(Node parent, Vector3 position, Player controller)
    {
        var Unit3D = (Unit3D)ArmyPool.GetNode3D(parent, position);
        ConnectSignals(Unit3D);
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

    void ConnectSignals(Unit3D Unit3D)
    {
        Unit3D.InputController.ConnectWCC(WorldCursorControl);
        ArmyPool.ConnectSaveNode(Unit3D);
    }
}
