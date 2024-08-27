using Godot;
using System;
using System.Collections.Generic;

public partial class ShipBT : BehaviourTree
{
    public ShipBT() : base(){

    }

    protected override TreeNode SetupTree(){
        TreeNode _root = null;
        _root = new Sequence( new List<TreeNode>{
                
            }
        );
        return _root;
    }

}
