using Godot;
using System;
using System.Collections.Generic;

public partial class Combat : Node
{

    [Export]
    string ScenePath = "res://Map/SpaceBattle.tscn";
    PackedScene _battleScene;

    public List<IEnterCombat> Combatants { get; set; }
    
    public SpaceBattle SpaceBattle { get; set; }

    public SpaceBattle CreateBattle(List<IEnterCombat> attackers, List<IEnterCombat> defenders, Node parent)
    {
        Combatants.AddRange(attackers);
        Combatants.AddRange(defenders);
        // var trans = SpaceBattle.Transform;
        // trans.Origin = ship.Transform.Origin;
        // SpaceBattle.Transform = trans;
        SpaceBattle.AddAttackers(attackers);
        SpaceBattle.AddDefenders(defenders);
        //HideNodes(ship, enemy);
        //parent.AddChild(SpaceBattle);
        return SpaceBattle;
    }

    public void ClearCombat()
    {
        SpaceBattle.AcceptResult();
        Combatants.Clear();
        SpaceBattle.Attackers.Clear();
        SpaceBattle.Defenders.Clear();
    }

    void HideNodes(params Node3D[] Nodes){
        foreach(Node3D spatial in Nodes){
            spatial.Visible = false;
        }
    }

    public override void _Ready()
    {
        _battleScene = (PackedScene)ResourceLoader.Load(ScenePath);
        SpaceBattle = GetNode<SpaceBattle>("SpaceBattle");
        Combatants = new List<IEnterCombat>();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
