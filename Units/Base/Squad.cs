using Godot;
using System;
using System.Collections.Generic;

public partial class Squad : Unit
{
	public List<Unit> Units { get; set; } = new List<Unit>();

	public override IEnumerable<Unit> GetUnits() => Units;

	public int ModelCount { private set; get; } = 0;

	Node UnitsParent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
		UnitsParent = GetNode("Units");
		foreach (var item in UnitsParent.GetChildren())
		{
			Units.Add((Unit)item);
		}
		ModelCount = Units.Count;
    }

    public override Unit Duplicate()
    {
        var unit = (Squad)base.Duplicate();
		// foreach (var item in Units)
		// {
		// 	unit.AddDuplicateUnit(item.Duplicate());
		// }
		unit.BuildCost = new System.Collections.Generic.Dictionary<int, int>(BuildCost);
		unit.BuildTime = BuildTime;
		unit.UnitName = UnitName;
        unit.Upkeep = new System.Collections.Generic.Dictionary<int, int>(Upkeep);
        unit.ModelVolume = ModelVolume;
        unit.ModelData = ModelData;
		return unit;
    }

    public override void LoadModelData(ModelLoader modelLoader)
    {
        base.LoadModelData(modelLoader);
		foreach (var item in Units)
		{
			item.LoadModelData(modelLoader);
		}
    }

    public override void LoadModelData(Unit unit)
    {
        base.LoadModelData(unit);
		if(unit is Squad squad)
		{
            for (int i = 0; i < squad.Units.Count; i++)
			{
                Unit item = Units[i];
                item.LoadModelData(squad.Units[i]);
			}
		}
		else
		{
			foreach (var item in Units)
			{
				item.LoadModelData(unit);
			}
		}
    }


	public void AddUnit(Unit unit)
	{
		if(unit.GetParent() != this) unit.Reparent(this);
		AddUpkeep(unit);
		AddStats(unit);
		Units.Add(unit);
		ModelCount = Units.Count;
	}

	public void AddDuplicateUnit(Unit unit)
	{
		if(unit.GetParent() != this) unit.Reparent(this);
		AddUpkeep(unit);
		Units.Add(unit);
		ModelCount = Units.Count;
	}

	void AddUpkeep(Unit unit)
	{
		foreach (var item in unit.Upkeep)
		{
			if(Upkeep.ContainsKey(item.Key))
				Upkeep[item.Key] += item.Value;
			else
				Upkeep.Add(item.Key, item.Value);	
		}
	}

	void RemoveUpkeep(Unit unit)
	{
		foreach (var item in unit.Upkeep)
		{
			if(Upkeep.ContainsKey(item.Key))
				Upkeep[item.Key] -= item.Value;
		}
	}

	void AddStats(Unit unit)
	{
		if (unit.StatManager.HasStat(GlobalStatNames.Health))
		{
			StatManager.GetStat(GlobalStatNames.Health).CurrentValue += unit.StatManager.GetStat(GlobalStatNames.Health).CurrentValue;
			StatManager.GetStat(GlobalStatNames.Health).MaxValue += unit.StatManager.GetStat(GlobalStatNames.Health).MaxValue;
		}
		// else
		// {
		// 	StatManager.AddStat(unit.StatManager.GetStat(GlobalStatNames.Health))
		// }
	}

	public void RemoveUnit(Unit unit)
	{
		RemoveUpkeep(unit);
		Units.Remove(unit);
		ModelCount = Units.Count;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
