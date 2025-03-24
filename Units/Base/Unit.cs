using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class Unit : Node, IBuilding, IUpkeep
{

	[Export]
	public int ID_Owner { get; set; }

	public bool HasHitpoints { get; set; } = true;  

	[Export]
	public int BuildTime { get; set; } = 10;

	public string UnitName { get; set; }

	public int CurrentTime { get; set; } = 0;

    [Export]
    public Godot.Collections.Dictionary<int, Array<string>> ExportRequirements { get; set; } = new Godot.Collections.Dictionary<int, Array<string>>();

     public System.Collections.Generic.Dictionary<int, List<int>> Requirements { get; set; } = new System.Collections.Generic.Dictionary<int, List<int>>();

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportBuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public System.Collections.Generic.Dictionary<int, int> BuildCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportUpkeep { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public System.Collections.Generic.Dictionary<int, int> Upkeep { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

	//public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

	public Node Stats { get; set; }

	public System.Collections.Generic.List<BaseStat> StatsList { get; set; } = new List<BaseStat>();
    string IBuilding.Name { get; set; }

    // World - initStartFleets, InitResistance
    public Unit(){  
	}

	public Unit(string name, Array<BaseStat> stats){
		Name = name;
		foreach(BaseStat stat in stats){
			Stats.AddChild(stat);
		}
	}

	void CopyStats(Node stats){
		foreach(Node node in stats.GetChildren()){
			if(node is BaseStat stat){
				var statCopy = new BaseStat(stat);
				Stats.AddChild(statCopy);
			}
		}
	}

	public void CalculateDamage(Unit unit){
		if(GetStat("Attack").BaseValue > unit.GetStat("Defence").BaseValue){
			unit.GetStat("HitPoints").CurrentValue -= GetStat("Attack").BaseValue - unit.GetStat("Defence").BaseValue;
		}else{
			unit.GetStat("HitPoints").CurrentValue--; // if defence is higher than attack deal minimal dmg
		}
		if(GetStat("HitPoints").CurrentValue<0){
			HasHitpoints = false;
		}
		if(unit.GetStat("HitPoints").CurrentValue<=0){
			unit.HasHitpoints = false;
		}else{
			if(unit.GetStat("Attack").BaseValue > GetStat("Defence").BaseValue){
				GetStat("HitPoints").CurrentValue -= unit.GetStat("Attack").BaseValue - GetStat("Defence").BaseValue;
			}else{
				GetStat("HitPoints").CurrentValue--; // if defence is higher than attack deal minimal dmg
			}
			// GetStat("HitPoints").CurrentValue -= unit.GetStat("Attack").BaseValue - GetStat("Defence").BaseValue;
		}
	}

	public override void _Ready()
	{
		Stats = GetNode("Stats");
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
		return GetNodeOrNull<BaseStat>("Stats/"+name);
	}

}
