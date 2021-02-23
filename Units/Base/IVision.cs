using Godot;
using System;

public interface IVision 
{
    VisionArea _area { get; }

    bool Visible { get; set; }

    int VisionRange { get; set; }

}
