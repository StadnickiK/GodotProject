using Godot;
using System;
using System.Collections.Generic;

public class Unit : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    public int ID_Owner { get; set; }

    public bool HasHitpoints { get; set; } = true;  

    public Unit(){
        Random rand = new Random();
        BaseStat attack = new BaseStat("Attack", rand.Next(10,30));
        BaseStat defence = new BaseStat("Defence", rand.Next(0,10));
        BaseStat hp = new BaseStat("HitPoints", 100);
        Stats.Add(attack.StatName, attack);
        Stats.Add(defence.StatName, defence);
        Stats.Add(hp.StatName, hp);
    }

    public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

    public void CalculateDamage(Unit unit){
        unit.Stats["HitPoints"].CurrentValue -= Stats["Attack"].BaseValue - unit.Stats["Defence"].BaseValue;
        if(Stats["HitPoints"].CurrentValue<0){
            HasHitpoints = false;
        }
        if(unit.Stats["HitPoints"].CurrentValue<=0){
            unit.HasHitpoints = false;
        }else{
                Stats["HitPoints"].CurrentValue -= unit.Stats["Attack"].BaseValue - Stats["Defence"].BaseValue;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
