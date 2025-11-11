using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ManualBattleScene : Node3D, IProjectileFactory
{

    [Export]
    public float FormationSpacing { get; set; } = 5;

    MeshInstance3D AttackerDeployMarker;

    MeshInstance3D DefenderDeployMarker;

    Area3D AttackerDeployZone;

    Area3D DefenderDeployZone;

    Area3D BattleZone;

    UnitFactory UnitFactory { get; set; }

    BattleUi battleUi;

    List<Action> SelectUnitActions = new List<Action>();

    public SpaceBattle SpaceBattle { get; set; }

    public HashSet<Ship> Combatants { get; set; } = new HashSet<Ship>();

    public delegate void OpenBattlePanelEventHandler(SpaceBattle battle);

    public event OpenBattlePanelEventHandler OpenBattlePanel;

    public delegate void LookAtEventHandler(Godot.Vector3 target);

    public event LookAtEventHandler CameraLookAt;

    public delegate void EndBattleEventHandler();

    public event EndBattleEventHandler EndBattle;

    public delegate void ChangeMovableStateEventHandler(IMovableState movableState);

    public event ChangeMovableStateEventHandler ChangeMovableState;

    public WorldCursorControl WCC { get; set; }

    public BoxSelectController BoxSelectController { get; set; }

    public List<Unit3D> LocalUnitModels { get; set; } = new List<Unit3D>();

    HashSet<Unit3D> Unit3DModels = new HashSet<Unit3D>();

    List<Unit3D> AttackerModels = new List<Unit3D>();

    List<Unit3D> DefenderModels = new List<Unit3D>();

    List<Unit> LocalUnits = new List<Unit>();
    public ProjectileFactory ProjectileFactory { get; set; }

    IDamageCalculator DamageCalculator;

    ArmyView armyInterface;

    int LocalModels = 0;

    int EnemyModels = 0;

    public void ClearCombat()
    {
        BoxSelectController?.SetProcessInput(false);
        Combatants.Clear();
        SpaceBattle.Attackers.Clear();
        SpaceBattle.Defenders.Clear();
        LocalUnitModels.Clear();
        LocalUnits.Clear();
        FreeModels();
        battleUi.Hide();
        DisconnectUnitCardsToSelect();
    }

    void FreeModels()
    {
        foreach (var model in Unit3DModels)
        {
            model.SaveNode -= _on_Death;
            model.InvokeSaveNode();
        }
        AttackerModels.Clear();
        DefenderModels.Clear();
        Unit3DModels.Clear();
    }

    public void EndCombat()
    {
        DisconnectInputEvent(BattleZone);
        if (LocalUnitModels.Count > 0)
        {
            SpaceBattle.UpdateStats(AttackerModels, SpaceBattle.GetAttackerUnits());
            SpaceBattle.UpdateStats(DefenderModels, SpaceBattle.GetDefenderUnits());
        } 
        SpaceBattle.AcceptResult();
        ClearCombat();
    }

    public void ResetCombat()
    {
        LocalUnits.Clear();
        FreeModels();
        UpdateBattle();
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
        CameraLookAt?.Invoke(AttackerDeployMarker.GlobalPosition);
        BoxSelectController?.SetProcessInput(true);
        AttackerDeployMarker.Show();
        DefenderDeployMarker.Show();
        AttackerDeployZone.Show();
        DefenderDeployZone.Show();
        LoadUnits();
        ConnectDeployZone();
        battleUi.Show();
        battleUi.UpdateBattleUI(LocalUnits);
        ConnectUnitCardsToSelect();
    }

    private void ConnectUnitCardsToSelect()
    {
        foreach (var card in armyInterface.ArmyInterfaceContainer.ArmyPanel.UnitCards)
        {
            if (card.Visible)
            {
                void selectAction() => WCC._SelectUnit(LocalUnitModels[card.Index]);
                card.button.ButtonUp += selectAction;
                SelectUnitActions.Add(selectAction);
                LocalUnitModels[card.Index].CardIndex = card.Index;
            }
        }
    }

    void DisconnectUnitCardsToSelect()
    {
        if(SelectUnitActions.Count > 0)
            for (int i = 0; i < armyInterface.ArmyInterfaceContainer.ArmyPanel.UnitCards.Count; i++)
            {
                UnitCard card = armyInterface.ArmyInterfaceContainer.ArmyPanel.UnitCards[i];

                if (card.Visible)
                    card.button.ButtonUp -= SelectUnitActions[i];
            }
        SelectUnitActions.Clear();
    }

    void ConnectDeployZone()
    {
        DisconnectInputEvent(BattleZone);
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
        AttackerModels = LoadUnits(SpaceBattle.Attackers, SpaceBattle.Attackers[0].Controller);
        DefenderModels = LoadUnits(SpaceBattle.Defenders, SpaceBattle.Defenders[0].Controller);
        Unit3DModels.UnionWith(AttackerModels);
        Unit3DModels.UnionWith(DefenderModels);
        PlaceUnits(AttackerModels, AttackerDeployMarker.Position);
        PlaceUnits(DefenderModels, DefenderDeployMarker.Position);
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
                    LocalUnits.Add(unit);
                    unit3D.MovableState = IMovableState.Placement;
                    ChangeMovableState += unit3D.ChangeMovableState;
                    LocalUnitModels.Add(unit3D);
                    LocalModels++;
                }
                else
                {
                    EnemyModels++;
                }
                unit3D.SaveNode += _on_Death;
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
                Vector3 offset = new Vector3(x * (FormationSpacing + size.X + (size.Z/2)), 1, z * (FormationSpacing + size.Z + size.X));
                Vector3 position = center + offset;

                unit.GlobalTransform = new Transform3D(Basis.Identity, position); 

                unitIndex++;
            }
        }
    }

    public override void _Ready()
    {
        GetNodes();
        BoxSelectController.LocalUnits = LocalUnitModels;
        BoxSelectController?.SetProcessInput(false);
        battleUi.Fight.ButtonUp += _on_Fight;
        UnitFactory.ProjectileFactory = ProjectileFactory;
        ProjectileFactory.ProjectileParent = this;
        ProjectileFactory.DamageCalculator = DamageCalculator;
        SpaceBattle.DamageCalculator = DamageCalculator;
    }

    private void GetNodes()
    {
        AttackerDeployMarker = GetNode<MeshInstance3D>("LDeployMarker");
        DefenderDeployMarker = GetNode<MeshInstance3D>("RDeployMarker");
        AttackerDeployZone = GetNode<Area3D>("LeftDeployZone");
        DefenderDeployZone = GetNode<Area3D>("RightDeployZone");
        BattleZone = GetNode<Area3D>("BattleZone");
        UnitFactory = GetNode<UnitFactory>("UnitFactory");
        SpaceBattle = GetNode<SpaceBattle>("SpaceBattle");
        BoxSelectController = GetNodeOrNull<BoxSelectController>("BoxSelectController");
        battleUi = GetNode<BattleUi>("BattleUI");
        armyInterface = GetNode<ArmyView>("BattleUI/ArmyInterface");
        ProjectileFactory = GetNode<ProjectileFactory>("ProjectileFactory");
        DamageCalculator = GetNode<DamageCalculator>("DamageCalculator");
        // LeftDeployZone/AttackerDeployZone
    }

    public void LoadUI(UI uI)
    {
        battleUi.ArmyView.ArmyInterfaceContainer.ArmyPanel.UI = uI;
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
        if (!node.IsConnected("EnterCombat", new Callable(this, nameof(_on_EnterCombat))))
            node.Connect("EnterCombat", new Callable(this, nameof(_on_EnterCombat)));
    }

    public void ConnectToEnterCombat(Ship node)
    {
        node.EnterCombat -= _on_EnterCombat;
        node.EnterCombat += _on_EnterCombat;
    }

    public void CreateBattle(List<IEnterCombat> attackers, List<IEnterCombat> defenders, Node parent)
    {

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
        AttackerDeployMarker.Hide();
        DefenderDeployMarker.Hide();
        AttackerDeployZone.Hide();
        DefenderDeployZone.Hide();
        DisconnectInputEvent(AttackerDeployZone);
        DisconnectInputEvent(DefenderDeployZone);
        ConnectInputEvent(BattleZone);
        ChangeMovableState?.Invoke(IMovableState.Movement);
    }

    void FinishBattle()
    {
        OpenBattlePanel?.Invoke(SpaceBattle);
        SpaceBattle.InvokeBattleFinished();
        battleUi.Hide();
        CameraLookAt?.Invoke(SpaceBattle.Attackers[0].GlobalPosition);
        EndBattle?.Invoke();
    }

    void _on_Death(Node node)
    {
        if (node is ICardIndex unit)
        {
            if (unit.Controller.IsLocal)
            {
                LocalModels--;
            }
            else
            {
                EnemyModels--;
            }
            if (LocalModels <= 0 || EnemyModels <= 0)
                FinishBattle();
        }
    }
}
