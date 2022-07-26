using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Parallel : TreeNode
{
    public Parallel() : base(){}

    public Parallel(List<TreeNode> nodes) : base(nodes){}

    public Parallel(Godot.Collections.Array nodes) : base(nodes){}

    public override NodeState Evaluate() // recreate as Task
    {
        bool AnyChildRunning = false;
        int nFailedChildren = 0;

        foreach(Node n in GetChildren()){
            if(n is TreeNode node)
            switch(node.Evaluate()){
                case NodeState.Failure:
                    nFailedChildren++;
                    continue;
                case NodeState.Succes:
                    continue;
                case NodeState.Running:
                    AnyChildRunning = true;
                    continue;
                default:
                    _state = NodeState.Succes;
                    return State;
            }
        }

        if(nFailedChildren == GetChildCount()){
            _state = NodeState.Failure;
        }else{
            _state = AnyChildRunning ? NodeState.Running : NodeState.Succes;
        }

        return _state;
    }

    // public async Task<NodeState> EvaluateAsync(){
    //     return _state; 
    // } 

}
