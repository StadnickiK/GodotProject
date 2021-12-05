using Godot;
using System;
using System.Collections.Generic;

public class AIPlayer : Player
{

    TreeNode root = null;

    Dictionary<string, object> blackBoard = new Dictionary<string, object>();

    void _on_MapObject_Entered(Node node){
        
    }

    void _on_MapObject_Exited(Node node){
        
    }

    public void ConnectSignals(){
        foreach(Node node in MapObjects){
            if(node is Ship ship){
                ship._area.Connect("body_entered", this, nameof(_on_MapObject_Entered));
                ship._area.Connect("body_exited", this, nameof(_on_MapObject_Exited));
            }
        }
    }

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
        blackBoard.Add("Map", new Dictionary<string, Planet>());
        root = new Sequence(new List<TreeNode> {
            new GetIdleShip(blackBoard),
            new ScoutSystem(blackBoard)
        });

        return root;
    }

}
