using Godot;
using System;

public class VisionArea : Area
{
    [Export]
    public int VisionRange { get; set; } = 4;

    public void UpdateVisionRange(){
        Scale = new Vector3(VisionRange, 0.1f, VisionRange);
    }

    public void UpdateVisionRange(int range){
        VisionRange = range;
        Scale = new Vector3(VisionRange, 0.1f, VisionRange);
    }
    public override void _Ready()
    {
        UpdateVisionRange();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
