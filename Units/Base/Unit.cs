using Godot;
using System;
using System.Collections.Generic;

public class Unit : Node, IBuilding
{

    [Export]
    public int ID_Owner { get; set; }

    public bool HasHitpoints { get; set; } = true;  

    public int BuildTime { get; set; } = 0;

    public int CurrentTime { get; set; } = 0;

    public Godot.Collections.Dictionary<string, int> BuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

    // World - initStartFleets, InitResistance
    public Unit(){  
        Random rand = new Random();
        BaseStat attack = new BaseStat("Attack", rand.Next(10,15));
        BaseStat defence = new BaseStat("Defence", rand.Next(0,5));
        BaseStat hp = new BaseStat("HitPoints", 200);
        Name = "Unit ";
        Stats.Add(attack.StatName, attack);
        Stats.Add(defence.StatName, defence);
        Stats.Add(hp.StatName, hp);
    }

    public Unit(int hitPoints){
        Random rand = new Random();
        BaseStat attack = new BaseStat("Attack", rand.Next(10,15));
        BaseStat defence = new BaseStat("Defence", rand.Next(0,5));
        BaseStat hp = new BaseStat("HitPoints", hitPoints);
        Name = "Unit ";
        Stats.Add(attack.StatName, attack);
        Stats.Add(defence.StatName, defence);
        Stats.Add(hp.StatName, hp);
    }

    // UI Planet Interface
    public Unit(int attack, int defence){
        BaseStat atck = new BaseStat("Attack", attack);
        BaseStat dfnce = new BaseStat("Defence", defence);
        BaseStat hp = new BaseStat("HitPoints", 200);
        Name = "Unit ";
        Stats.Add(atck.StatName, atck);
        Stats.Add(dfnce.StatName, dfnce);
        Stats.Add(hp.StatName, hp);
    }

    public Unit(string name, List<BaseStat> stats){
        Name = name;
        foreach(BaseStat stat in stats){
            Stats.Add(stat.Name, stat);
        }
    }

    public Unit(Unit unit){
        Name = unit.Name;
        Stats = new Dictionary<string, BaseStat>(unit.Stats);
        BuildCost = new Godot.Collections.Dictionary<string, int>(unit.BuildCost);
        BuildTime = unit.BuildTime;
        ID_Owner = unit.ID_Owner;
    }

    public void CalculateDamage(Unit unit){
        if(Stats["Attack"].BaseValue > unit.Stats["Defence"].BaseValue){
            unit.Stats["HitPoints"].CurrentValue -= Stats["Attack"].BaseValue - unit.Stats["Defence"].BaseValue;
        }else{
            unit.Stats["HitPoints"].CurrentValue--; // if defence is higher than attack deal minimal dmg
        }
        if(Stats["HitPoints"].CurrentValue<0){
            HasHitpoints = false;
        }
        if(unit.Stats["HitPoints"].CurrentValue<=0){
            unit.HasHitpoints = false;
        }else{
            if(unit.Stats["Attack"].BaseValue > Stats["Defence"].BaseValue){
                Stats["HitPoints"].CurrentValue -= unit.Stats["Attack"].BaseValue - Stats["Defence"].BaseValue;
            }else{
                Stats["HitPoints"].CurrentValue--; // if defence is higher than attack deal minimal dmg
            }
            // Stats["HitPoints"].CurrentValue -= unit.Stats["Attack"].BaseValue - Stats["Defence"].BaseValue;
        }
    }

    public override void _Ready()
    {

    }

}
