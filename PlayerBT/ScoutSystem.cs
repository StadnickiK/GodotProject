using Godot;
using System;
using System.Collections.Generic;

public class ScoutSystem : TreeNode
{

    public ScoutSystem() : base(){}

    public ScoutSystem(Dictionary<string, object> globalContext) : base(globalContext){}

    public override TreeNode.NodeState Evaluate(){

        var playerObject = GetGlobalData("Player");
        var scoutObject = GetGlobalData("Scout");

        if(playerObject != null && scoutObject != null){
            var player = (AIPlayer)playerObject;
            var scout = (Ship)scoutObject;
            var planet = player.GetPlanet();
            if(planet != null){
                if(planet.System != null){
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
                    State = NodeState.Running;
                    return NodeState.Running;
                }
            }
        }

        State = NodeState.Failure;
        return NodeState.Failure;
    }
}
