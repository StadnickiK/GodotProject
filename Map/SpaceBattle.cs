using Godot;
using System;
using System.Collections.Generic;
using System.Linq;




public partial class SpaceBattle : Node
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

	public IDamageCalculator DamageCalculator { get; set; }

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
		InvokeBattleFinished();
	}

	public void InvokeBattleFinished()
	{
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
			DamageCalculator.CalculateDamage(attacker, target);

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

	public void UpdateStats(List<Unit3D> unit3Ds, List<Unit> units)
	{
		for (int i = 0; i < units.Count; i++)
		{
			units[i].StatManager.CloneStats(unit3Ds[i]);
		}
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
            if (Logging) gameLogger.LogInfo("Ship " + i + " count " + ships[i].UnitController.Count);
            ships[i].UnitController.ClearUnits();
            if (Logging) gameLogger.LogInfo("Ship " + i + " clean count " + ships[i].UnitController.Count);
            if (!ships[i].UnitController.HasUnits) ships.RemoveAt(i);
        }
    }
}
