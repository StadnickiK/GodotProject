using Godot;
using System;
using System.Collections.Generic;


public class Node3DModel
{
    public Node Parent { get; set; }
	public Vector3 Position { get; set; }

    public StringName Name { get; set; }

    public bool Visible { get; set; }    
}

public partial class Node3DPool : Node
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
        foreach (var node in GetChildren())
        {
            if (node is Node3D Node3D)
            {
                Node3D.Hide();
                AvaiableNode3Ds.Add(Node3D);
            }
        }
        InitAmount(MinAmount);
    }

    public Node3D GetNode3D(Node3DModel model)
    {
        if (AvaiableNode3Ds.Count - 1 == 0)
        {
            InitAmount(MinAmount);
        }
        return GetNode3D(model.Parent, model.Position, model.Name, model.Visible);
    }

    public Node3D GetNode3D(Node parent, Vector3 position, string name, bool visible = false)
    {
        if (AvaiableNode3Ds.Count - 1 == 0)
        {
            InitAmount(MinAmount);
        }
        var Node3D = GetNode3D(parent, name, visible);
        UpdatePosition(Node3D, position);
        return Node3D;
    }

    public Node3D GetNode3D(Node parent, Vector3 position, bool visible = false)
    {
        if (AvaiableNode3Ds.Count - 1 == 0)
        {
            InitAmount(MinAmount);
        }
        var Node3D = GetNode3D(parent, visible);
        UpdatePosition(Node3D, position);
        return Node3D;
    }

    public Node3D GetNode3D(Node parent, string name, bool visible = false)
    {
        if (AvaiableNode3Ds.Count - 1 == 0)
        {
            InitAmount(MinAmount);
        }
        var Node3D = GetNode3D(parent, visible);
        Node3D.Name = name;
        return Node3D;
    }

    public Node3D GetNode3D(Node parent, bool visible = false)
    {
        if (AvaiableNode3Ds.Count - 1 == 0)
        {
            InitAmount(MinAmount);
        }
        var Node3D = AvaiableNode3Ds[AvaiableNode3Ds.Count - 1];
        Node3D.Visible = visible;
        RemoveChild(Node3D);
        parent.AddChild(Node3D);
        Node3D.ProcessMode = ProcessModeEnum.Inherit;
        AvaiableNode3Ds.RemoveAt(AvaiableNode3Ds.Count - 1);
        return Node3D;
    }

    public void SaveNode3D(Node3D Node3D)
    {
        Node3D.GetParent()?.RemoveChild(Node3D);
        Node3D.Hide();
        AddChild(Node3D);
        Node3D.ProcessMode = ProcessModeEnum.Disabled;
        //UpdatePosition(Node3D, Spawnpoint);
        AvaiableNode3Ds.Add(Node3D);
    }

    Node3D GetNewNode3D()
    {
        return (Node3D)_Node3DScene.Instantiate();
    }

    public void InitAmount(int amount = 1)
    {
        if (AvaiableNode3Ds.Count < amount)
        {
            for (int i = 0; i < amount; i++)
                SaveNode3D(GetNewNode3D());
        }
    }

    void UpdatePosition(Node3D Node3D, Vector3 position)
    {
        var transform = Node3D.Transform;
        transform.Origin = position;
        Node3D.Transform = transform;
    }
}
