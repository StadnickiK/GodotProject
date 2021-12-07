using Godot;
using System;
using System.Collections.Generic;

public class Unit : Node, IBuilding
{

    [Export]
    public int ID_Owner { get; set; }

    public bool HasHitpoints { get; set; } = true;  

    [Export]
    public int BuildTime { get; set; } = 0;

    public int CurrentTime { get; set; } = 0;

    [Export]
    public Godot.Collections.Array<string[]> Requirements { get; set; } = new Godot.Collections.Array<string[]>();

    [Export]
    public Godot.Collections.Dictionary<string, int> BuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    //public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

    public Node Stats { get; set; } = new Node();

    // World - initStartFleets, InitResistance
    public Unit(){  
        Random rand = new Random();
        BaseStat attack = new BaseStat("Attack", rand.Next(10,15));
        BaseStat defence = new BaseStat("Defence", rand.Next(0,5));
        BaseStat hp = new BaseStat("HitPoints", 200);
        Name = "Unit ";
        Stats.AddChild(attack);
        Stats.AddChild(defence);
        Stats.AddChild(hp);
    }

    public Unit(int hitPoints){
        Random rand = new Random();
        BaseStat attack = new BaseStat("Attack", rand.Next(10,15));
        BaseStat defence = new BaseStat("Defence", rand.Next(0,5));
        BaseStat hp = new BaseStat("HitPoints", hitPoints);
        Name = "Unit ";
        Stats.AddChild(attack);
        Stats.AddChild(defence);
        Stats.AddChild(hp);
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
        if(Stats.GetNode<BaseStat>("Attack").BaseValue > unit.Stats.GetNode<BaseStat>("Defence").BaseValue){
            unit.Stats.GetNode<BaseStat>("HitPoints").CurrentValue -= Stats.GetNode<BaseStat>("Attack").BaseValue - unit.Stats.GetNode<BaseStat>("Defence").BaseValue;
        }else{
            unit.Stats.GetNode<BaseStat>("HitPoints").CurrentValue--; // if defence is higher than attack deal minimal dmg
        }
        if(Stats.GetNode<BaseStat>("HitPoints").CurrentValue<0){
            HasHitpoints = false;
        }
        if(unit.Stats.GetNode<BaseStat>("HitPoints").CurrentValue<=0){
            unit.HasHitpoints = false;
        }else{
            if(unit.Stats.GetNode<BaseStat>("Attack").BaseValue > Stats.GetNode<BaseStat>("Defence").BaseValue){
                Stats.GetNode<BaseStat>("HitPoints").CurrentValue -= unit.Stats.GetNode<BaseStat>("Attack").BaseValue - Stats.GetNode<BaseStat>("Defence").BaseValue;
            }else{
                Stats.GetNode<BaseStat>("HitPoints").CurrentValue--; // if defence is higher than attack deal minimal dmg
            }
            // Stats.GetNode<BaseStat>("HitPoints").CurrentValue -= unit.Stats.GetNode<BaseStat>("Attack").BaseValue - Stats.GetNode<BaseStat>("Defence").BaseValue;
        }
    }

    public override void _Ready()
    {

    }

}
