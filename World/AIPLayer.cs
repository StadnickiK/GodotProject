using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
                var map = (Dictionary<string, List<Planet>>)MapObject;
                if(planet.System != null){
                    if(!map.ContainsKey(planet.System.Name)){
                        List<Planet> list = new List<Planet>();
                        list.Add(planet);
                        map.Add(planet.System.Name, list);
                    }else{
                        var list = map[planet.System.Name];
                        if(!list.Contains(planet)){
                            list.Add(planet);
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
                if(planet.BuildingsManager.Constructions.ConstructionList.Count == 0)
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

    TreeNode.NodeState HasResources(){
        var reqBuildingsObj = GetBlackBoardObj("BuildingRequirements");
        var reqBuildings = (Dictionary<Building, int>)reqBuildingsObj;
        blackBoard["BuildingRequirements"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        if(reqBuildings.Count > 0){
            var BuildCost = reqBuildings.Keys.Last().BuildCost;
            var resReqObj = blackBoard["ResourceRequirements"]; // resReq - resource requirements
            var resReq = (Dictionary<string, int>)resReqObj;
            int count = resReq.Count;
            foreach(var resName in BuildCost.Keys){
                if(BuildCost[resName] > 0)
                    if(ResManager.Resources.ContainsKey(resName)){
                        if(ResManager.Resources[resName] < BuildCost[resName]){
                            if(!resReq.ContainsKey(resName)){
                                resReq.Add(resName, BuildCost[resName]);
                            }else{
                                resReq[resName] += (BuildCost[resName] - resReq[resName]);
                            }
                        }
                    }else{
                        if(!resReq.ContainsKey(resName)){
                            resReq.Add(resName, BuildCost[resName]);
                        }else{
                            resReq[resName] += (resReq[resName]);
                        }
                    }
            }
            
            if(count > resReq.Count)
                return TreeNode.NodeState.Failure;
        }
        return TreeNode.NodeState.Succes;
    }

    public bool HasResources(Godot.Collections.Dictionary<string, int> BuildCost){

        foreach(var resName in BuildCost.Keys){
            if(BuildCost[resName] > 0)
                if(ResManager.Resources.ContainsKey(resName)){
                    if(ResManager.Resources[resName] < BuildCost[resName]){
                        return false;
                    }
                }else{
                    return false;
            }
        }
        return true;
    }

    bool CheckBuildingResources(Planet planet, Building building){
		foreach(var resName in building.Products.Keys){
			if(!planet.ResourcesManager.Resources.ContainsKey(resName)){
				return false;
			}
		}
		return true;
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

    TreeNode.NodeState ChooseBuildToConstruct(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var planetObj = blackBoard["BuildConstructor"];
            var planet = (Planet)planetObj;
            if(_data.GetNode("Buildings").GetChildren().Count >0)
                if(planet.StartConstruction((Building)_data.GetData("Buildings")[0])){
                    return TreeNode.NodeState.Succes;
                }
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState HasBuilding(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var planetObj = blackBoard["BuildConstructor"];
            var planet = (Planet)planetObj;
            var reqBuildingsObj = GetBlackBoardObj("BuildingRequirements");
            var reqBuildings = (Dictionary<Building, int>)reqBuildingsObj;
            if(reqBuildings.Count > 0){
                blackBoard["BuildingRequirements"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
                for(int i = 0; i < reqBuildings.Keys.Count; i++){
                    if(planet.BuildingsManager.HasBuilding(reqBuildings.Keys.ElementAt(i))){
                        reqBuildings.Remove(reqBuildings.Keys.ElementAt(i));
                    }else{
                        return TreeNode.NodeState.Succes;
                    }
                }
            }
        }
        return TreeNode.NodeState.Failure;
        
    }
    
    TreeNode.NodeState ConstructBuilding(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var planetObj = blackBoard["BuildConstructor"];
            var planet = (Planet)planetObj;

            var reqBuildingsObj = GetBlackBoardObj("BuildingRequirements");
            var reqBuildings = (Dictionary<Building, int>)reqBuildingsObj;
            var targetBuilding = reqBuildings.Last().Key;
            if(planet.StartConstruction(targetBuilding)){
                reqBuildings.Remove(targetBuilding);
                return TreeNode.NodeState.Succes;
            }
        }
        return TreeNode.NodeState.Failure;
    }

    object GetBlackBoardObj(string name){
        if(blackBoard.ContainsKey(name))
            return blackBoard[name];
        return null;
    }

    TreeNode.NodeState GetReqResourceBuilding(){
        if(blackBoard.ContainsKey("ResourceRequirements")){
            var reqResObj = GetBlackBoardObj("ResourceRequirements");
            var reqRes = (Dictionary<string, int>)reqResObj;
            blackBoard["ResourceRequirements"] = reqRes = reqRes.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            var reqBuildObj = GetBlackBoardObj("BuildingRequirements");
            var reqBuildings = (Dictionary<Building, int>)reqBuildObj;
            if( reqRes.Count > 0){
                var resName = reqRes.Last().Key;
                var allBuildings = _data.GetNode("Buildings").GetChildren();
                var count = reqBuildings.Count;
                if(allBuildings.Count > 0){
                    foreach(Node node in allBuildings){
                        if(node is Building building){
                            if(
                                (building.Products.ContainsKey(resName) 
                                || building.ResourceLimits.ContainsKey(resName))
                                && !reqBuildings.ContainsKey(building))
                                    reqBuildings.Add(building, 1);
                        }   
                    }
                }
                if(reqBuildings.Count > count)
                    return TreeNode.NodeState.Succes;
            }
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState GetReqResourcePlanet(){
        var mapObj = blackBoard["Map"];
        var map = (Dictionary<string, List<Planet>>)mapObj; 
        var targetsObj = blackBoard["ColonyTargets"];
        var targets = (Dictionary<Planet, int>)targetsObj;
        var resReqObj = blackBoard["ResourceRequirements"];
        var reqRes = (Dictionary<string, int>)resReqObj;
        if(reqRes.Count > 0){ 
            blackBoard["ResourceRequirements"] = reqRes = reqRes.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach(string s in map.Keys){
                foreach(Planet planet in map[s]){
                    if(planet.ResourcesManager.Resources.ContainsKey(reqRes.Keys.Last())
                        && !targets.ContainsKey(planet)
                    ){
                        targets.Add(planet, 1);
                        return TreeNode.NodeState.Succes;
                    }
                }
            }
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode SetupTree(){
        TreeNode root = null;
        blackBoard.Add("Player", this);
        blackBoard.Add("ConstructUnitID", 0);
        blackBoard.Add("Map", new Dictionary<string, List<Planet>>{});
        blackBoard.Add("ResourceRequirements", new Dictionary<string, int>());
        blackBoard.Add("BuildingRequirements", new Dictionary<Building, int>(){{(Building)_data.GetData("Buildings")[2], 1}});
        blackBoard.Add("ColonyTargets", new Dictionary<Planet, int>());

        TreeNode scout = new Sequence(new List<TreeNode> {
            new GetIdleShip(blackBoard),
            new ScoutSystem(blackBoard)
        });

        TreeNode buildUnits = new Sequence(new List<TreeNode> {
            //new ActionTN(HasIdleUnitConstructionPlanet),
            //new ActionTN(ConstructUnit)
        });

        TreeNode buildBuild = new Sequence(new List<TreeNode> {
            new ActionTN(HasIdleBuildConstructionPlanet),
            new ActionTN(HasBuilding),
            new ActionTN(HasResources),
            new ActionTN(ConstructBuilding)
        });

        TreeNode getReq = new Parallel(new List<TreeNode>{
            new ActionTN(GetReqResourceBuilding),
            new ActionTN(GetReqResourcePlanet)
        });

        root = new Parallel(new List<TreeNode> {
            scout, getReq, buildBuild, buildUnits
        });

        return root;
    }

}
