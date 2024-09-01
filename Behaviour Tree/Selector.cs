using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Selector : TreeNode
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

    public override async Task<NodeState> EvaluateAsync() // recreate as Task
    {
        var taksList = new List<Task<NodeState>>();

        foreach(Node n in GetChildren()){
            if(n is TreeNode node)
            {
                taksList.Add(Task.Run(() => { return node.Evaluate(); }));
            }
        }

        var task = await Task.WhenAny(taksList);
        
        // może wymagać użycia Result bo będzie się blokowało
        _state = await task;
        return _state;
    }

    public override void _Ready()
    {
        
    }


}
