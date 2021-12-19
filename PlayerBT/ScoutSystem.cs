using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ScoutSystem : TreeNode
{

    public ScoutSystem() : base(){}

    public ScoutSystem(Dictionary<string, object> globalContext) : base(globalContext){}

    Ship ChooseScout(Dictionary<Ship, int> fleets){
        return fleets.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.ElementAt(0);
    }

    public override TreeNode.NodeState Evaluate(){

        var playerObject = GetGlobalData("Player");
        var fleetsObject = GetGlobalData("IdleFleets");

        if(playerObject != null && fleetsObject != null){
            var player = (AIPlayer)playerObject;
            var fleets = (Dictionary<Ship, int>)fleetsObject;
            var planet = player.GetPlanet();
            if(planet != null && fleets.Count > 0){
                if(planet.System != null){
                    var mapObj = GetGlobalData("Map");
                    var scoutMissionsObj = GetGlobalData("ScoutMissions");
                    var scoutMissions = (Dictionary<Ship, string>)scoutMissionsObj;
                        if(scoutMissions.ContainsValue(planet.System.Name)){
                            State = NodeState.Succes;
                            return NodeState.Succes;  
                        }
                    var scout = ChooseScout(fleets);
                    foreach(Node node in planet.System.StarSysObjects.GetChildren()){
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
                    scoutMissions.Add(scout, planet.System.Name);
                    State = NodeState.Running;
                    return NodeState.Running;
                }
            }
        }

        State = NodeState.Failure;
        return NodeState.Failure;
    }
}
