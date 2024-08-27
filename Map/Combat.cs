using Godot;
using System;
using System.Collections.Generic;

public partial class Combat : Node
{
    public List<PhysicsBody3D> Combatants { get; set; } = new List<PhysicsBody3D>();
    PackedScene _battleScene = (PackedScene)ResourceLoader.Load("res://Map/SpaceBattle.tscn");

    public SpaceBattle CreateBattle(PhysicsBody3D ship, PhysicsBody3D enemy, Node parent){
        
        var battle = (SpaceBattle)_battleScene.Instantiate();
        var trans = battle.Transform;
        trans.Origin =  ship.Transform.Origin;
        battle.Transform = trans;
        battle.AddCombatants(ship, enemy);
        HideNodes(ship, enemy);
        parent.AddChild(battle);
        return battle;
    }

    void HideNodes(params Node3D[] Nodes){
        foreach(Node3D spatial in Nodes){
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
