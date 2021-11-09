using Godot;
using System;

public interface IVision : IVisible
{
    VisionArea _area { get; }

    int VisionRange { get; set; }

}
