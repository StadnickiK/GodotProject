using Godot;
using System;

public partial class DeployZone : Area3D
{
    private Vector3 size;


    public MeshInstance3D Mesh { get; set; }

    public CollisionShape3D Collision { get; set; }

    [Export]
    public Color ZoneColor { get; set; }

    [Export]
    public Vector3 MinSize { get; set; }

    public Vector3 Size { get {return size; } set {size = value; UpdateSize(value); } }

    public override void _Ready()
    {
        base._Ready();
        Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        Collision = GetNode<CollisionShape3D>("CollisionShape3D");
        ResetSize();
    }
    
    public void UpdateSize(Vector3 size)
    {
        var collBox = (BoxShape3D)Collision.Shape;
        var meshBox = (BoxMesh)Mesh.Mesh;
        collBox.Size = size;
        meshBox.Size = size;
    }

    public void ResetSize()
    {
        UpdateSize(MinSize);
    }

    public void UpdatePosition(Vector3 pos)
    {
        Position = pos;
    }
}
