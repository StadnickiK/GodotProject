using Godot;
using System;
using System.Collections.Generic;

public class GetIdleShip : TreeNode
{

    public GetIdleShip() : base(){}
    public GetIdleShip(Dictionary<string, object> globalContext) : base(globalContext){}

    public override TreeNode.NodeState Evaluate(){

        var playerObject = GetGlobalData("Player");
        var idleFleetsObj = GetGlobalData("IdleFleets");

        if(playerObject != null && idleFleetsObj != null){
            var player = (AIPlayer)playerObject;
            var ship = player.GetIdleShip();
            if(ship != null){
                var fleets = (Dictionary<Ship, int>)idleFleetsObj;
                if(!fleets.ContainsKey(ship)){
                    fleets.Add(ship, ship.Units.GetChildCount());
                    State = NodeState.Succes;
                    return NodeState.Succes;
                }
            }
        }

        State = NodeState.Failure;
        return NodeState.Failure;
    }
}
