using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Sequence : TreeNode
{
    bool _isRandom = false;

    public Sequence() : base(){}

    //public Sequence(bool isRandom) : base(){ _isRandom = isRandom; }

    public Sequence(List<TreeNode> nodes, bool isRandom = false) : base(nodes){
        _isRandom = isRandom;
    }

    public Sequence(Godot.Collections.Array nodes, bool isRandom = false) : base(nodes){
        _isRandom = isRandom;
    }

    public static List<TreeNode> Shuffle(List<TreeNode> list){
        System.Random r = new Random();
        return list.OrderBy(x => r.Next()).ToList();
    }

    public override NodeState Evaluate()
    {
        bool anyChildRunning = false;

        if(_isRandom)
            GetChildren().Shuffle();    // needs testing

        foreach(Node n in GetChildren()){
            if(n is TreeNode node)
                switch(node.Evaluate()){
                    case NodeState.Failure:
                        _state = NodeState.Failure;
                        return State;
                    case NodeState.Succes:
                        continue;
                    case NodeState.Running:
                        anyChildRunning = true;
                        continue;
                    default:
                        _state = NodeState.Succes;
                        return State;
                }
        }

        _state = anyChildRunning ? NodeState.Running : NodeState.Succes;
        return _state;
    }

}
