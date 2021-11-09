using Godot;
using System;

public interface IVisible : IMapObjectController
{
    bool Visible { get; set; }

    void ChangeVision();

    bool IsVisible();
}
