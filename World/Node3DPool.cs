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


public interface ISavingNode
{
    event Node3DPool.SaveNodeEventHandler SaveNode;

    void InvokeSaveNode();

    public void BeforeSave();
}

public class SavedNode3D
{
    public Node3D Node { get; set; }

    public uint CollisionLayer { get; set; }

    public uint CollisionMask { get; set; }

    public CollisionObject3D CollisionObject3D { get; set; }

    public CollisionShape3D CollisionShape3D { get; set; }
}

public partial class Node3DPool : Node3D
{

    [Export]
    public int MinAmount { get; set; } = 5;

    [Export]
    public int InitNodeAmount { get; set; } = 50;

    // [Export]
    // public Vector3 Spawnpoint { get; set; } = new Vector3(0,-200,0);

    [Export]
    public string Node3DPath { get; set; } = "res://Units/Base/Ship.tscn";

    PackedScene _Node3DScene;
    public List<SavedNode3D> AvaiableNode3Ds { get; set; } = new List<SavedNode3D>();

    Dictionary<Node3D, SavedNode3D> UsedNodes { get; set; } = new Dictionary<Node3D, SavedNode3D> (); 

    public delegate void SaveNodeEventHandler(Node3D node);

    public override void _Ready()
    {
        _Node3DScene = (PackedScene)ResourceLoader.Load(Node3DPath);
        foreach (var item in GetChildren())
        {
            if (item is Node3D Node3D)
            {
                Node3D.Hide();
                if (Node3D is ISavingNode node) node.SaveNode += SaveNode3D;
                AvaiableNode3Ds.Add(GetSavedNode3D(Node3D));
            }
        }
        InitAmount(InitNodeAmount);
    }

    SavedNode3D GetSavedNode3D(Node3D Node3D)
    {
        var coll = Node3D.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
        var saveNode = new SavedNode3D()
        {
            Node = Node3D, 
            CollisionShape3D = coll
        };
        if(Node3D is CollisionObject3D collisionObject3D)
        {
            saveNode.CollisionObject3D = collisionObject3D;
            saveNode.CollisionLayer = collisionObject3D.CollisionLayer;
            saveNode.CollisionMask = collisionObject3D.CollisionMask;
        }
            
        return saveNode;
    }

    public Node3D GetNode3D(Node3DModel model)
    {
        return GetNode3D(model.Parent, model.Position, model.Name, model.Visible);
    }

    public Node3D GetNode3D(Node parent, Vector3 position, string name, bool visible = false)
    {
        var Node3D = GetNode3D(parent, name, visible);
        UpdatePosition(Node3D, position);
        return Node3D;
    }

    public Node3D GetNode3D(Node parent, Vector3 position, bool visible = false)
    {
        var Node3D = GetNode3D(parent, visible);
        UpdatePosition(Node3D, position);
        return Node3D;
    }

    public Node3D GetNode3D(Node parent, string name, bool visible = false)
    {
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
        if(!UsedNodes.ContainsKey(Node3D.Node)) 
            UsedNodes.Add(Node3D.Node, Node3D);

        Node3D.CollisionObject3D.CollisionLayer = Node3D.CollisionLayer;
        Node3D.CollisionObject3D.CollisionMask = Node3D.CollisionMask;

        //Node3D.SetDeferred("disabled", false);
        //Node3D.ProcessMode = ProcessModeEnum.Inherit;
        Node3D.CollisionShape3D?.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
        Node3D.Node.SetDeferred(Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Inherit);
        Node3D.Node.Visible = visible;
        Node3D.Node.GetParent()?.RemoveChild(Node3D.Node);
        if(Node3D.Node.GetParent() == null) parent.AddChild(Node3D.Node);
        AvaiableNode3Ds.RemoveAt(AvaiableNode3Ds.Count - 1);
        
        return Node3D.Node;
    }

    public void SaveNode3D(Node3D Node3D)
    {
        if(Node3D is ISavingNode savingNode) savingNode.BeforeSave();
        //Node3D.GetParent()?.RemoveChild(Node3D);
        var parent = Node3D.GetParent();

        if(parent != null) 
            Node3D.CallDeferred(Node.MethodName.Reparent, this);
        else 
            CallDeferred(Node.MethodName.AddChild, Node3D);

        Node3D.Hide();
        UpdatePosition(Node3D, GlobalPosition);
        //Node3D.SetDeferred(Node3D.PropertyName.Transform, GlobalPosition);
        SavedNode3D node;
        if (UsedNodes.ContainsKey(Node3D))
        {
            node = UsedNodes[Node3D];
            if(node.CollisionObject3D != null)
            {
                node.CollisionObject3D.CollisionLayer = 0;
                node.CollisionObject3D.CollisionMask = 0;
                //PhysicsServer3D.BodySetState(node.CollisionObject3D.GetRid(), PhysicsServer3D.BodyState.Transform, GlobalPosition);
            }
        }
        else
        {
            node = GetSavedNode3D(Node3D);
            if(node.CollisionObject3D != null)
            {
                node.CollisionLayer = node.CollisionObject3D.CollisionLayer;
                node.CollisionMask = node.CollisionObject3D.CollisionMask;
                node.CollisionObject3D.CollisionLayer = 0;
                node.CollisionObject3D.CollisionMask = 0;
                //PhysicsServer3D.BodySetState(node.CollisionObject3D.GetRid(), PhysicsServer3D.BodyState.Transform, GlobalPosition);
            }
        }
        node.CollisionShape3D?.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
        node.Node.SetDeferred(Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Disabled);
        //UpdatePosition(Node3D, Spawnpoint);
        AvaiableNode3Ds.Add(node);
    }

    Node3D GetNewNode3D()
    {
        var Node3D = (Node3D)_Node3DScene.Instantiate();
        if (Node3D is ISavingNode node) node.SaveNode += SaveNode3D;
        return Node3D;
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

    public void ConnectSaveNode(ISavingNode node)
    {
        node.SaveNode -= SaveNode3D;
        node.SaveNode += SaveNode3D;
    }
}
