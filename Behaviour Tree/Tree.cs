using Godot;
using System;

public abstract class Tree : Node
{
    TreeNode _root = null;

    protected void Start(){
        _root = SetupTree();
    }

    private void Update(){  // probably to change
        if(_root != null)
            _root.Evaluate();
    }

    public TreeNode Root => _root;
    protected abstract TreeNode SetupTree();
}
