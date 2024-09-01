using Godot;
using System;
using System.Threading.Tasks;

public partial class ActionTN : TreeNode
{
    public delegate NodeState ActionNodeDelegate();

    private ActionNodeDelegate action = null;

    public ActionTN(ActionNodeDelegate actionDelegate){
        action = actionDelegate;
    }

    public override NodeState Evaluate()
    {
        switch(action()){
            case NodeState.Succes:
                State = NodeState.Succes;
                return State;
            case NodeState.Failure:
                State = NodeState.Failure;
                return State;
            case NodeState.Running:
                State = NodeState.Running;
                return State;
            default:
                State = NodeState.Failure;
                return State;
        }
    }

    public override async Task<NodeState> EvaluateAsync(){
        return Evaluate();
    }
}
