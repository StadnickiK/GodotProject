using Godot;
using System;
using System.Collections.Generic;
using System.Linq;




public partial class SpaceBattle : StaticBody3D, ISelectMapObject
{

    // public List<PhysicsBody> Comabatants { get; set; } = new List<PhysicsBody>();

    public event World.FreeShipEventHandler FreeShip;

    public event Node2DPool.FreeNodeEventHandler FreeUnit;

    public event Node2DPool.FreeNodeEventHandler FreeBattle;

    public delegate void BattleEventHandler(SpaceBattle battle);

    public event BattleEventHandler BattleFinished;

    public Node Participants = null;

    GameLogger gameLogger = GameLogger.Instance;
    // public Ship Attacker { get; set; } = null;

    [Export]
    public bool Logging { get; set; } = true;

    public List<Ship> Attackers { get; set; } = new List<Ship>();

    public float AttackPower { get; set; }

    // public Ship Defender { get; set; } = null;

    public Random Rand { get; set; } = new Random();

    public List<Ship> Defenders { get; set; } = new List<Ship>();

    public float DefPower { get; set; }

    public bool IsLocal { get; set; } = false;

    bool ZeroOne = false;

    bool _endCombat = false;

    public bool PowerChanged { get; set; } = false;

    public delegate void OpenBattlePanelEventHandler(SpaceBattle battle);

    public event OpenBattlePanelEventHandler OpenBattlePanel;

    MeshInstance3D _placeholder = null;

    Node3D _mesh = null;

    double _time = 0;

    public int TimeStep { get; set; } = 1;

    public new void SetPosition(Vector3 pos)
    {
        var trans = Transform;
        trans.Origin = pos;
        Transform = trans;
    }

    public void AddAttackers(List<Ship> attackers)
    {
        Attackers.AddRange(attackers);
    }

    public void AddDefenders(List<Ship> defenders)
    {
        Defenders.AddRange(defenders);
    }

    void GetNodes()
    {
        Participants = GetNode("Participants");
        _placeholder = GetNode<MeshInstance3D>("Placeholder");
        _mesh = GetNode<Node3D>("Node3D");
    }

    public override void _Ready()
    {
        GetNodes();
        //GenerateMesh();
        //InitAttackers();
        //InitDefenders();
    }

    void InitAttackers()
    {
        foreach (var attacker in Attackers)
        {
            attacker.GetParent().RemoveChild(attacker);
            Participants.AddChild(attacker);
            AttackPower += attacker.Power;
        }
    }

    void UpdateAttackPower()
    {
        foreach (var attacker in Attackers)
        {
            AttackPower += attacker.Power;
        }
    }

    void InitDefenders()
    {
        foreach (var defender in Defenders)
        {
            defender.GetParent().RemoveChild(defender);
            Participants.AddChild(defender);
            DefPower += defender.Power;
        }
    }

    void UpdateDefPower()
    {
        foreach (var defender in Defenders)
        {
            DefPower += defender.Power;
        }
    }

    void UpdatePower()
    {
        UpdateAttackPower();
        UpdateDefPower();
    }

    List<Unit> GetUnits(List<Ship> ships)
    {
        var list = new List<Unit>();
        foreach (var s in ships)
            list.AddRange(s.Units.UnitsList);
        return list;
    }

    public void AutoFight()
    {
        if(Logging) gameLogger.LogInfo("Auto Fight start");

        var attackers = GetUnits(Attackers);
        var defenders = GetUnits(Defenders);
        
        int round = 1;
        do
        {
            if (Logging)
            {
                gameLogger.LogInfo("Attackers count =" + attackers.Count + "");
                gameLogger.LogInfo("Defenders count =" + defenders.Count + "");
                gameLogger.LogInfo("Round =" + round + "");
                gameLogger.LogInfo("Attackers vs Defenders, round " + round + "");
            } 
            Combat(attackers, defenders);
            if (Logging) gameLogger.LogInfo("Defenders vs Attackers, round " + round + "");
            Combat(defenders, attackers);
            round++;
        } while (attackers.Count > 0 && defenders.Count > 0);
        if (Logging)
        {
            gameLogger.LogInfo("Auto Fight end");
            gameLogger.LogInfo("Attackers count =" + attackers.Count + "");
            gameLogger.LogInfo("Defenders count =" + defenders.Count + "");
        } 
        BattleFinished?.Invoke(this);
    }

    void Combat(List<Unit> attackers, List<Unit> defenders)
    {
        //var attackers = attackingGroups.Where(g => g.HasHitpoints);
        //var defenders = defendingGroups.Where(g => g.HasHitpoints).ToList();
        int attackerID = 0;
        foreach (var attacker in attackers)
        {
            attackerID++;
            if (defenders.Count == 0) break;
            if(Logging) gameLogger.LogInfo("Attacker " + attackerID);
            var target = defenders.OrderBy(e => Rand.Next()).LastOrDefault();
            attacker.CalculateDamage(target);

            // Optional: re-filter in case someone died
            if (!target.HasHitpoints)
            {
                defenders.RemoveAt(defenders.Count - 1);
                if(Logging) gameLogger.LogInfo("Remove target from combat, defenders left " + defenders.Count);
            }
            //defenders = defendingGroups.Where(g => g.HasHitpoints).ToList();
        }
    }

    public void AcceptResult()
    {
        CleaDeadUnits();
    }

    void CleaDeadUnits()
    {
        for (int i = Attackers.Count - 1; i >= 0; i--)
        {
            Attackers[i].Units.ClearUnits();
            if (!Attackers[i].Units.HasUnits) Attackers.RemoveAt(i);
        }
        for (int i = Defenders.Count - 1; i >= 0; i--)
        {
            Defenders[i].Units.ClearUnits();
            if (!Defenders[i].Units.HasUnits) Defenders.RemoveAt(i);
        }
    }

    public void SelectMapObject()
    {
        EmitSignal(nameof(OpenBattlePanel), (PhysicsBody3D)this);
    }

    public void _on_SpaceBattle_input_event(Camera3D camera, InputEvent input, Vector3 clickPosition, Vector3 clickNormal, int index)
    {
        if (input is InputEventMouseButton eventMouseButton)
        {
            switch (eventMouseButton.ButtonIndex)
            {
                case MouseButton.Left:
                    SelectMapObject();
                    break;
                case MouseButton.Right:
                    //EmitSignal(nameof(Ship.SelectTarget), (PhysicsBody)this);
                    break;
            }
        }
    }
}
