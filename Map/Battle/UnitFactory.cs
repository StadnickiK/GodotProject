using Godot;
using System.Collections.Generic;

public partial class UnitFactory : NodeFactoryBase<Unit3D>
{
    Node3DPool ArmyPool { get; set; }

    public WorldCursorControl WorldCursorControl { get; set; }

    public ModelLoader ModelLoader { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ArmyPool = GetNode<Node3DPool>("UnitPool");
    }

    public Unit3D CreateUnit(Node parent, Vector3 position, Player controller, Unit unit)
    {
        var Unit3D = CreateUnit(parent, position, controller);
        ConnectSignals(Unit3D);
        Unit3D.MeshInstance3D.Mesh = ModelLoader.GetMeshInstance3D(unit.ModelName).Mesh;
        Unit3D.CollisionShape3D.Shape = ModelLoader.GetCollisionShape3D(unit.ModelName).Shape;
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
        WorldCursorControl.ConnectToSelectTarget(Unit3D.InputController);
        WorldCursorControl.ConnectToSelectUnit(Unit3D.InputController);
    }
}
