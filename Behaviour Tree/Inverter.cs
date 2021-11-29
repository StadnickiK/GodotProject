using Godot;
using System;
using System.Collections.Generic;

public class Inverter : TreeNode
{

    public Inverter() : base(){}

    public Inverter(List<TreeNode> nodes) : base(nodes){}

    public Inverter(Godot.Collections.Array nodes): base(nodes){}

    public override NodeState Evaluate()
    {
        if(GetChildCount() <= 0) return NodeState.Failure;


        if(GetChildren()[0] is TreeNode node)
            switch(node.Evaluate()){
                case NodeState.Failure:
                    _state = NodeState.Succes;
                    return State;
                case NodeState.Succes:
                    _state = NodeState.Failure;
                    return State;
                case NodeState.Running:
                    _state = NodeState.Running;
                    return State;
                default:
                    _state = NodeState.Failure;
                    return _state;
            }

        return NodeState.Failure;
    }


}
