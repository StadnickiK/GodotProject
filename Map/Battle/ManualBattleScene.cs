using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ManualBattleScene : Node3D, IProjectileFactory, IGameScene
{

    [Export]
    public float FormationSpacing { get; set; } = 5;

    [Export]
    public float VisionRangeBattlefieldSizeMultiplier { get; set; } = 8;

    [Export]
    public float DeploymentZoneWidhtMultiplier { get; set; } = 0.9f;

    [Export]
    public float DeploymentZoneHeightMultiplier { get; set; } = 0.2f;

    [Export]
    public float UnitHeightMultiplier { get; set; } = 3f;

    public Node GetAsNode { get { return this; } }

    public DeployZone AttackZone { get; set; }

    public DeployZone DefendZone { get; set; }

    public DeployZone BattleZone { get; set; }

    UnitFactory UnitFactory { get; set; }

    BattleUi battleUi;

    List<Action> SelectUnitActions = new List<Action>();

    public SpaceBattle SpaceBattle { get; set; }

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

    Dictionary<int, Unit3D> Models = new Dictionary<int, Unit3D>();

    Vector3 MaxUnitSize = Vector3.Zero;

    int MaxVisionRange = 0;

    List<Unit> LocalUnits = new List<Unit>();
    public ProjectileFactory ProjectileFactory { get; set; }

    IDamageCalculator DamageCalculator;

    ArmyView armyInterface;

    int LocalModels = 0;

    int EnemyModels = 0;

    public void ClearCombat()
    {
        BoxSelectController?.SetProcessInput(false);
        SpaceBattle.Combatants.Clear();
        // SpaceBattle.Attackers.Clear();
        // SpaceBattle.Defenders.Clear();
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
        MaxUnitSize = Vector3.Zero;
        MaxVisionRange = 0;
        Models.Clear();
        Unit3DModels.Clear();
    }

    public void EndCombat()
    {
        DisconnectInputEvent(BattleZone);
        if (LocalUnitModels.Count > 0)
        {
            // SpaceBattle.UpdateStats(AttackerModels, SpaceBattle.GetAttackerUnits());
            SpaceBattle.UpdateStats(Models.Values.ToList(), SpaceBattle.GetUnitsList());
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

    public void InitializeBattle(IWorld world)
    {
        SpaceBattle.Rand = world.Rand;
        UnitFactory.Data = world.Data;
        UnitFactory.WorldCursorControl = WCC =  world.WCC;
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

    public void UpdateBattle(BattlefieldSettings settings = null)
    {
        Show();
        BoxSelectController?.SetProcessInput(true);
        AttackZone.Show();
        DefendZone.Show();
        LoadUnits(settings);
        ConnectDeployZone();
        battleUi.Show();
        battleUi.UpdateBattleUI(LocalUnits);
        ConnectUnitCardsToSelect();
    }

    float GetMaxAxis(Vector3 vector3)
    {
        var max = Mathf.Max(vector3.X, vector3.Y);
        return Mathf.Max(max, vector3.Z);
    }

    void CalculateBattlefieldSize()
    {
        var max = GetMaxAxis(BattleZone.Size);
        max = Mathf.Max(max, VisionRangeBattlefieldSizeMultiplier * MaxVisionRange);  
        
        if(MaxUnitSize.Z * UnitHeightMultiplier > DeploymentZoneHeightMultiplier * max) // check if unit higher than deployment zone height
        {
            max = MaxUnitSize.Z * UnitHeightMultiplier * (1f/DeploymentZoneHeightMultiplier);
        }
        UpdateBattlefieldSize(new Vector3(max, 1, max));
    }

    void UpdateBattlefieldSize(Vector3 size)
    {
        AttackZone.UpdateSize(new Vector3(DeploymentZoneWidhtMultiplier*size.X, 1, DeploymentZoneHeightMultiplier*size.Z));
        DefendZone.UpdateSize(new Vector3(DeploymentZoneWidhtMultiplier*size.X, 1, DeploymentZoneHeightMultiplier*size.Z));
        var half = size.Z/2;
        BattleZone.UpdateSize(size);
        var z = (0.9f-DeploymentZoneHeightMultiplier) * half;
        AttackZone.UpdatePosition(new Vector3(0, 1, z));
        DefendZone.UpdatePosition(new Vector3(0, 1, -z));
    }

    private void ConnectUnitCardsToSelect()
    {
        foreach (var card in armyInterface.ArmyInterfaceContainer.ArmyPanel.UnitCards)
        {
            if (card.Visible)
            {
                //was WCC._SelectUnit
                void selectAction() => Select(LocalUnitModels[card.Index]);
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
                ConnectInputEvent(AttackZone);
                break;
            default:
                ConnectInputEvent(DefendZone);
                break;
        }
    }

    void LoadUnits(BattlefieldSettings settings)
    {
        //var units = LoadUnits(SpaceBattle.Combatants);
        
        var AttackerModels = LoadUnits(SpaceBattle.CombatantTeams[0]);
        var DefenderModels = LoadUnits(SpaceBattle.CombatantTeams[1]);
        
        Unit3DModels.UnionWith(AttackerModels);
        Unit3DModels.UnionWith(DefenderModels);
        
        if(settings == null)
            CalculateBattlefieldSize();
        else
            UpdateBattlefieldSize(settings.BattleZone);
        CameraLookAt?.Invoke(AttackZone.GlobalPosition);
        AttackZone.PlaceUnits(AttackerModels, AttackZone.Position);
        DefendZone.PlaceUnits(DefenderModels, DefendZone.Position, MathF.PI);
    }

    private List<Unit3D> LoadUnits(HashSet<IEnterCombatBase> armies)
    {
        var units = new List<Unit3D>();
        foreach (var army in armies)
        {
            foreach (var unit in army.UnitController.UnitsList)
            {
                var unit3D = UnitFactory.CreateUnit(this, Vector3.Zero, army.Controller, unit);
                unit3D.Name = unit.UnitName + " " + units.Count;
                CheckUnitSize(unit3D.GetUnitSize());
                CheckVisionRange(unit3D.StatManager.GetStat("Vision Range").CurrentValue);
                if (army.Controller.IsLocal)
                {
                    LocalUnits.Add(unit);
                    unit3D.MovableState = IMovableState.Placement;
                    ChangeMovableState -= unit3D.ChangeMovableState;
                    ChangeMovableState += unit3D.ChangeMovableState;
                    LocalUnitModels.Add(unit3D);
                    LocalModels++;
                }
                else
                {
                    EnemyModels++;
                }
                unit3D.SaveNode -= _on_Death;
                unit3D.SaveNode += _on_Death;
                units.Add(unit3D);
            }
        }
        return units;
    }

    void CheckUnitSize(Vector3 size)
    {
        if (size.X > MaxUnitSize.X)
            size.X = MaxUnitSize.X;
        if (size.Y > MaxUnitSize.Y)
            size.Y = MaxUnitSize.Y;
        if (size.Z > MaxUnitSize.Z)
            size.Z = MaxUnitSize.Z;
    }

    void CheckVisionRange(float range)
    {
        if (range > MaxVisionRange)
            MaxVisionRange = Mathf.CeilToInt(range);
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
        AttackZone = GetNode<DeployZone>("AttackZone");
        DefendZone = GetNode<DeployZone>("DefendZone");
        BattleZone = GetNode<DeployZone>("BattleZone");
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

    public Ship GetLocalOrNull()
    {
        foreach (var s in SpaceBattle.Combatants)
        {
            if (s.Controller.IsLocal && s is Ship ship)
                return ship;
        }      
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

    // public void CreateBattle(List<IEnterCombatBase> attackers, List<IEnterCombatBase> defenders, Node parent)
    // {

    //     Combatants.Union(attackers);
    //     Combatants.Union(defenders);

    //     SpaceBattle.AddAttackers(attackers);
    //     SpaceBattle.AddDefenders(defenders);
    // }

    public void CreateBattle(HashSet<Combatant> combatants, Node parent)
    {
        SpaceBattle.Combatants.Union(combatants);
        foreach (var item in combatants)
        {
            if (SpaceBattle.CombatantTeams.ContainsKey(item.Team))
            {
                SpaceBattle.CombatantTeams[item.Team].Add(item);
            }else
            {
                SpaceBattle.CombatantTeams.Add(item.Team, new HashSet<IEnterCombatBase>(){ item });
            }
        }
    }

    void _on_EnterCombat(IEnterCombatBase ship, IEnterCombatBase enemy, Node parent)
    {
        if (ship != null && enemy != null)
        {
            if (ship != enemy)
            {
                if (!SpaceBattle.Combatants.Contains(ship) && !SpaceBattle.Combatants.Contains(enemy))
                {
                    CreateBattle(OrganizeCombatants(new HashSet<IEnterCombatBase>(){ship, enemy}), parent);
                    //CreateBattle(new System.Collections.Generic.List<IEnterCombatBase>() { ship }, new System.Collections.Generic.List<IEnterCombatBase>() { enemy }, parent);
                    OpenBattlePanel?.Invoke(SpaceBattle);
                    //battle.ConnectToOpenBattlePanel(this, nameof(_on_OpenBattlePanel));
                }
            }
        }
    }


    private HashSet<Combatant> OrganizeCombatants(HashSet<IEnterCombatBase> enterCombatBases)
    {
        var scene = ResourceLoader.Load<PackedScene>(ScenePaths.Instance.CombatantScene);
        var set = new HashSet<Combatant>();
        int team = 0;
        foreach (var combatBase in enterCombatBases)
        {
            var c = scene.Instantiate<Combatant>();
            c.Controller = combatBase.Controller;
            c.UnitController = combatBase.UnitController;
            c.Team = team;
            set.Add(c);
            team++;
        }
        return set;
    }

    public void _on_Fight()
    {
        AttackZone.Hide();
        DefendZone.Hide();
        DisconnectInputEvent(AttackZone);
        DisconnectInputEvent(DefendZone);
        ConnectInputEvent(BattleZone);
        ChangeMovableState?.Invoke(IMovableState.Movement);
    }

    void FinishBattle()
    {
        OpenBattlePanel?.Invoke(SpaceBattle);
        SpaceBattle.InvokeBattleFinished();
        battleUi.Hide();
        var local = GetLocalOrNull();
        if(local != null) CameraLookAt?.Invoke(local.GlobalPosition);
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

    public void _on_Deselect()
	{
		WCC.ClearSelection();
	}

    public void Select(ISelection planet)
    {
        _on_Deselect();
		WCC._SelectUnit(planet);
    }

    public void SplitShip(ShipModel shipStruct)
    {
        throw new NotImplementedException();
    }

    public void InitializeScene(IWorld world)
    {
        CameraLookAt += world.Camera3D.LookAt;
        InitializeBattle(world);
        LoadPlayers(world.WorldGenParameters.Combatants, world);
        world.Host = world.WorldGenParameters.Combatants.FirstOrDefault().Controller;
        // CreateBattle(world.WorldGenParameters.Attackers, world.WorldGenParameters.Defenders, world.GetAsNode);
        CreateBattle(world.WorldGenParameters.Combatants, world.GetAsNode);
        UpdateBattle(world.WorldGenParameters.BattlefieldSettings);
    }

    void LoadPlayers(HashSet<Combatant> bases, IWorld world)
    {
        foreach (var item in bases)
        {
            world.AddPlayer(item.Controller);
            if(item.GetAsNode.GetParent() == null) world.GetAsNode.AddChild(item.GetAsNode);
        }
    }
}
