using Godot;
using System;
using System.Collections.Generic;

public partial class Node3DPool : Node3D
{

    [Export]
    public int MinAmount { get; set; } = 5;

    // [Export]
    // public Vector3 Spawnpoint { get; set; } = new Vector3(0,-200,0);

    [Export]
    public string Node3DPath { get; set; } = "res://Units/Base/Ship.tscn";

    PackedScene _Node3DScene;
    public List<Node3D> AvaiableNode3Ds { get; set; } = new List<Node3D>();

	public override void _Ready()
	{
        _Node3DScene = (PackedScene)ResourceLoader.Load(Node3DPath);
        foreach (var node in GetChildren()){
            if(node is Node3D Node3D){
                AvaiableNode3Ds.Add(Node3D);
            }
        }
        InitAmount(MinAmount);
    }

    public Node3D GetNode3D(Node parent, Vector3 position, string name){
        if(AvaiableNode3Ds.Count - 1 == 0){
            InitAmount(MinAmount);
        }
        var Node3D = AvaiableNode3Ds[AvaiableNode3Ds.Count - 1];
        Node3D.Name = name;
        UpdatePosition(Node3D, position);
        RemoveChild(Node3D);
        parent.AddChild(Node3D);
        AvaiableNode3Ds.RemoveAt(AvaiableNode3Ds.Count - 1);
        return Node3D;
    }

    public void SaveNode3D(Node3D Node3D){
        Node3D.GetParent()?.RemoveChild(Node3D);
        AddChild(Node3D);
        //UpdatePosition(Node3D, Spawnpoint);
        AvaiableNode3Ds.Add(Node3D);
    }

    Node3D GetNewNode3D(){
        return (Node3D)_Node3DScene.Instantiate();
    }

    public void InitAmount(int amount = 1){
        if(AvaiableNode3Ds.Count < amount){
            for(int i = 0; i < amount;i++)
                SaveNode3D(GetNewNode3D());
        }
    }

    void UpdatePosition(Node3D Node3D, Vector3 position){
        var transform = Node3D.Transform;
		transform.Origin = position;
		Node3D.Transform = transform;
    }
}
