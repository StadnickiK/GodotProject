using Godot;
using System;

public partial class NavAgent3d : NavigationAgent3D
{
    public CollisionObject3D Parent { get; set; }

    public void Initialize(CollisionObject3D parent, Vector3 size, StatManager statManager)
    {
        Parent = parent;
        Height = size.Y;
        Radius = size.X > size.Z ? size.X/2 : size.Z/2;
        MaxSpeed = statManager.GetStat(GlobalStatNames.Speed).CurrentValue;
        TargetDesiredDistance = statManager.GetStat(GlobalStatNames.Tolerance).CurrentValue;
        //SetNavigationMap
    }

    void _on_navigation_finished()
    {
        
    }

    void _on_velocity_computed(Vector3 velocity)
    {
        
    }

    public void SetTarget(Vector3 pos)
    {
        TargetPosition = pos;
    }

}
