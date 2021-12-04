using Godot;
using System;
using System.Collections.Generic;

public class Timer : TreeNode
{
    private float _delay;

    private float _time;

    public delegate void TickEnded();

    public event TickEnded onTickEnded;

    public Timer(float delay, TickEnded onTickEnded = null) : base()
    {
        _delay = delay;
        _time = delay;
        this.onTickEnded = onTickEnded;
    }

    public Timer(float delay,List<TreeNode> nodes, TickEnded onTickEnded = null) : base(nodes)
    {
        _delay = delay;
        _time = delay;
        this.onTickEnded = onTickEnded;
    }

    public Timer(float delay,Godot.Collections.Array nodes, TickEnded onTickEnded = null) : base(nodes)
    {
        _delay = delay;
        _time = delay;
        this.onTickEnded = onTickEnded;
    }

    public override NodeState Evaluate(){
        if(GetChildren().Count == 0) return NodeState.Failure;

        if(GetChildren()[0] is TreeNode node)
        if(_time <= 0){
            _time = _delay;
            _state = node.Evaluate();
            if (onTickEnded != null)
                onTickEnded();
            _state = NodeState.Succes;
        }else{
            _time -= GetProcessDeltaTime();
            _state = NodeState.Running;
        }
        return State;
    }

}
