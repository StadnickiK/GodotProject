using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AIPlayer : Player
{

    Data _data = null;

    TreeNode root = null;

    [Export]
    public float FleetStrengthMul { get; set; } = 1.1f;

    public AIPlayer() : base(){}

    public AIPlayer(Data data) : base(){
        _data = data;
    }

    Dictionary<string, object> blackBoard = new Dictionary<string, object>();

    void _on_MapObject_Entered(Node node, Ship ship){
        if(node is Planet planet){
            if(ship.MapObject == planet.GetParent().GetParent())
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
                if(!ship._area.IsConnected("body_entered", this, nameof(_on_MapObject_Entered))){
                    ship._area.Connect("body_entered", this, nameof(_on_MapObject_Entered), new Godot.Collections.Array(){ship});
                }
                if(!ship._area.IsConnected("body_exited", this, nameof(_on_MapObject_Exited)))
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
        if(MapObjectsChanged){
            ConnectSignals();
        }
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
                if(!planet.Constructions.HasConstruct())
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

    public List<Planet> GetIdleBuildConstructionPlanets(){
        var list = new List<Planet>();
        foreach(Node node in MapObjects){
            if(node is Planet planet)
                if(planet.BuildingsManager.Constructions.ConstructionList.Count == 0)
                    list.Add(planet);
        }
        return list;
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

    TreeNode.NodeState HasIdleBuildConstructionPlanets(){
        var planets = GetIdleBuildConstructionPlanets();
        if(planets != null){
            if(!blackBoard.ContainsKey("BuildConstructor")){
                blackBoard.Add("BuildConstructor", planets);
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
    
    bool CheckBuildingResources(Planet planet, Building building){
		foreach(var resName in building.Products.Keys){
			if(!planet.ResourcesManager.Resources.ContainsKey(resName)){
				return false;
			}
		}
		return true;
	}

    // todo: remove leak when a ship has a number of tempPoints and they get canceled, to reproduce add a number of tempTargets and then add a new command

    TreeNode.NodeState CreateFleetRequest(){
        var targetsObj = blackBoard["ColonyTargets"];
        var targets = (Dictionary<Planet, int>)targetsObj;
        if(targets.Count() > 0){
            var unitsObj = blackBoard["UnitsToRecruit"];
            var units = (Dictionary<Unit, int>)unitsObj;
            blackBoard["ColonyTargets"] = targets = targets.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            var enemies = targets.Keys.First().Orbit.GetChildren();
            int count = 0;
            foreach(Node node in enemies){
                if(node is Ship ship)
                    count += ship.Units.GetChildren().Count;
            }
            if(count > 0){
                count = (int)Math.Ceiling((float)count*FleetStrengthMul);
                units.Add((Unit)((PackedScene)GD.Load(((Unit)_data.GetData("Units")[0]).Filename)).Instance(), count);
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState ConstructFleet(){
        if(blackBoard.ContainsKey("UnitConstructor")){
            var planetObj = blackBoard["UnitConstructor"];
            var planet = (Planet)planetObj;
            var unitsObj = blackBoard["UnitsToRecruit"];
            var units = (Dictionary<Unit, int>)unitsObj;
            var constObj = blackBoard["FleetConstructionPlanet"];
            var constructor = (Dictionary<string, int>)constObj;
            if(units.Count >0){
                var count = planet.StartConstruction(units.Keys.ElementAt(0), units.Values.ElementAt(0));
                if(!constructor.ContainsKey(planet.Name))
                    constructor.Add(planet.Name, units.Values.ElementAt(0));
                if(count == units.Values.ElementAt(0)){
                    return TreeNode.NodeState.Succes;
                }
                units[units.Keys.ElementAt(0)] -= count; 
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

    //todo: redo with planet building dictionary

    TreeNode.NodeState HasBuilding(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var reqBuildingsObj = GetBlackBoardObj("BuildingRequirements");
            var reqBuildings = (Dictionary<Building, int>)reqBuildingsObj;
            if(reqBuildings.Count > 0){
                var planetObj = blackBoard["BuildConstructor"];
                var planet = (List<Planet>)planetObj;
                if(planet.Count > 0){
                    blackBoard["BuildingRequirements"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
                    for(int i = 0; i < reqBuildings.Count; i++){
                        var building = reqBuildings.Keys.ElementAt(i);
                        if(planet[0].BuildingsManager.HasBuilding(building)){
                            reqBuildings.Remove(building);
                        }else{
                            return TreeNode.NodeState.Succes;
                        }
                    }
                }
            }
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState PlanInvasion(){
        var targetsObj = blackBoard["ColonyTargets"];
        var targets = (Dictionary<Planet, int>)targetsObj;
        var fleetsObject = blackBoard["IdleFleets"];
        var fleets = (Dictionary<Ship, int>)fleetsObject;
        if(targets.Count() > 0 && fleets.Count > 0){
            var invasionsObject = blackBoard["InvasionsInProgress"];
            var invasionsInProgress = (Dictionary<Ship, Spatial>)invasionsObject;
            blackBoard["ColonyTargets"] = targets = targets.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            var enemies = targets.Keys.First().Orbit.GetChildren();
            int count = 0;
            foreach(Node node in enemies){
                if(node is Ship ship)
                    count += ship.Units.GetChildren().Count;
            }
            invasionsObject = blackBoard["InvasionPlans"];
            var invasions = (Dictionary<Ship, Planet>)invasionsObject;
            for(int i = 0; i < fleets.Count(); i++){
                var ship = fleets.Keys.ElementAt(i);
                if(ship != null && targets.ElementAt(0).Key != null)
                    if(ship.Units.GetChildren().Count > targets.ElementAt(0).Key.Orbit.GetChildren().Count){    
                        if(!invasions.ContainsKey(ship)){
                            invasions.Add(ship, targets.Keys.ElementAt(0));
                            targets.Remove(targets.Keys.ElementAt(0));
                            fleets.Remove(ship);
                            return TreeNode.NodeState.Succes;
                        }
                    }
            }
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState ExecuteInvasionPlan(){
        var plansObject = blackBoard["InvasionPlans"];
        var plans = (Dictionary<Ship, Planet>)plansObject;
        if(plans.Count > 0){
            var invasionsObject = blackBoard["InvasionsInProgress"];
            var invasions = (Dictionary<Ship, Spatial>)invasionsObject;
            for(int i = 0; i < plans.Count; i++){
                var plan = plans.ElementAt(i);
                if(!invasions.ContainsKey(plan.Key)){
                    plan.Key.targetManager.AddTarget(plan.Value);
                    plan.Key.Task = CmdPanel.CmdPanelOption.Conquer;
                    plan.Key.MoveToTarget(plan.Value);
                    plans.Remove(plan.Key);
                    invasions.Add(plan.Key, plan.Value);
                }
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }
    

    // todo: change the way buildings are passed to Planet, add a filter or smth
    TreeNode.NodeState ConstructBuilding(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var planetObj = blackBoard["BuildConstructor"];
            var planet = (List<Planet>)planetObj;
            if(planet.Count > 0){
                var reqBuildingsObj = GetBlackBoardObj("BuildingRequirements");
                var reqBuildings = ((Dictionary<Building, int>)reqBuildingsObj);//.Reverse().ToDictionary(x => x.Key, x => x.Value);
                var targetBuilding = reqBuildings.First().Key;
                if(targetBuilding.Products.Count > 0)
                    for(int i = 0; i < reqBuildings.Count(); i ++){ // foreach throws because indexer changes
                        var building = reqBuildings.Keys.ElementAt(i);
                            foreach(string resName in building.Products.Keys){
                                if(!planet[0].ResourcesManager.Resources.ContainsKey(resName) || planet[0].BuildingsManager.HasBuilding(building)){
                                    var temp = reqBuildings[building];
                                    reqBuildings.Remove(targetBuilding); // remove fist element
                                    reqBuildings.Append(new KeyValuePair<Building, int>(targetBuilding, temp));
                                    targetBuilding = reqBuildings.FirstOrDefault().Key; // set new target building
                                    break;
                                }
                            }
                    }
                if(!planet[0].BuildingsManager.HasBuilding(targetBuilding)) // needee if the dictionary contains only 1 item
                    if(planet[0].StartConstruction(targetBuilding)){
                        reqBuildings.Remove(targetBuilding);
                        return TreeNode.NodeState.Succes;
                    }
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
                            if((building.Products.ContainsKey(resName) 
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
        var invasionsObject = blackBoard["InvasionsInProgress"];
        var invasions = (Dictionary<Ship, Spatial>)invasionsObject;
        if(reqRes.Count > 0){ 
            blackBoard["ResourceRequirements"] = reqRes = reqRes.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach(string systemName in map.Keys){
                foreach(Planet planet in map[systemName]){
                    if(!MapObjects.Contains(planet) && !invasions.ContainsValue(planet))
                        if(planet.ResourcesManager.Resources.ContainsKey(reqRes.Keys.Last())
                            && !targets.ContainsKey(planet)
                        ){
                            targets.Add(planet, planet.ResourcesManager.Resources.Count());
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
        blackBoard.Add("BuildingPlan", new Dictionary<Planet, Building>());
        blackBoard.Add("ColonyTargets", new Dictionary<Planet, int>());
        blackBoard.Add("UnitsToRecruit", new Dictionary<Unit, int>()); // unit and amount to recruit
        blackBoard.Add("UnitsRecruitPrio", new Dictionary<string, int>()); // prioriutu for unit construction
        blackBoard.Add("FleetConstructionPlanet", new Dictionary<string, int>()); // planet name and unit quantity
        blackBoard.Add("IdleFleets", new Dictionary<Ship, int>()); // ship and its unit count
        blackBoard.Add("InvasionPlans", new Dictionary<Ship, Planet>()); // ship and its target
        blackBoard.Add("InvasionsInProgress", new Dictionary<Ship, Spatial>()); // ship and its target
        blackBoard.Add("ScoutMissions", new Dictionary<Ship, string>()); // ship and its target

        TreeNode scout = new Sequence(new List<TreeNode> {
            new GetIdleShip(blackBoard),
            new ScoutSystem(blackBoard)
        });


        // todo: Add has resources, add recruitment queue based on target planet Reapeter?? 
        TreeNode buildUnits = new Sequence(new List<TreeNode> {
            new ActionTN(HasIdleUnitConstructionPlanet),
            new ActionTN(CreateFleetRequest),
            new ActionTN(ConstructFleet)
        });

        TreeNode buildBuild = new Sequence(new List<TreeNode> {
            new ActionTN(HasIdleBuildConstructionPlanets),
            new ActionTN(HasBuilding),
            new ActionTN(HasResources),
            new ActionTN(ConstructBuilding)
        });

        TreeNode getReq = new Parallel(new List<TreeNode>{
            new ActionTN(GetReqResourceBuilding),
            new ActionTN(GetReqResourcePlanet)
        });

        TreeNode executeInvasion = new Sequence(new List<TreeNode>{
            new ActionTN(PlanInvasion),
            new ActionTN(ExecuteInvasionPlan)
        });

        root = new Parallel(new List<TreeNode> {
            scout, getReq, buildBuild, buildUnits, executeInvasion
        });

        return root;
    }

}
