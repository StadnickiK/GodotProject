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

	GameLogger gameLogger = GameLogger.Instance;
	// public Ship Attacker { get; set; } = null;

	[Export]
	public bool Logging { get; set; } = true;

	public List<IEnterCombat> Attackers { get; set; } = new List<IEnterCombat>();

	public float AttackPower { get; set; }

	// public Ship Defender { get; set; } = null;

	public Random Rand { get; set; } = new Random();

	public List<IEnterCombat> Defenders { get; set; } = new List<IEnterCombat>();

	public float DefPower { get; set; }

	public bool PowerChanged { get; set; } = false;

	public delegate void OpenBattlePanelEventHandler(SpaceBattle battle);

	public event OpenBattlePanelEventHandler OpenBattlePanel;

	public enum HasLocal
	{
		None,
		Attacker,
		Defender
	}
	public HasLocal Local { get; private set; } = SpaceBattle.HasLocal.None;

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

	public void AddAttackers(List<IEnterCombat> attackers)
	{
		if (attackers.FirstOrDefault(x => x.Controller.IsLocal) != null) Local = HasLocal.Attacker;
		Attackers.AddRange(attackers);
	}

	public void AddDefenders(List<IEnterCombat> defenders)
	{
		if (defenders.FirstOrDefault(x => x.Controller.IsLocal) != null) Local = HasLocal.Defender;
		Defenders.AddRange(defenders);
	}

	void GetNodes()
	{
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

	HashSet<Unit> GetUnits(List<IEnterCombat> ships)
	{
		var set = new HashSet<Unit>();
		foreach (var s in ships)
			set.UnionWith(s.UnitController.UnitsList);
		return set;
	}
	
	List<Unit> GetUnitsList(List<IEnterCombat> ships)
	{
		var set = new List<Unit>();
		foreach (var s in ships)
			set.AddRange(s.UnitController.UnitsList);
		return set;
	}

	public List<Unit> GetLocalUnits()
	{
		switch (Local)
		{
			case HasLocal.Attacker:
				return GetUnitsList(Attackers);
			default:
				return GetUnitsList(Defenders);
		}
	}

	public List<Unit> GetAttackerUnits()
	{
		return GetUnitsList(Attackers);
	}

	public List<Unit> GetDefenderUnits()
	{
		return GetUnitsList(Defenders);
	}


	public void AutoFight()
	{
		if (Logging) gameLogger.LogInfo("Auto Fight start");

		var attackers = GetUnits(Attackers);
		var defenders = GetUnits(Defenders);

		int round = 1;
		while (attackers.Count > 0 && defenders.Count > 0)
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
		}
		if (Logging)
		{
			gameLogger.LogInfo("Auto Fight end");
			gameLogger.LogInfo("Attackers count =" + attackers.Count + "");
			gameLogger.LogInfo("Defenders count =" + defenders.Count + "");
		}
		BattleFinished?.Invoke(this);
	}

	void Combat(HashSet<Unit> attackers, HashSet<Unit> defenders)
	{
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
				defenders.Remove(target);
				if(Logging) gameLogger.LogInfo("Remove target from combat, defenders left " + defenders.Count);
			}
		}
	}

	public void AcceptResult()
	{
		CleanDeadUnits();
		Local = HasLocal.None;
	}

	void CleanDeadUnits()
	{
        if (Logging) gameLogger.LogInfo("Cleaning dead units");
        if (Logging) gameLogger.LogInfo("Attackers");
        CleanDeadUnits(Attackers);
        if (Logging) gameLogger.LogInfo("Defenders");
        CleanDeadUnits(Defenders);
	}

    void CleanDeadUnits(List<IEnterCombat> ships)
    {
        for (int i = ships.Count - 1; i >= 0; i--)
        {
            if (Logging) gameLogger.LogInfo("Ship " + i + " count " + Attackers[i].UnitController.Count);
            ships[i].UnitController.ClearUnits();
            if (Logging) gameLogger.LogInfo("Ship " + i + " clean count " + Attackers[i].UnitController.Count);
            if (!ships[i].UnitController.HasUnits) ships.RemoveAt(i);
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
