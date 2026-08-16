using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    [Export]
    public float FormationSpacing { get; set; }


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
        this.size = size;
        collBox.Size = size;
        meshBox.Size = size;
    }

    public void ResetSize()
    {
        UpdateSize(MinSize);
    }

    public void UpdatePosition(Vector3 pos)
    {
        GlobalPosition = pos;
    }

    public void PlaceUnits(List<IUnit3D> units, Vector3 center, float yRotation = 0)
    {
        int rows = (int)Mathf.Ceil(Mathf.Sqrt(units.Count));
        int cols = rows;
        int unitIndex = 0;
        Vector3 maxSize = GetMaximumSize(units);

        for (int x = 0; x < cols; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                if (unitIndex >= units.Count)
                    return;

                var unit = units[unitIndex];
                var unitSize = unit.GetUnitSize();
                Vector3 offset = new Vector3(x * (FormationSpacing + maxSize.X/2 + unitSize.X/2), center.Y, z * (FormationSpacing + maxSize.Z + unitSize.Z));
                Vector3 position = center + offset + (GlobalPosition * Vector3.Up);

                // unit.GlobalTransform = new Transform3D(GlobalBasis, position);
                // unit.GlobalRotation = new Vector3(0, yRotation, 0);
                unit.UpdatePosition(position, yRotation);
                unitIndex++;
            }
        }
    }

    private Vector3 GetMaximumSize(List<IUnit3D> units)
    {
        Vector3 max = Vector3.Zero;
        foreach (var item in units)
        {
            if(item.GetUnitSize() > max)
                max = item.GetUnitSize();
        }
        return max; 
    }

}
