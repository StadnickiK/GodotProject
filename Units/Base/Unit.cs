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

	[Export]
    public Godot.Collections.Array<Vector3> ExportBatteries { get; set; } = new Godot.Collections.Array<Vector3>();

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

	public ModelData ModelData { get; protected set; }

    public CollisionObject3D GetAsSpecificNode => throw new System.NotImplementedException();

	public virtual IEnumerable<Unit> GetUnits()
	{
		yield return this;
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
	
	public virtual Unit Duplicate()
    {
		var unit = (Unit)base.Duplicate();
		unit.BuildCost = new System.Collections.Generic.Dictionary<int, int>(BuildCost);
		unit.BuildTime = BuildTime;
		unit.UnitName = UnitName;
        unit.Upkeep = new System.Collections.Generic.Dictionary<int, int>(Upkeep);
        unit.ModelVolume = ModelVolume;
        unit.ModelData = ModelData;
		return unit;
    }

	public virtual void LoadModelData(ModelLoader modelLoader)
	{
		ModelData = modelLoader.Models[ModelName];
	}

	public virtual void LoadModelData(Unit unit)
	{
		ModelData = unit.ModelData;
	}
}
