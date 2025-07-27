using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ManualBattleScene : Node3D
{

    [Export]
    public float FormationSpacing { get; set; } = 5;

    MeshInstance3D AttackeDefenderDeployMarker;

    MeshInstance3D DefenderDeployMarker;

    Area3D AttackerDeployZone;

    Area3D DefenderDeployZone;

    UnitFactory UnitFactory { get; set; }

    public SpaceBattle SpaceBattle { get; set; }

    public HashSet<Ship> Combatants { get; set; } = new HashSet<Ship>();

    public delegate void OpenBattlePanelEventHandler(SpaceBattle battle);

    public event OpenBattlePanelEventHandler OpenBattlePanel;

    public delegate void LookAtEventHandler(Godot.Vector3 target);

    public event LookAtEventHandler CameraLookAt;

    public delegate void ChangeMovableStateEventHandler(IMovableState movableState);

    public event ChangeMovableStateEventHandler ChangeMovableState;

    public WorldCursorControl WCC { get; set; }

    public BoxSelectController BoxSelectController { get; set; }

    public void ClearCombat()
    {
        BoxSelectController.SetProcessInput(false);
        SpaceBattle.AcceptResult();
        Combatants.Clear();
        SpaceBattle.Attackers.Clear();
        SpaceBattle.Defenders.Clear();
    }

    public void InitializeBattle(Random random, ModelLoader modelLoader, WorldCursorControl worldCursorControl)
    {
        SpaceBattle.Rand = random;
        UnitFactory.ModelLoader = modelLoader;
        WCC = worldCursorControl;
        UnitFactory.WorldCursorControl = worldCursorControl;
    }

    void ConnectInputEvent(Node node)
    {
        if (!node.IsConnected(CollisionObject3D.SignalName.InputEvent, WCC.OnGroundInputCallable))
            node.Connect(CollisionObject3D.SignalName.InputEvent, WCC.OnGroundInputCallable);
    }

    void DisconnectInputEvent(Node node)
    {
        if (node.IsConnected(CollisionObject3D.SignalName.InputEvent, WCC.OnGroundInputCallable))
            node.Disconnect(CollisionObject3D.SignalName.InputEvent, WCC.OnGroundInputCallable);
    }

    public void UpdateBattle()
    {

        Show();
        CameraLookAt?.Invoke(AttackeDefenderDeployMarker.Position);
        LoadUnits();
        ConnectDeployZone();
    }

    void ConnectDeployZone()
    {
        switch (SpaceBattle.Local)
        {
            case SpaceBattle.HasLocal.Attacker:
                ConnectInputEvent(AttackerDeployZone);
                break;
            default:
                ConnectInputEvent(DefenderDeployZone);
                break;
        }
    }

    void LoadUnits()
    {
        var attacker = LoadUnits(SpaceBattle.Attackers, SpaceBattle.Attackers[0].Controller);
        var defender = LoadUnits(SpaceBattle.Defenders, SpaceBattle.Defenders[0].Controller);

        PlaceUnits(attacker, AttackeDefenderDeployMarker.Position);
        PlaceUnits(defender, DefenderDeployMarker.Position);
    }

    private List<Unit3D> LoadUnits(List<IEnterCombat> armies, Player controller)
    {
        var units = new List<Unit3D>();
        foreach (var army in armies)
        {
            foreach (var unit in army.UnitController.UnitsList)
            {
                var unit3D = UnitFactory.CreateUnit(this, Vector3.Zero, controller, unit);
                if (controller.IsLocal)
                {
                    unit3D.MovableState = IMovableState.Placement;
                }
                units.Add(unit3D);
            }
        }
        return units;
    }


    void PlaceUnits(List<Unit3D> units, Vector3 center)
    {
        int rows = (int)Mathf.Ceil(Mathf.Sqrt(units.Count));
        int cols = rows;
        int unitIndex = 0;

        for (int x = 0; x < cols; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                if (unitIndex >= units.Count)
                    return;

                var unit = units[unitIndex];
                var size = unit.GetUnitSize();
                Vector3 offset = new Vector3(x * (FormationSpacing + size.X), 0, z * (FormationSpacing + size.Z));
                Vector3 position = center + offset;

                unit.GlobalTransform = new Transform3D(Basis.Identity, position);

                unitIndex++;
            }
        }
    }

    public override void _Ready()
    {
        GetNodes();
    }

    private void GetNodes()
    {
        AttackeDefenderDeployMarker = GetNode<MeshInstance3D>("LDeployMarker");
        DefenderDeployMarker = GetNode<MeshInstance3D>("RDeployMarker");
        AttackerDeployZone = GetNode<Area3D>("LeftDeployZone");
        DefenderDeployZone = GetNode<Area3D>("RightDeployZone");
        UnitFactory = GetNode<UnitFactory>("UnitFactory");
        SpaceBattle = GetNode<SpaceBattle>("SpaceBattle");
        BoxSelectController = GetNode<BoxSelectController>("BoxSelectController");
        // LeftDeployZone/AttackerDeployZone
    }

    public Ship GetLocalAttakcerOrNull(int index = 0)
    {
        var s = SpaceBattle.Attackers[index];
        if (s.Controller.IsLocal && s is Ship ship)
            return ship;
        return null;
    }

    public void ConnectToEnterCombat(Node node)
    {
        node.Connect("EnterCombat", new Callable(this, nameof(_on_EnterCombat)));
    }

    public void ConnectToEnterCombat(Ship node)
    {
        node.EnterCombat += _on_EnterCombat;
    }

    public void CreateBattle(List<IEnterCombat> attackers, List<IEnterCombat> defenders, Node parent)
    {
        BoxSelectController.SetProcessInput(true);
        AttackeDefenderDeployMarker.Show();
        DefenderDeployMarker.Show();
        AttackerDeployZone.Show();
        DefenderDeployZone.Show();

        Combatants.Union(attackers);
        Combatants.Union(defenders);

        SpaceBattle.AddAttackers(attackers);
        SpaceBattle.AddDefenders(defenders);

    }

    void _on_EnterCombat(IEnterCombat ship, IEnterCombat enemy, Node parent)
    {
        if (ship != null && enemy != null)
        {
            if (ship != enemy)
            {
                if (!Combatants.Contains(ship) && !Combatants.Contains(enemy))
                {
                    CreateBattle(new System.Collections.Generic.List<IEnterCombat>() { ship }, new System.Collections.Generic.List<IEnterCombat>() { enemy }, parent);
                    OpenBattlePanel?.Invoke(SpaceBattle);
                    //battle.ConnectToOpenBattlePanel(this, nameof(_on_OpenBattlePanel));
                }
            }
        }
    }

    public void _on_Fight()
    {
        AttackeDefenderDeployMarker.Hide();
        DefenderDeployMarker.Hide();
        AttackerDeployZone.Hide();
        DefenderDeployZone.Hide();
        ChangeMovableState?.Invoke(IMovableState.Movement);
    }
}
