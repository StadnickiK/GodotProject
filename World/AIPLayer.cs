using Godot;
using System;
using System.Collections.Generic;

public class AIPlayer : Player
{

    TreeNode root = null;

    Dictionary<string, object> blackBoard = new Dictionary<string, object>();

    public override void _Ready()
    {
        root = SetupTree();
        AddChild(root);
    }

    public override void _Process(float delta){
        base._Process(delta);
        if(root != null)
            root.Evaluate();
    }

    public Ship GetIdleShip(){

        foreach(Node node in MapObjects){
            if(node is Ship ship)
                if(ship.Sleeping == true)
                    return ship;
        }
        return null;
    }

    public Planet GetPlanet(){
        foreach(Node node in MapObjects){
            if(node is Planet planet)
                return planet;
        }
        return null;
    }

    public TreeNode.NodeState GetShip(){
        return TreeNode.NodeState.Failure;
    }

    TreeNode SetupTree(){
        TreeNode root = null;
        blackBoard.Add("Player", this);
        root = new Sequence(new List<TreeNode> {
            new GetIdleShip(blackBoard),
            new ScoutSystem(blackBoard)
        });

        return root;
    }

}
