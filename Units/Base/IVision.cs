using Godot;
using System;

public interface IVision : IVisible
{
    VisionComponent _area { get; }

    int VisionRange { get; set; }

}
