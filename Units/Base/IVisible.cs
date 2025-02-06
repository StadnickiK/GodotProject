using Godot;
using System;

public interface IVisible : IMapObjectController
{
    //bool Visible { get; set; }

    void ChangeVision();

    void SetVisibility(bool visible);

    public int ReturnIndex();

    public bool ReturnVisible();
}
