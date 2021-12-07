using Godot;
using System;
using System.Collections.Generic;

public class AIPlayer : Player
{

    Data _data = null;

    TreeNode root = null;

    public AIPlayer() : base(){}

    public AIPlayer(Data data) : base(){
        _data = data;
    }

    Dictionary<string, object> blackBoard = new Dictionary<string, object>();

    void _on_MapObject_Entered(Node node){
        if(node is Planet planet){
            if(blackBoard.ContainsKey("Map")){
                var MapObject = blackBoard["Map"];
                var map = (Dictionary<string, List<int>>)MapObject;
                if(planet.System != null){
                    if(!map.ContainsKey(planet.System.Name)){
                        List<int> list = new List<int>();
                        list.Add(planet.GetIndex());
                        map.Add(planet.System.Name, list);
                    }else{
                        var listObj = map[planet.System.Name];
                        var list = (List<int>)listObj;
                        if(!list.Contains(planet.GetIndex())){
                            list.Add(planet.GetIndex());
                        }else{

                        }
                    }
                }
            }
        }
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

    public Planet GetIdleShipConstructionPlanet(){
        foreach(Node node in MapObjects){
            if(node is Planet planet)
                if(planet.CurrentUnit == null)
                    return planet;
        }
        return null;
    }

    public Planet GetIdleBuildConstructionPlanet(){
        foreach(Node node in MapObjects){
            if(node is Planet planet)
                if(planet.BuildingsManager.Constructions.ConstructionList.Count != 0)
                    return planet;
        }
        return null;
    }

    public TreeNode.NodeState GetShip(){
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState HasIdleUnitConstructionPlanet(){

        var planet = GetIdleShipConstructionPlanet();
        if(planet != null){
            if(!blackBoard.ContainsKey("UnitConstructor")){
                blackBoard.Add("UnitConstructor", planet);
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState HasIdleBuildConstructionPlanet(){
        var planet = GetIdleBuildConstructionPlanet();
        if(planet != null){
            if(!blackBoard.ContainsKey("BuildConstructor")){
                blackBoard.Add("BuildConstructor", planet);
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState ConstructUnit(){
        if(blackBoard.ContainsKey("UnitConstructor")){
            var planetObj = blackBoard["UnitConstructor"];
            var planet = (Planet)planetObj;
            if(_data.WorldUnits.Count >0)
                if(planet.StartConstruction(_data.WorldUnits[0])){
                    return TreeNode.NodeState.Succes;
                }
        }
        return TreeNode.NodeState.Failure;
    }
    
    TreeNode.NodeState ConstructBuilding(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var planetObj = blackBoard["BuildConstructor"];
            var planet = (Planet)planetObj;
            if(_data.WorldUnits.Count >0)
                if(planet.StartConstruction(null)){
                    return TreeNode.NodeState.Succes;
                }
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode SetupTree(){
        TreeNode root = null;
        blackBoard.Add("Player", this);
        blackBoard.Add("ConstructUnitID", 0);
        blackBoard.Add("Map", new Dictionary<string, List<int>>());
        TreeNode scout = new Sequence(new List<TreeNode> {
            new GetIdleShip(blackBoard),
            new ScoutSystem(blackBoard)
        });

        TreeNode buildUnits = new Sequence(new List<TreeNode> {
            new ActionTN(HasIdleUnitConstructionPlanet),
            new ActionTN(ConstructUnit)
        });

        TreeNode buildBuild = new Sequence(new List<TreeNode> {
            new ActionTN(HasIdleBuildConstructionPlanet)
        });

        root = new Parallel(new List<TreeNode> {
            scout, buildUnits
        });

        return root;
    }

}
