using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ManualBattleScene : Node3D
{

    [Export]
    public float FormationSpacing { get; set; } = 5;

    MeshInstance3D LDeployMarker;

    MeshInstance3D RDeployMarker;

    CollisionShape3D LCollisionShape3D;

    CollisionShape3D RCollisionShape3D;

    UnitFactory UnitFactory { get; set; }

    public SpaceBattle SpaceBattle { get; set; }

    public HashSet<Ship> Combatants { get; set; } = new HashSet<Ship>();

    public delegate void OpenBattlePanelEventHandler(SpaceBattle battle);

    public event OpenBattlePanelEventHandler OpenBattlePanel;

    public delegate void LookAtEventHandler(Godot.Vector3 target);

    public event LookAtEventHandler CameraLookAt;

    public void ClearCombat()
    {
        SpaceBattle.AcceptResult();
        Combatants.Clear();
        SpaceBattle.Attackers.Clear();
        SpaceBattle.Defenders.Clear();
    }

    public void InitializeBattle(Random random, ModelLoader modelLoader, WorldCursorControl worldCursorControl)
    {
        SpaceBattle.Rand = random;
        UnitFactory.ModelLoader = modelLoader;
        UnitFactory.WorldCursorControl = worldCursorControl;
    }

    public void ShowBattle()
    {
        Show();
        CameraLookAt?.Invoke(LDeployMarker.Position);
        LoadUnits();
    }

    void LoadUnits()
    {

        var attacker = LoadUnits(SpaceBattle.Attackers, SpaceBattle.Attackers[0].Controller);
        var defender = LoadUnits(SpaceBattle.Defenders, SpaceBattle.Defenders[0].Controller);

        PlaceUnits(attacker, LDeployMarker.Position);
        PlaceUnits(defender, RDeployMarker.Position);
    }

    private List<Unit3D> LoadUnits(List<IEnterCombat> armies, Player controller)
    { 
        var units = new List<Unit3D>();
        foreach (var army in armies)
        {
            foreach (var unit in army.UnitController.UnitsList)
            {
                units.Add(UnitFactory.CreateUnit(this,Vector3.Zero, controller, unit));
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
                Vector3 offset = new Vector3(x * (FormationSpacing+size.X), 0, z * (FormationSpacing+size.Z));
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
        LDeployMarker = GetNode<MeshInstance3D>("LDeployMarker");
        RDeployMarker = GetNode<MeshInstance3D>("RDeployMarker");
        LCollisionShape3D = GetNode<CollisionShape3D>("LeftDeployZone/LCollisionShape3D");
        RCollisionShape3D = GetNode<CollisionShape3D>("RightDeployZone/RCollisionShape3D");
        UnitFactory = GetNode<UnitFactory>("UnitFactory");
        SpaceBattle = GetNode<SpaceBattle>("SpaceBattle");
        // LeftDeployZone/LCollisionShape3D
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

    public void ConnectToEnterCombat(Ship node){
        node.EnterCombat += _on_EnterCombat;
    }

    public void CreateBattle(List<IEnterCombat> attackers, List<IEnterCombat> defenders, Node parent)
    {
        Combatants.Union(attackers);
        Combatants.Union(defenders);
        // var trans = SpaceBattle.Transform;
        // trans.Origin = ship.Transform.Origin;
        // SpaceBattle.Transform = trans;
        SpaceBattle.AddAttackers(attackers);
        SpaceBattle.AddDefenders(defenders);
        //HideNodes(ship, enemy);
        //parent.AddChild(SpaceBattle);
    }

    void _on_EnterCombat(IEnterCombat ship, IEnterCombat enemy, Node parent){
        if(ship != null && enemy != null){
            if(ship != enemy){
                if (!Combatants.Contains(ship) && !Combatants.Contains(enemy))
                {
                    CreateBattle(new System.Collections.Generic.List<IEnterCombat>() { ship }, new System.Collections.Generic.List<IEnterCombat>() { enemy }, parent);
                    OpenBattlePanel?.Invoke(SpaceBattle);
                    //battle.ConnectToOpenBattlePanel(this, nameof(_on_OpenBattlePanel));
                }
            }
        }
    }

}
