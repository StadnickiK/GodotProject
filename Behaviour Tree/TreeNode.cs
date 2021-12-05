using Godot;
using System;
using System.Collections.Generic;

public class TreeNode : Node
{

    public enum NodeState{
        Running,
        Succes,
        Failure
    }

    protected NodeState _state;
    public NodeState State
    {
        get { return _state; }
        set { _state = value; }
    }
    
    private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

    private Dictionary<string, object> _globalContext = new Dictionary<string, object>();

    public TreeNode(){}

    public TreeNode(Dictionary<string, object> globalContext ){
        _globalContext = globalContext;
    }

    public TreeNode(List<TreeNode> nodes, Dictionary<string, object> globalContext ){
        AddChildren(nodes);
        _globalContext = globalContext;
    }

    public TreeNode(Godot.Collections.Array nodes, Dictionary<string, object> globalContext){
        AddChildren(nodes);
        _globalContext = globalContext;
    }

    public TreeNode(List<TreeNode> nodes){
        AddChildren(nodes);
    }

    public TreeNode(Godot.Collections.Array nodes){
        AddChildren(nodes);
    }

    public void AddChildren(List<TreeNode> nodes){
        foreach(TreeNode node in nodes)
            AddChild(node);
    }

    public void AddChildren(Godot.Collections.Array nodes){
        foreach(Node node in nodes){
            AddChild(node);
        }
    }

    public object GetData(string key){
        object value = null;
        if(_dataContext.TryGetValue(key, out value))
            return value;

        Node n = GetParent();
        
            while(n != null){
                if(n is TreeNode node){
                    value = node.GetData(key);
                    if(value != null)
                        return value;
                    n = node.GetParent();
                }else{
                    return null;
                }
            }

        return null;
    }

    public bool RemoveData(string key){
        
        if(_dataContext.ContainsKey(key)){
            _dataContext.Remove(key);
            return true;
        }

        Node n = GetParent();
        
            while(n != null){
                if(n is TreeNode node){
                    bool cleared = node.RemoveData(key);
                    if (cleared)
                        return true;

                    n = node.GetParent();  
                }else{
                    return false;
                }
            }

        return false;
    }

    public void SetData(string key, object value){
        _dataContext.Add(key, value);
    }

    public object GetGlobalData(string key){
        object value = null;
        if(_globalContext.TryGetValue(key, out value))
            return value;

        Node n = GetParent();
        
            while(n != null){
                if(n is TreeNode node){
                    value = node.GetData(key);
                    if(value != null)
                        return value;
                    n = node.GetParent();
                }else{
                    return null;
                }
            }

        return null;
    }

    public bool RemoveGloabalData(string key){
        
        if(_globalContext.ContainsKey(key)){
            _globalContext.Remove(key);
            return true;
        }

        Node n = GetParent();
        
            while(n != null){
                if(n is TreeNode node){
                    bool cleared = node.RemoveData(key);
                    if (cleared)
                        return true;

                    n = node.GetParent();  
                }else{
                    return false;
                }
            }

        return false;
    }

    public void SetGlobalData(string key, object value){
        if(!_globalContext.ContainsKey(key))
            _globalContext.Add(key, value);
    }


    public override void _Ready()
    {
        
    }

    public virtual NodeState Evaluate() => NodeState.Failure;

    //public virtual Task<NodeState> TEvaluate() => NodeState.Failure;

}
