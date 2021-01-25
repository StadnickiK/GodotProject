using Godot;
using System;
using System.Collections.Generic;

public class Combat : Node
{
    public List<PhysicsBody> Combatants { get; set; } = new List<PhysicsBody>();
    // Called when the node enters the scene tree for the first time.

    public List<SpaceBattle> SpaceBattles {get; set; } = new List<SpaceBattle>();

    PackedScene _battleScene = (PackedScene)ResourceLoader.Load("res://Map/SpaceBattle.tscn");

    public SpaceBattle CreateBattle(PhysicsBody ship, PhysicsBody enemy, Node parent){
        var battle = (SpaceBattle)_battleScene.Instance();
        var trans = battle.Transform;
        trans.origin =  ship.Transform.origin;
        battle.Transform = trans;
        battle.AddCombatants(ship, enemy);
        //HideNodes(ship, enemy);
        if(parent.Name =="StarSysObjects"){
            parent.AddChild(battle);
        }else{
            parent.AddChild(battle);
        }
        return battle;
    }

    void HideNodes(params Spatial[] Nodes){
        foreach(Spatial spatial in Nodes){
            spatial.Visible = false;
        }
    }

    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
