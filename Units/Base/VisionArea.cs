using Godot;
using System;

public partial class VisionArea : Area3D
{
    [Export]
    public int VisionRange { get; set; } = 4;

    CollisionShape3D _shape;

    public void UpdateVisionRange(){
        var s = new CylinderShape3D();
        s.Radius = VisionRange;
        _shape.Shape = s;
    }

    public void UpdateVisionRange(int range){
        VisionRange = range;
        var s = new CylinderShape3D();
        s.Radius = VisionRange;
        _shape.Shape = s;
    }
    public override void _Ready()
    {
        _shape = GetChild<CollisionShape3D>(0);
        UpdateVisionRange();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
