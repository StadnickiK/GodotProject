using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AIPlayer : Player
{

    Data _data = null;

    Map _worldMap;

    TreeNode root = null;

    [Export]
    public float FleetStrengthMul { get; set; } = 1.1f;

    [Export]
    public float BaseHighResourceQuantity { get; set; } = 70;

    [Export]
    public float BaseLowResourceQuantity { get; set; } = 30; 

    [Export]
    public float BaseCriticalResourceQuantity { get; set; } = 10;    

    [Export]
    public float PlanetDevelopmentLvl { get; set; } = 0.6f;

    [Export]
    public int AICount { get; set; } = 50;

    [Export]
    public int MineWeight { get; set; } = 3;

    public float AvgPlanetDevelopmentLvl { get; set; } = 0f;

    public Dictionary<string, int> HighResourceQuantities { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> LowResourceQuantity { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> CriticalResourceQuantity { get; set; } = new Dictionary<string, int>();

    [Export]
    public Dictionary<Building.Category, float> BuildingPriorityList { get; set; } = new Dictionary<Building.Category, float>(){
        {Building.Category.Mine, 7},
        {Building.Category.Production, 6},
        {Building.Category.Growth, 5},
        {Building.Category.Storage, 4},
        {Building.Category.Research, 3},
        {Building.Category.Construction, 2},
        {Building.Category.Recruitment, 1}
    };

    public AIPlayer() : base(){}

    public AIPlayer(Data data) : base(){
        _data = data;
    }

    Dictionary<string, object> blackBoard = new Dictionary<string, object>();

    public void SetMap(Map map){
        _worldMap = map;
        blackBoard.Add("WorldMap", map);
    }

    public override void _Ready()
    {
        // root = SetupTree();
        // AddChild(root);
    }

    float _time = 0;

    int counter = 0;

    public override void _Process(float delta){
        base._Process(delta);
        if(root != null)
            if(PlayerID == counter){
                root.Evaluate();
                if(counter >= 50)
                    counter = -1;
                _time = 0;
            }else{
                _time += delta;
            }
            if(counter >= 50)
                counter = -1;
            counter++;
        if(MapObjectsChanged){
            ConnectSignals();
        }
    }


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

    public Ship GetIdleShip(){
        var scoutMissions = (Dictionary<Ship, string>)blackBoard["ScoutMissions"];
        var invasionsObject = blackBoard["InvasionsInProgress"];
        var invasions = (Dictionary<Ship, Spatial>)invasionsObject;
        var fleets = (Dictionary<Ship, int>)blackBoard["IdleFleets"];
        foreach(Node node in MapObjects){
            if(node is Ship ship)
                if(!fleets.ContainsKey(ship) && (ship.Sleeping == true || (!scoutMissions.ContainsKey(ship) && !invasions.ContainsKey(ship))))
                    return ship;
        }
        return null;
    }

    public TreeNode.NodeState ClearFinishedScoutMissions(){
        var scoutMissions = (Dictionary<Ship, string>)blackBoard["ScoutMissions"];
        var map = (Dictionary<string, List<Planet>>)blackBoard["Map"];
        if(scoutMissions.Count > 0){
            for(int i = 0; i < scoutMissions.Keys.Count; i++){
                var scout = scoutMissions.ElementAt(i).Key;
                var systemName = scoutMissions.ElementAt(i).Value;
                if(map.ContainsKey(systemName) && IsInstanceValid(scout)){
                    if(map[systemName].Count > 0){
                        var planet = map[systemName].ElementAt(0);
                        var sysPlanets = planet.System.Planets;
                        foreach(var p in sysPlanets){
                            if(p.Controller == this && !map[systemName].Contains(p))
                                map[systemName].Add(p);
                        }
                    }
                    var count = (_worldMap.galaxy.StarSystems.FirstOrDefault( x => x.Name == systemName)).Planets.Count;
                    if(map[systemName].Count == count || scout.Sleeping == true){
                        scoutMissions.Remove(scout);
                    }

                }
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    public TreeNode.NodeState ClearFinishedInvasionMissions(){
        var invasions = (Dictionary<Ship, Spatial>)blackBoard["InvasionsInProgress"];
        var invPlans = (Dictionary<Ship, Planet>)blackBoard["InvasionPlans"];
        if(invasions.Count > 0){
            for(int i = 0; i < invasions.Count; i++){
                var pair = invasions.ElementAt(i);
                if(pair.Value != null){
                    if(pair.Value is IMapObjectController controller){
                        if(controller.Controller == this)
                            invasions.Remove(pair.Key);
                    }
                }else{
                    invasions.Remove(pair.Key);
                }
                if(pair.Key == null)
                    invasions.Remove(pair.Key);
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    public Planet GetFirstPlanet(){
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
            if(blackBoard.ContainsKey("BuildConstructor")){
                var list = (List<Planet>)blackBoard["BuildConstructor"];
                foreach(var planet in planets){
                    if(!list.Contains(planet))
                        list.Add(planet);
                }
            }
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState HasResources(){
        var reqBuildings = (Dictionary<Building, int>)GetBlackBoardObj("BuildingsToBuild");
        if(reqBuildings.Count <= 0) return TreeNode.NodeState.Failure;
        blackBoard["BuildingsToBuild"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            var BuildCost = reqBuildings.Keys.Last().BuildCost;
            var resReq = (Dictionary<string, int>)blackBoard["ResourceRequirements"]; // resReq - resource requirements
            
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
            
            
        return TreeNode.NodeState.Succes;
    }

    TreeNode.NodeState GetBuildingResourceRequirements(){
        var reqBuildings = (Dictionary<Building, int>)GetBlackBoardObj("BuildingsToBuild");
        if(reqBuildings.Count <= 0) return TreeNode.NodeState.Failure;
        //blackBoard["BuildingsToBuild"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            //var BuildCost = reqBuildings.Keys.Last().BuildCost;
            var resReq = (Dictionary<string, int>)blackBoard["ResourceRequirements"]; // resReq - resource requirements
            resReq.Clear();
            foreach(Building building in reqBuildings.Keys){
                for(int i = 0; i < building.BuildCost.Count; i++){
                    var resName = building.BuildCost.Keys.ElementAt(i);
                    if(building.BuildCost[resName] > 0)
                        if(!resReq.ContainsKey(resName)){
                            resReq.Add(resName, building.BuildCost[resName]);
                        }else{
                            resReq[resName] += (building.BuildCost[resName]);
                        }
                    if(i+1 == building.BuildCost.Count)
                        if(ResManager.Resources.ContainsKey(resName))
                            // if(resReq[resName] - ResManager.Resources[resName] > 0){
                                resReq[resName] -= ResManager.Resources[resName];   
                            // }else{
                            //     resReq[resName] = 1;
                            // }       
                }
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

    public StarSystem GetClosestStarSystem(StarSystem system){
        var children = _worldMap.galaxy.GetChildren();
        var map = (Dictionary<string, List<Planet>>)blackBoard["Map"];
        var scoutMissions = (Dictionary<Ship, string>)blackBoard["ScoutMissions"];
        StarSystem target = null;
        foreach(Node node in children){
            if(node is StarSystem starSystem){
                if(!map.Keys.Contains(starSystem.Name) && !scoutMissions.Values.Contains(starSystem.Name))
                    if(target == null){
                        target = starSystem;
                    }else{
                        if(system.Transform.origin.DistanceTo(target.Transform.origin) > system.Transform.origin.DistanceTo(starSystem.Transform.origin))
                            target = starSystem;
                    }
            }
        }
        return target;
    }

    // public IMapObject NextScoutTarget(){
    //     var starSystems = _worldMap.galaxy.GetChildren();
    //     var map = (Dictionary<string, List<Planet>>)blackBoard["Map"];
    //     var scoutMissions = (Dictionary<Ship, string>)blackBoard["ScoutMissions"];
    //     if(){

    //     }else{

    //     }

    //     return null;
    // }

    // todo: remove leak when a ship has a number of tempPoints and they get canceled, to reproduce add a number of tempTargets and then add a new command

    TreeNode.NodeState CreateFleetRequest(){
        var targetsObj = blackBoard["ColonyTargets"];
        var targets = (Dictionary<Planet, int>)targetsObj;
        if(targets.Count() > 0 && ResManager.Resources.ContainsKey("Resource 0")){
            if(ResManager.Resources["Resource 0"] < 300) return TreeNode.NodeState.Failure; // Recruit units onlu on good resource number or high planet development
            var unitsObj = blackBoard["UnitsToRecruit"];
            var units = (Dictionary<string, int>)unitsObj;
            blackBoard["ColonyTargets"] = targets = targets.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            var enemies = targets.Keys.First().Orbit.GetChildren();
            int count = 0;
            foreach(Node node in enemies){
                if(node is Ship ship)
                    count += ship.Units.GetChildren().Count;
            }
            if(count > 0){
                count = (int)Math.Ceiling((float)count*FleetStrengthMul);
                var key = ((Node)_data.GetData("Units")[0]).Name;
                if(!units.ContainsKey(key)){
                    units.Add(key, count);
                }
                //units.Add((Unit)_data.GetData("Units")[0], count);
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
            var units = (Dictionary<string, int>)unitsObj;
            var constObj = blackBoard["FleetConstructionPlanet"];
            var constructor = (Dictionary<string, int>)constObj;
            if(units.Count >0){
                // var count = planet.StartConstruction(units.Keys.ElementAt(0), units.Values.ElementAt(0));
                int count = 0;
                for (int i = 0; i < units.Values.ElementAt(0); i++)
                {
                    if(planet.StartConstruction((Unit)((PackedScene)GD.Load((_data.GetData("Units", units.Keys.ElementAt(0))).Filename)).Instance(), 1) == 0){
                        count = i;
                        break;
                    }
                }


                if(!constructor.ContainsKey(planet.Name))
                    constructor.Add(planet.Name, units.Values.ElementAt(0));
                // if(count == units.Values.ElementAt(0)){
                //     units.Remove(units.Keys.ElementAt(0));
                //     return TreeNode.NodeState.Succes;
                // }
                units[units.Keys.ElementAt(0)] -= count;
                if(units.Values.ElementAt(0) <= 0){
                    units.Remove(units.Keys.ElementAt(0));
                    return TreeNode.NodeState.Succes;
                } 
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
            var reqBuildingsObj = GetBlackBoardObj("BuildingsToBuild");
            var reqBuildings = (Dictionary<Building, int>)reqBuildingsObj;
            if(reqBuildings.Count > 0){
                var planets = (List<Planet>)blackBoard["BuildConstructor"];
                if(planets.Count > 0){
                    blackBoard["BuildingsToBuild"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
                    for(int i = 0; i < reqBuildings.Count; i++){
                        var building = reqBuildings.Keys.ElementAt(i);
                        if(planets[0].BuildingsManager.HasBuilding(building)){
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
            if(AvgPlanetDevelopmentLvl >= PlanetDevelopmentLvl){
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
                    if(!invasionsInProgress.ContainsKey(ship))
                        if(ship.Units.GetChildren().Count > count){    
                            if(!invasions.ContainsKey(ship)){
                                invasions.Add(ship, targets.Keys.ElementAt(0));
                                targets.Remove(targets.Keys.ElementAt(0));
                                fleets.Remove(ship);
                                return TreeNode.NodeState.Succes;
                            }
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

    TreeNode.NodeState GetBuildingRequirements(){
        var buildings = (Dictionary<Building, int>)blackBoard["BuildingsToBuild"];
        var buildingReq = (Dictionary<string, Dictionary<string, List<string>>>)blackBoard["BuildingRequirements"];
        bool newEntry = false;
        //var count = 0;
        foreach(var building in buildings.Keys){
            foreach(var reqName in building.Requirements.Keys){
                if(!buildingReq.ContainsKey(building.Name)){
                    buildingReq.Add(building.Name, new Dictionary<string, List<string>>(){{reqName, new List<string>(building.Requirements[reqName]) }});
                    //count++;
                    newEntry = true;
                }else{
                    if(newEntry){
                        buildingReq[building.Name].Add(reqName, new List<string>(building.Requirements[reqName]));
                    }else{
                        break; // break req loop if buildingis already on the list
                    }
                }
            }
            newEntry = false;
        }
        return TreeNode.NodeState.Succes;
    }

    TreeNode.NodeState HasBuildingRequirements(){
        var buildings = (Dictionary<Building, int>)blackBoard["BuildingsToBuild"];
        var buildingReq = (Dictionary<string, Dictionary<string, List<string>>>)blackBoard["BuildingRequirements"];
        var technologies = (Dictionary<Technology, int>)blackBoard["TechnologyToBuild"];

        //var count = 0;
        foreach(var building in buildings.Keys){
            for(int j = 0; j < building.Requirements.Count; j++){
                var reqName = building.Requirements.Keys.ElementAt(j);
                if(reqName == "Technology"){
                    for(int i = 0; i < building.Requirements[reqName].Count; i++){
                        var techName = building.Requirements[reqName][i];
                        if(Technologies.FirstOrDefault(x => x.Name == techName) != null){
                            if(buildingReq.ContainsKey(building.Name)){
                                if(buildingReq[building.Name][reqName].Contains(techName))
                                    buildingReq[building.Name][reqName].Remove(techName);
                            }else{
                                buildingReq.Add(building.Name, new Dictionary<string, List<string>>(){ { reqName, new List<string>(building.Requirements[reqName]) } });
                                buildingReq[building.Name][reqName].Remove(techName);
                            }
                        }else{
                            var tech = (Technology)_data.GetData("Technology", techName);
                            if(!technologies.ContainsKey(tech))
                                technologies.Add(tech, buildings[building]+1);
                        }
                    }
                }
                if(buildingReq.ContainsKey(building.Name)){
                        if(buildingReq[building.Name].ContainsKey(reqName))
                            if(buildingReq[building.Name][reqName].Count <= 0)
                                buildingReq[building.Name].Remove(reqName);
                }
                // need to rework building plan to per planet
                // if(reqName == "Building")
                //     foreach(var name in building.Requirements[reqName]){
                //         buildings.Add((Building)_data.GetData("Building", name),buildings[building]+1);
                //     }
            }
            if(!buildingReq.ContainsKey(building.Name))
                buildingReq.Add(building.Name, building.Requirements);
        }
        return TreeNode.NodeState.Succes;
    }

    TreeNode.NodeState GetTechnologyRequirements(){
        var technology = (Dictionary<Technology, int>)blackBoard["TechnologyToBuild"];
        //var techReq = (Dictionary<string, Dictionary<string, List<string>>>)blackBoard["BuildingRequirements"];
        //bool newEntry = false;
        //var count = 0;
        foreach(var tech in technology.Keys){
            if(tech.Requirements.ContainsKey("Technoloy"))
                foreach(var reqName in tech.Requirements["Technology"]){
                    if(Technologies.FirstOrDefault(x => x.Name == reqName) == null){
                        var newTech = (Technology)_data.GetData("Technology", reqName);
                        if(!technology.ContainsKey(newTech))
                            technology.Add(newTech, technology[tech] + 1);
                        //count++;
                        //newEntry = true;
                    }
                }
            //newEntry = false;
        }
        return TreeNode.NodeState.Succes;
    }

    TreeNode.NodeState GetTechnologyCost(){
        var technology = (Dictionary<Technology, int>)blackBoard["TechnologyToBuild"];
        //var techReq = (Dictionary<string, Dictionary<string, List<string>>>)blackBoard["BuildingRequirements"];
        var resReq = (Dictionary<string, int>)blackBoard["ResourceRequirements"];
        bool noResource = false;
        //var count = 0;
        foreach(var tech in technology.Keys){
            foreach(var resName in tech.BuildCost.Keys){
                if(tech.BuildCost[resName] > 0)
                    if(ResManager.Resources.ContainsKey(resName)){
                        if(ResManager.Resources[resName] < tech.BuildCost[resName]){
                            if(!resReq.ContainsKey(resName)){
                                resReq.Add(resName, tech.BuildCost[resName]);
                            }else{
                                resReq[resName] += (tech.BuildCost[resName] - resReq[resName]);
                            }
                            noResource = true;
                        }
                    }else{
                        if(!resReq.ContainsKey(resName)){
                            resReq.Add(resName, tech.BuildCost[resName]);
                        }else{
                            resReq[resName] += tech.BuildCost[resName];
                        }
                    }
            }
            if(noResource == true)
                return TreeNode.NodeState.Failure;
        }
        return TreeNode.NodeState.Succes;
    }
    
    TreeNode.NodeState ResearchTechnology(){
        if(blackBoard.ContainsKey("TechnologyToBuild")){
            var technology = (Dictionary<Technology, int>)blackBoard["TechnologyToBuild"];
            technology.OrderBy( x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            for(int i = technology.Count - 1; i >= 0; i--){
                var tech = technology.Keys.ElementAt(i);
                if(!Technologies.Contains(tech)){
                    if(ResManager.PayCost(tech.BuildCost)){
                        Research.ConstructBuilding(tech);
                        technology.Remove(tech);
                    }else{
                        return TreeNode.NodeState.Failure;
                    }
                }else{
                    technology.Remove(tech);
                }
            }
            
        }
        return TreeNode.NodeState.Succes;
    }

    // todo: change the way buildings are passed to Planet, add a filter or smth
 
    TreeNode.NodeState ConstructBuilding(Dictionary<Building, int> reqBuildings, Planet planet, Building targetBuilding){
        if(!planet.BuildingsManager.HasBuilding(targetBuilding)){ // needee if the dictionary contains only 1 item
            return ConstructBuildingIfNotHave(reqBuildings, planet, targetBuilding);
        }else{
            var temp = reqBuildings[targetBuilding];
            reqBuildings.Remove(targetBuilding); // remove fist element
            reqBuildings.Append(new KeyValuePair<Building, int>(targetBuilding, temp));
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState ConstructBuildingIfNotHave(Dictionary<Building, int> reqBuildings, Planet planet, Building targetBuilding){
            if(planet.StartConstruction(targetBuilding)){
                reqBuildings.Remove(targetBuilding);
                return TreeNode.NodeState.Succes;
            }else{
                var temp = reqBuildings[targetBuilding];
                reqBuildings.Remove(targetBuilding); // remove fist element
                reqBuildings.Append(new KeyValuePair<Building, int>(targetBuilding, temp));
            }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState ConstructBuilding(){
        if(blackBoard.ContainsKey("BuildConstructor")){
            var planetObj = blackBoard["BuildConstructor"];
            var planets = (List<Planet>)planetObj;
            var reqBuildingsObj = GetBlackBoardObj("BuildingsToBuild");
            var reqBuildings = ((Dictionary<Building, int>)reqBuildingsObj);//.Reverse().ToDictionary(x => x.Key, x => x.Value);
            var buildingReq = (Dictionary<string, Dictionary<string, List<string>>>)blackBoard["BuildingRequirements"];
            blackBoard["BuildingsToBuild"] = reqBuildings = reqBuildings.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            if(planets.Count > 0 && reqBuildings.Count > 0){
                foreach(var planet in planets){
                        for(int i = reqBuildings.Count() - 1; i > -1; i --){ // foreach throws because indexer changes
                            var building = reqBuildings.Keys.ElementAt(i);
                            if(buildingReq[building.Name].Count <= 0 && !planet.BuildingsManager.HasBuilding(building))
                                if(ResManager.CanPayCost(building.BuildCost))
                                    if(building.Products.Count > 0 && building.Type == Building.Category.Mine){
                                        int j = 0;
                                        foreach(string resName in building.Products.Keys){
                                            if(planet.ResourcesManager.Resources.ContainsKey(resName)){
                                                j ++;
                                            }
                                        }
                                        if( j != building.Products.Count){
                                            var temp = reqBuildings[building];
                                            reqBuildings.Remove(building); // remove fist element
                                            reqBuildings.Add(building, temp);
                                        }else{
                                            return ConstructBuildingIfNotHave(reqBuildings, planet, building);
                                        }
                                    }else{
                                        return ConstructBuilding(reqBuildings, planet, building);
                                    }
                        }
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

    bool PlanetHasResourceBuilding(Planet planet, string resName){
        foreach(Building building in planet.BuildingsManager.Buildings){
            if(building.Products.ContainsKey(resName))
                return true;
        }
        return false;   
    }

    TreeNode.NodeState CreateResourceRequest(){
        var constructors = (List<Planet>)blackBoard["BuildConstructor"];
        var resReqObj = blackBoard["ResourceRequirements"]; // resReq - resource requirements
        var resReq = (Dictionary<string, int>)resReqObj;
        if(constructors.Count > 0){
            foreach(Planet planet in constructors){
                foreach(var resName in planet.ResourcesManager.Resources.Keys){
                    if(!PlanetHasResourceBuilding(planet, resName))
                        if(resReq.ContainsKey(resName)){
                            resReq[resName] += 100;
                        }else{
                            resReq.Add(resName, 100);
                        }
                }
            }
        }
        return TreeNode.NodeState.Failure;
    }



    TreeNode.NodeState GetReqResourceBuilding(){
        if(blackBoard.ContainsKey("ResourceRequirements")){
            var reqResObj = GetBlackBoardObj("ResourceRequirements");
            var reqRes = (Dictionary<string, int>)reqResObj;
            blackBoard["ResourceRequirements"] = reqRes = reqRes.OrderBy( x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            var reqBuildObj = GetBlackBoardObj("BuildingsToBuild");
            var reqBuildings = (Dictionary<Building, int>)reqBuildObj;
            reqBuildings.Clear();
            if( reqRes.Count > 0){
                for(int i = reqRes.Count - 1; i >= 0; i--){
                    var resName = reqRes.ElementAt(i).Key;
                    var allBuildings = _data.GetNode("Buildings").GetChildren();
                    var count = reqBuildings.Count;
                    if(allBuildings.Count > 0){
                        foreach(Node node in allBuildings){
                            if(node is Building building){
                                if(building.Products.ContainsKey(resName) 
                                    && !reqBuildings.ContainsKey(building))
                                        reqBuildings.Add(building, reqRes[resName]);
                            }   
                        }
                    }
                }
            }
        }
        return TreeNode.NodeState.Succes;
    }

    TreeNode.NodeState GetReqResourceBuildingStockpile(){

        var state = TreeNode.NodeState.Failure;
        if(blackBoard.ContainsKey("ResourceRequirements")){
            var reqResObj = GetBlackBoardObj("ResourceRequirements");
            var reqRes = (Dictionary<string, int>)reqResObj;
            foreach(var res in ResManager.Resources.Keys){
                if(ResManager.Resources[res] >= ResManager.ResourceLimits[res]){
                    var stockpiles = _data.GetBuildingsList().FirstOrDefault(x => (x.Products.Count == 0 && x.ResourceLimits.ContainsKey(res)));
                    if(stockpiles != null){
                        var reqBuildings = (Dictionary<Building, int>)GetBlackBoardObj("BuildingsToBuild");
                        if(reqBuildings.ContainsKey(stockpiles)){
                            reqBuildings[stockpiles] += 10;
                        }else{
                            reqBuildings.Add(stockpiles, 10);
                        }
                        state = TreeNode.NodeState.Succes;
                    }
                }
            }
        }
        return state;
    }

    TreeNode.NodeState CreateBuildingPlan(){
        var reqBuildings = (Dictionary<Building, int>)blackBoard["BuildingsToBuild"];
        var planets = (List<Planet>)blackBoard["BuildConstructor"];
        var plan = (Dictionary<Planet, Dictionary<Building, int>>)blackBoard["BuildingPlan"];
        var buildingReq = (Dictionary<string, Dictionary<string, List<string>>>)blackBoard["BuildingRequirements"];
        var planetScore = new Dictionary<Planet, int>();
        foreach(var building in reqBuildings.Keys){
            planetScore.Clear();
            if(buildingReq["Building"].Count > 0){
                foreach(var planet in planets){
                    foreach(var buildingName in buildingReq["Building"].Keys){
                        if(planet.BuildingsManager.HasBuilding(buildingName)){
                            if(!planetScore.ContainsKey(planet)){
                                planetScore.Add(planet, 1);
                            }else{
                                planetScore[planet]++;
                            }
                            if(planetScore[planet] == buildingReq["Building"].Count)
                                if(!plan.ContainsKey(planet)){
                                    plan.Add(planet, new Dictionary<Building, int>(){{building, reqBuildings[building]}});
                                    break;
                                }else{
                                    plan[planet].Add(building, reqBuildings[building]);
                                    break;
                                }
                        }
                    }
                    if(!planet.BuildingsManager.HasBuilding(building)){
                        if(!plan.ContainsKey(planet)){
                            plan.Add(planet, new Dictionary<Building, int>(){{building, reqBuildings[building]}});
                        }else{
                            var planetPlan = plan[planet];
                            if(!planetPlan.ContainsKey(building)){
                                planetPlan.Add(building, reqBuildings[building]);
                                reqBuildings.Remove(building);
                            }
                        }
                    }
                }
            }else{
                foreach(var planet in planets){
                    if(!planet.BuildingsManager.HasBuilding(building)){
                        if(!plan.ContainsKey(planet)){
                            plan.Add(planet, new Dictionary<Building, int>(){{building, reqBuildings[building]}});
                        }else{
                            var planetPlan = plan[planet];
                            if(!planetPlan.ContainsKey(building)){
                                planetPlan.Add(building, reqBuildings[building]);
                                reqBuildings.Remove(building);
                            }
                        }
                    }
                }
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

    TreeNode.NodeState CreatePlanetRequest(){
        var mapObj = blackBoard["Map"];
        var map = (Dictionary<string, List<Planet>>)mapObj; 
        var targetsObj = blackBoard["ColonyTargets"];
        var targets = (Dictionary<Planet, int>)targetsObj;
        var invasionsObject = blackBoard["InvasionsInProgress"];
        var invasions = (Dictionary<Ship, Spatial>)invasionsObject;
        Planet target = null;
        int i = 4;
        foreach(string systemName in map.Keys){
            foreach(Planet planet in map[systemName]){
                if(!MapObjects.Contains(planet) && !invasions.ContainsValue(planet))
                    if(!targets.ContainsKey(planet)){
                        i++;
                        if(target == null){
                            target = planet;
                        }else{
                            if(planet.ResourcesManager.Resources.Count > target.ResourcesManager.Resources.Count)
                                target = planet;
                        }
                    }
            }
        }
        if(i > 4){
            targets.Add(target, target.ResourcesManager.Resources.Count());
            return TreeNode.NodeState.Succes;
        }
        return TreeNode.NodeState.Failure;
    }

    TreeNode.NodeState GetAvgPlanetDevLvl(){
        float avg = 0;
        var reqBuildings = (Dictionary<Building, int>)blackBoard["BuildingsToBuild"];
        foreach(Planet planet in Planets){
            float planetBuildingCount = 0;
            float maxPlanetBuildCount = 0;
            foreach(Node node in _data.GetData("Buildings")){
                if(node is Building building){
                    if(building.Type != Building.Category.Mine){
                        maxPlanetBuildCount += reqBuildings.ContainsKey(building) ? (reqBuildings[building]<0 ? 0.9f : reqBuildings[building]) : 1;
                    }else{
                        int count = 0;
                        foreach(var s in building.Products.Keys)
                            if(planet.ResourcesManager.Resources.ContainsKey(s)){
                                count++;
                            }else{
                                break;
                            }
                        if(count == building.Products.Count)
                            maxPlanetBuildCount += reqBuildings.ContainsKey(building) ? (reqBuildings[building]<0 ? 0.9f : reqBuildings[building]) : 1;
                    }
                    if(planet.BuildingsManager.Buildings.Contains(building))
                        planetBuildingCount += reqBuildings.ContainsKey(building) ? (reqBuildings[building]<0 ? 0.9f : reqBuildings[building]) : 1;
                }
            }
            avg += planetBuildingCount/maxPlanetBuildCount;
        }
        AvgPlanetDevelopmentLvl = avg / Planets.Count;
        return TreeNode.NodeState.Succes;
    }

    TreeNode SetupTree(){

        // ResManager
        // WorldMapObjects
        // ResourceRequirements
        // Map
        // Technology
        // data Buildings Units Technology Resources
        // FleetConstructionPlanet
        // BuildConstructor


        // MicroManageUnits && Scout || Conquer && BuildBuilding || BuildUnits || BuildTechnology
        
        // BuildBuilding || BuildUnits || BuildTechnology
        // BuildCost Production ProductionCost Upkeep ResManager Constructor TechLevel Status?

        // ResManager - Production ResQuantity Upkeep
        // TechLevel - for development
        // Status - Peace / War / Conquest? / Growth?


        TreeNode root = null;
        blackBoard.Add("Player", this);
        blackBoard.Add("ConstructUnitID", 0);
        blackBoard.Add("Map", new Dictionary<string, List<Planet>>{});
        blackBoard.Add("ResourceRequirements", new Dictionary<string, int>() );//{{"Resource 2", 1}});

        blackBoard.Add("BuildConstructor", new List<Planet>());
        blackBoard.Add("BuildingsToBuild", new Dictionary<Building, int>());
        //blackBoard.Add("BuildingPlan", new Dictionary<Planet, Dictionary<Building, int>>());
        blackBoard.Add("BuildingRequirements", new Dictionary<string, Dictionary<string, List<string>>>()); // building name, requirement name, requirements array

        blackBoard.Add("TechnologyToBuild", new Dictionary<Technology, int>());
        blackBoard.Add("TechnologyRequirements", new Dictionary<string, Dictionary<string, List<string>>>()); // building name, requirement name, requirements array

        blackBoard.Add("ColonyTargets", new Dictionary<Planet, int>());
        blackBoard.Add("UnitsToRecruit", new Dictionary<string, int>()); // unit name and amount to recruit
        blackBoard.Add("UnitsRecruitPrio", new Dictionary<string, int>()); // prioriutu for unit construction
        blackBoard.Add("FleetConstructionPlanet", new Dictionary<string, int>()); // planet name and unit quantity
        blackBoard.Add("IdleFleets", new Dictionary<Ship, int>()); // ship and its unit count
        blackBoard.Add("InvasionPlans", new Dictionary<Ship, Planet>()); // ship and its target
        blackBoard.Add("InvasionsInProgress", new Dictionary<Ship, Spatial>()); // ship and its target
        blackBoard.Add("ScoutMissions", new Dictionary<Ship, string>()); // ship and its target
        blackBoard.Add("BuildingPriorityList", BuildingPriorityList);

        var scoutSystem = new ScoutSystem(blackBoard);
        scoutSystem._worldMap = _worldMap;

        TreeNode clear = new Parallel( new List<TreeNode> {
            new ActionTN(ClearFinishedScoutMissions), 
            new ActionTN(ClearFinishedInvasionMissions)
        });

        // todo: Add has resources, add recruitment queue based on target planet Reapeter?? 
        TreeNode buildUnits = new Sequence(new List<TreeNode> {
            new ActionTN(CreateFleetRequest),
            new ActionTN(ConstructFleet)
        });

        TreeNode buildBuildings = new Sequence(new List<TreeNode> {
            //new ActionTN(HasBuilding),
            new ActionTN(HasBuildingRequirements),
            //new ActionTN(HasResources),
            new ActionTN(ConstructBuilding)
        });

        TreeNode research = new Sequence(new List<TreeNode> {
            new ActionTN(ResearchTechnology)
        });

        TreeNode getPlanet = new Sequence( new List<TreeNode> {
            new Inverter(
                new List<TreeNode>{
                    new ActionTN(GetReqResourcePlanet)
                }
            ), 
            new ActionTN(CreatePlanetRequest)
        });

        TreeNode getReq = new Parallel(new List<TreeNode>{
            new ActionTN(HasIdleUnitConstructionPlanet),
            new ActionTN(HasIdleBuildConstructionPlanets),
            new ActionTN(GetBuildingResourceRequirements),
            new ActionTN(GetTechnologyRequirements),
            new ActionTN(GetTechnologyCost),
            new ActionTN(CreateResourceRequest),
            new ActionTN(GetReqResourceBuilding),
            getPlanet,
            new ActionTN(GetReqResourceBuildingStockpile),
            new GetIdleShip(blackBoard),
            new ActionTN(GetAvgPlanetDevLvl)
        });

        TreeNode executeInvasion = new Parallel(new List<TreeNode>{
            new ActionTN(PlanInvasion),
            new ActionTN(ExecuteInvasionPlan)
        });

        root = new Parallel(new List<TreeNode> {
            clear, scoutSystem, getReq, research, buildBuildings, buildUnits, executeInvasion
        });

        // root = new Parallel(new List<TreeNode> {
        //     clear, getReq, scoutSystem //, research, buildBuildings
        // });

        return root;
    }

}
