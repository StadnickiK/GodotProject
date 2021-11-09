using Godot;
using System;
using System.Collections.Generic;

public class Combat : Node
{
    public List<PhysicsBody> Combatants { get; set; } = new List<PhysicsBody>();
    PackedScene _battleScene = (PackedScene)ResourceLoader.Load("res://Map/SpaceBattle.tscn");

    public SpaceBattle CreateBattle(PhysicsBody ship, PhysicsBody enemy, Node parent){
        
        var battle = (SpaceBattle)_battleScene.Instance();
        var trans = battle.Transform;
        trans.origin =  ship.Transform.origin;
        battle.Transform = trans;
        battle.AddCombatants(ship, enemy);
        HideNodes(ship, enemy);
        parent.AddChild(battle);
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
