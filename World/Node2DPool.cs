using Godot;
using System;
using System.Collections.Generic;

public partial class Node2DPool : Node
{

    public delegate void FreeNodeEventHandler(Node node);

    [Export]
    public int MinAmount { get; set; } = 5;

    [Export]
    public string NodePath { get; set; } = "res://Data/Units/Unit 1.tscn";

    PackedScene _NodeScene;
    public List<Node> AvaiableNodes { get; set; } = new List<Node>();

	public override void _Ready()
	{
        _NodeScene = (PackedScene)ResourceLoader.Load(NodePath);
        foreach (var node in GetChildren()){
            if(node is Node Node){
                AvaiableNodes.Add(Node);
            }
        }
        InitAmount(MinAmount);
    }

    public Node GetNode(Node parent, string name){
        if(AvaiableNodes.Count - 1 == 0){
            InitAmount(MinAmount);
        }
        var Node = AvaiableNodes[AvaiableNodes.Count - 1];
        Node.Name = name;
        RemoveChild(Node);
        parent.AddChild(Node);
        AvaiableNodes.RemoveAt(AvaiableNodes.Count - 1);
        return Node;
    }

    public void SaveNode(Node Node){
        Node.GetParent()?.RemoveChild(Node);
        AddChild(Node);
        //UpdatePosition(Node, Spawnpoint);
        AvaiableNodes.Add(Node);
    }

    Node GetNewNode(){
        return (Node)_NodeScene.Instantiate();
    }

    public void InitAmount(int amount = 1){
        if(AvaiableNodes.Count < amount){
            for(int i = 0; i < amount;i++)
                SaveNode(GetNewNode());
        }
    }
}
