using Godot;
using System;

public partial class Turretjoint : ConeTwistJoint3D
{
    public Turret Turret { get; set; }

    public override void _Ready()
    {
        GetNodes();
    }

    public void GetNodes()
    {
        Turret = GetNode<Turret>("Turret");
    }

}
