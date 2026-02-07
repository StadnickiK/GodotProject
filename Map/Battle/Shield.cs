using Godot;
using System;
using System.Collections.Generic;

public partial class Shield : Area3D, IUpdateStat, IDamagable
{
    public HashSet<string> StatNames { get; set; }

    MeshInstance3D MeshInstance3D { get; set; }

    CollisionShape3D CollisionShape3D { get; set; }

    SphereShape3D sphereShape3D;

    SphereMesh sphereMesh;
    [Export]
    private float radius = 5;
    
    public float Radius { get => radius; set { radius = value; UpdateRadius(value); } }

    public StatManager StatManager { get; set; }

    public Node GetAsNode {get { return this; }}

    public CollisionObject3D GetAsSpecificNode {get { return this; }}

    Node INode.GetAsNode => GetAsNode;


    public override void _Ready()
    {
        MeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
        CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        sphereShape3D = (SphereShape3D)CollisionShape3D.Shape;
        sphereMesh = MeshInstance3D.Mesh as SphereMesh;
    }

    public void UpdateRadius(float value)
    {
        sphereShape3D.Radius = value;
        UpdateMesh(value);

    }

    void UpdateMesh(float value)
    {
        sphereMesh.Radius = value;
        sphereMesh.Height = value * 2;
    }

    public void UpdateStat(IStat stat)
    {
        switch (stat.Name)
        {
            case GlobalStatNames.Shield:
                if ((stat.CurrentValue <= 0 && !stat.HasMinValue) || (stat.CurrentValue < stat.MinValue && stat.HasMinValue))
                {
                    Visible = false;
                    ProcessMode = ProcessModeEnum.Disabled;
                }else
                {
                    Visible = true;
                    ProcessMode = ProcessModeEnum.Inherit;
                }
                break;
        }
    }

    public void Damage()
    {
        throw new NotImplementedException();
    }
}
