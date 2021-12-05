using Godot;
using System;
using System.Collections.Generic;

public class GetIdleShip : TreeNode
{

    public GetIdleShip() : base(){}
    public GetIdleShip(Dictionary<string, object> globalContext) : base(globalContext){}

    public override TreeNode.NodeState Evaluate(){

        var playerObject = GetGlobalData("Player");

        if(playerObject != null){
            var player = (AIPlayer)playerObject;
            var ship = player.GetIdleShip();
            if(ship != null){
                SetGlobalData("Scout", ship);
                State = NodeState.Succes;
                return NodeState.Succes;
            }
        }

        State = NodeState.Failure;
        return NodeState.Failure;
    }
}
