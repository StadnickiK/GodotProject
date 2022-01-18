using Godot;
using System;
using System.Collections.Generic;

public class Unit : Node, IBuilding, IUpkeep
{

    [Export]
    public int ID_Owner { get; set; }

    public bool HasHitpoints { get; set; } = true;  

    [Export]
    public int BuildTime { get; set; } = 0;

    public int CurrentTime { get; set; } = 0;

    [Export]
    public Godot.Collections.Dictionary<string, string[]> Requirements { get; set; } = new Godot.Collections.Dictionary<string, string[]>();

    [Export]
    public Godot.Collections.Dictionary<string, int> BuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    [Export]
    public Dictionary<string, int> Upkeep { get; set; } = new Dictionary<string, int>();

    //public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

    public Node Stats { get; set; } = new Node();

    // World - initStartFleets, InitResistance
    public Unit(){  
    }

    // UI Planet Interface
    public Unit(int attack, int defence){
        BaseStat a = new BaseStat("Attack", attack);
        BaseStat d = new BaseStat("Defence", defence);
        BaseStat hp = new BaseStat("HitPoints", 200);
        Name = "Unit ";
        Stats.AddChild(a);
        Stats.AddChild(d);
        Stats.AddChild(hp);
    }

    public Unit(string name, List<BaseStat> stats){
        Name = name;
        foreach(BaseStat stat in stats){
            Stats.AddChild(stat);
        }
    }

    public Unit(Unit unit){
        Name = unit.Name;
        CopyStats(unit.Stats);
        BuildCost = new Godot.Collections.Dictionary<string, int>(unit.BuildCost);
        BuildTime = unit.BuildTime;
        ID_Owner = unit.ID_Owner;
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

    public BaseStat GetStat(string name){
        return GetNode<BaseStat>("Stats/"+name);
    }

}
