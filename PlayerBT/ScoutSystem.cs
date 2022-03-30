using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ScoutSystem : TreeNode
{

    public ScoutSystem() : base(){}

    public ScoutSystem(Dictionary<string, object> globalContext) : base(globalContext){}

    public Map _worldMap { get; set; }

    Ship ChooseScout(Dictionary<Ship, int> fleets){
        return fleets.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.ElementAt(0);
    }

        StarSystem GetUnscoutedStarSystem(){
        var starSystems = new Godot.Collections.Array(_worldMap.galaxy.GetChildren());
        var map = (Dictionary<string, List<Planet>>)GetGlobalData("Map");
        var scoutMissions = (Dictionary<Ship, string>)GetGlobalData("ScoutMissions");
        foreach(Node node in starSystems){
            if(node is StarSystem starSystem){
                if(!scoutMissions.Values.Contains(starSystem.Name))
                        if(map.ContainsKey(starSystem.Name)){
                            var planetList = map[starSystem.Name];
                            if(planetList.Count <= starSystem.Planets.Count)
                                return starSystem;
                        }else{
                            return starSystem;
                        }    
            }
        }
        return null;
    }

    Vector3 GetPlanetSystemPosition(Planet planet){
        if(planet.GetParent().GetParent() is StarSystem system)
            return system.Transform.origin;
        return planet.Transform.origin;
    }

    public StarSystem GetUnscoutedStarSystem(Vector3 position){
        var starSystems = new Godot.Collections.Array(_worldMap.galaxy.GetChildren());
        var map = (Dictionary<string, List<Planet>>)GetGlobalData("Map");
        var scoutMissions = (Dictionary<Ship, string>)GetGlobalData("ScoutMissions");
        StarSystem target = null;
        foreach(Node node in starSystems){
            if(node is StarSystem starSystem){
                if(!scoutMissions.Values.Contains(starSystem.Name))
                    if(target == null){
                        target = starSystem;
                    }else{
                        if(map.ContainsKey(starSystem.Name)){
                            var planetList = map[starSystem.Name];
                            if(planetList.Count <= starSystem.Planets.Count)
                                if(target.Transform.origin.DistanceTo(position) < starSystem.Transform.origin.DistanceTo(position))
                                    target = starSystem;
                        }else{
                            if(target.Transform.origin.DistanceTo(position) < starSystem.Transform.origin.DistanceTo(position))
                                target = starSystem;
                        }    
                    }
            }
        }
        return target;
    }

    public override TreeNode.NodeState Evaluate(){

        var playerObject = GetGlobalData("Player");
        var fleetsObject = GetGlobalData("IdleFleets");

        if(playerObject != null && fleetsObject != null){
            var player = (AIPlayer)playerObject;
            var fleets = (Dictionary<Ship, int>)fleetsObject;
            var planet = player.GetFirstPlanet();
            var position = GetPlanetSystemPosition(planet);
            if(planet != null && fleets.Count > 0){
                if(planet.System != null){
                    var map = (Dictionary<string, List<Planet>>)GetGlobalData("Map");
                    var scoutMissionsObj = GetGlobalData("ScoutMissions");
                    var scoutMissions = (Dictionary<Ship, string>)scoutMissionsObj;
                        if(scoutMissions.ContainsValue(planet.System.Name)){
                            State = NodeState.Succes;
                            return NodeState.Succes;  
                        }
                    var scout = ChooseScout(fleets);
                    StarSystem system;
                    if(map.ContainsKey(planet.System.Name)){
                        var count = (_worldMap.galaxy.StarSystems.FirstOrDefault( x => x.Name == planet.System.Name)).Planets.Count;
                        if(map[planet.System.Name].Count == count){
                            system = GetUnscoutedStarSystem(position);
                        }else{
                            system = planet.System;
                        }
                    }else{
                        system = planet.System;
                    }
                    if(system != null){
                        if(scout.MapObject != system){
                            scout.MoveToTarget(system);
                        }
                        foreach(Node node in system.StarSysObjects.GetChildren()){
                            if(node is Planet targetPlanet && node != planet){
                                var target = scout.GetTempWaypoint(targetPlanet.GlobalTransform.origin);
                                if(scout.targetManager.HasTarget){
                                    scout.targetManager.AddTarget(target);
                                }else{
                                    scout.targetManager.SetTarget(target);
                                }
                            }
                        }
                        fleets.Remove(scout);
                        scoutMissions.Add(scout, system.Name);
                        State = NodeState.Succes;
                        return NodeState.Succes;
                    }
                }
            }
        }

        State = NodeState.Failure;
        return NodeState.Failure;
    }
}
