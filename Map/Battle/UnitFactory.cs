using Godot;
using System.Collections.Generic;

public partial class UnitFactory : NodeFactoryBase<Unit3d>
{
    Node3DPool ArmyPool { get; set; }

    public ModelLoader ModelLoader { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ArmyPool = GetNode<Node3DPool>("UnitPool");
    }

    public Unit3d CreateUnit(Node parent, Vector3 position, Player controller, Unit unit)
    {
        var unit3d = CreateUnit(parent, position, controller);
        unit3d.MeshInstance3D.Mesh = ModelLoader.GetMeshInstance3D(unit.ModelName).Mesh;
        return unit3d;
    }

    public Unit3d CreateUnit(Node parent, Vector3 position, string name)
    {
        var ship = (Unit3d)ArmyPool.GetNode3D(parent, position, name);
        return ship;
    }

    public Unit3d CreateUnit(Node parent, Vector3 position, Player controller)
    {
        var unit = (Unit3d)ArmyPool.GetNode3D(parent, position);
        UpdateVisibility(
            unit,
            new VisibilityConroller.VisibilityStruct()
            { 
                Visibility = VisibilityConroller.VisibilityState.Unexplored,
                Visible = controller.IsLocal
            },
            controller.PlayerID,
            controller.IsLocal
            );
        unit.Controller = controller;
        return unit;
    }
    
    void UpdateVisibility(Unit3d unit, VisibilityConroller.VisibilityStruct visibilityStruct,int PlayerID, bool visible) {
        unit.VisibilityConroller.UpdateVisible(visibilityStruct, PlayerID, visible);
    }
}
