using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class Unit : Construct, IStatManager, IDamagable
{

	public event Node2DPool.FreeNodeEventHandler FreeUnit;

	[Export]
	public int ID_Owner { get; set; }

	[Export]
	public string ModelName { get; set; }

	public int ModelVolume { get; set; } = 0;

	public bool HasHitpoints { get { return GetStat(GlobalStatNames.Health).CurrentValue > 0; } } 

	public string UnitName { get; set; }

	public Vector3 GlobalPosition { get; set; }

	[Export]
	public Godot.Collections.Dictionary<string, int> ExportStats { get; set; } = new Godot.Collections.Dictionary<string, int>();

	public StatManager StatManager { get; set; }

	//public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

	public Node Stats { get; set; }

	public System.Collections.Generic.List<BaseStat> StatsList { get; set; } = new List<BaseStat>();

	public Turrets Turrets { get; set; }

	public ModelData ModelData { get; set; }

    public CollisionObject3D GetAsSpecificNode => throw new System.NotImplementedException();

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
		Turrets = GetNodeOrNull<Turrets>("Turrets");
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
	
	public Unit Duplicate()
    {
		var unit = (Unit)base.Duplicate();
		unit.BuildCost = new System.Collections.Generic.Dictionary<int, int>(BuildCost);
		unit.BuildTime = BuildTime;
		return unit;
    }
}
