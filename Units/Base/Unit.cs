using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class Unit : Construct, IUpkeep, IStatManager
{

	public event Node2DPool.FreeNodeEventHandler FreeUnit;

	[Export]
	public int ID_Owner { get; set; }

	[Export]
	public bool Logging { get; set; } = true;

	[Export]
	public string ModelName { get; set; }

	public int ModelVolume { get; set; } = 0;

	GameLogger gameLogger = GameLogger.Instance;

	public bool HasHitpoints { get; private set; } = true;  

	public string UnitName { get; set; }

	[Export]
	public Godot.Collections.Dictionary<string, int> ExportUpkeep { get; set; } = new Godot.Collections.Dictionary<string, int>();

	public System.Collections.Generic.Dictionary<int, int> Upkeep { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

	[Export]
	public Godot.Collections.Dictionary<string, int> ExportStats { get; set; } = new Godot.Collections.Dictionary<string, int>();

	public StatManager StatManager { get; set; }



	//public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

	public Node Stats { get; set; }

	public System.Collections.Generic.List<BaseStat> StatsList { get; set; } = new List<BaseStat>();

	// World - initStartFleets, InitResistance
	public Unit(){  
	}

	public Unit(string name, Array<BaseStat> stats){
		Name = name;
		foreach(BaseStat stat in stats){
			Stats.AddChild(stat);
		}
	}

	public void CalculateDamage(Unit unit){
		var attack = GetStat("Attack");
		var unitDefence = unit.GetStat("Defence");
		var unitHP = unit.GetStat("HitPoints");
		if(Logging) gameLogger.LogInfo("Attack " + attack.CurrentValue + "");
		if(Logging) gameLogger.LogInfo("Target Defence " + unitDefence.CurrentValue + " Target HP " + unitHP.CurrentValue);
		if (attack.CurrentValue > unitDefence.CurrentValue)
		{
			var EffectiveAttack = attack.CurrentValue - unitDefence.CurrentValue;
			unitHP.CurrentValue -= EffectiveAttack;
			if (Logging) gameLogger.LogInfo("Damage dealt " + EffectiveAttack + " Target HP " + unitHP.CurrentValue);
		}
		else
		{
			unitHP.CurrentValue -= 1; // if defence is higher than attack deal minimal dmg
			if (Logging) gameLogger.LogInfo("Damage dealt " + 1 + " Target HP " + unitHP.CurrentValue);
		}
		if (unitHP.CurrentValue <= 0)
		{
			unit.HasHitpoints = false;
			if (Logging) gameLogger.LogInfo("Target has no HP, hasHP "+unit.HasHitpoints);
			//FreeUnit?.Invoke(this);
		}
	}

	public override void _Ready()
	{
		StatManager = GetNode<StatManager>("StatManager");
	}

	/// <summary>
	/// Returns requested stat or null
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns> <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public BaseStat GetStat(string name){
		return StatManager.GetNodeOrNull<BaseStat>(name);
	}

}
