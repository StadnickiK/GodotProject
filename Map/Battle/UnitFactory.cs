using Godot;
using System.Collections.Generic;

public partial class UnitFactory : NodeFactoryBase<Unit3D>, IProjectileFactory
{
    Node3DPool ArmyPool { get; set; }

    public WorldCursorControl WorldCursorControl { get; set; }
    public ProjectileFactory ProjectileFactory { get; set; }

    public DrawingLines3DController DrawingLines3D { get; set; }

    PackedScene packedScene;

    public Data Data { get; set; }
    public UnitDragHandler UnitDragHandler { get; internal set; }


    public override void _Ready()
    {
        base._Ready();
        ArmyPool = GetNode<Node3DPool>("UnitPool");
        packedScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.Squad3DPath);
    }

    public IUnit3D CreateUnit(Node parent, Vector3 position, Player controller, Unit unit)
    {
        IUnit3D Unit3D;
        if (unit.StatManager.GetStat(GlobalStatNames.ModelCount).CurrentValue > 1)
        {
            Unit3D = packedScene.Instantiate<Squad3D>();
            parent.AddChild(Unit3D.GetAsNode);
        }
        else
        {
            Unit3D = CreateUnit(parent, position, controller);
        }        
        Unit3D.Initialize(this, parent, position, controller, unit);
        return Unit3D;
    }

    public IUnit3D CreateUnit(Node parent, Vector3 position, Player controller)
    {
        var Unit3D = (Unit3D)ArmyPool.GetNode3D(parent, position);
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

    public void ConnectSignals(IUnit3D Unit3D)
    {
        DrawingLines3D.ConnectUnit3D(Unit3D);
        Unit3D.InputController.ConnectWCC(WorldCursorControl);
        
        ConnectSaveNode(Unit3D);
    }

    public void ConnectSaveNode(ISavingNode savingNode)
    {
        ArmyPool.ConnectSaveNode(savingNode);
    }
}
