using Godot;
using System;
using System.Collections.Generic;

public class Selector : TreeNode
{

    public Selector(): base(){}

    public Selector(List<TreeNode> nodes) : base(nodes){}

    public Selector(Godot.Collections.Array nodes) : base(nodes){}

    public override NodeState Evaluate()
    {
        foreach(Node n in GetChildren()){
            if(n is TreeNode node){
                switch(node.Evaluate()){
                    case NodeState.Failure:
                        continue;
                    case NodeState.Succes:
                        _state = NodeState.Succes;
                        return _state;
                    case NodeState.Running:
                        _state = NodeState.Running;
                        return State;
                    default:
                        continue;

                }
            }
        }
        _state = NodeState.Failure;
        return _state;
    }

    public override void _Ready()
    {
        
    }


}
