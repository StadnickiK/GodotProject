using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class Unit : Construct, IUpkeep, IStatManager, IDamagable
{

	public event Node2DPool.FreeNodeEventHandler FreeUnit;

	[Export]
	public int ID_Owner { get; set; }

	[Export]
	public string ModelName { get; set; }

	public int ModelVolume { get; set; } = 0;

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

    public void Damage()
    {
        throw new System.NotImplementedException();
    }
}
