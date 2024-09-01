using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Parallel : TreeNode
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

    public override async Task<NodeState> EvaluateAsync() // recreate as Task
    {
        bool AnyChildRunning = false;
        var taksList = new List<Task<NodeState>>();

        foreach(Node n in GetChildren()){
            if(n is TreeNode node)
            {
                taksList.Add(Task.Run(() => { return node.Evaluate(); }));
            }
        }

        var list = await Task.WhenAll(taksList);

        if(list.Select(x => x == NodeState.Failure).Count() == GetChildCount()){
            _state = NodeState.Failure;
        }else{
            _state = AnyChildRunning ? NodeState.Running : NodeState.Succes;
        }

        return _state;
    }

}
