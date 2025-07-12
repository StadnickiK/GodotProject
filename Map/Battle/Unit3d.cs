using Godot;
using System;


public class Unit3dModel : Node3DModel, IMapObjectController
{
    public Player Controller { get; set; }

}

public partial class Unit3d : RigidBody3D, IMapObjectController, IVisible
{

    public VisibilityConroller VisibilityConroller { get; set; }

    public Player Controller { get; set; }
    public MeshInstance3D MeshInstance3D { get; set; }
    public CollisionShape3D CollisionShape3D { get; set; }

    public override void _Ready()
    {
        GetNodes();
    }

    private void GetNodes()
    {
        MeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
        CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        VisibilityConroller = GetNode<VisibilityConroller>("VisibilityConroller");
    }


    public Vector3 GetUnitSize()
    {
        return MeshInstance3D.GetAabb().Size;
    }

    public void ChangeVision(VisibilityConroller.VisibilityStruct visibilityStruct, int playerID)
    {
        throw new NotImplementedException();
    }

    public int ReturnIndex()
    {
        return GetIndex();
    }

    public void SetVisibility(bool visible)
    {
        Visible = visible;
    }

    public bool ReturnVisible()
    {
        return Visible;
    }

}
