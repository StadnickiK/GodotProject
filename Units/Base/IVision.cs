using Godot;
using System;

public interface IVision : IMapObjectController, IVisible
{
    VisionArea _area { get; }

    int VisionRange { get; set; }

}
