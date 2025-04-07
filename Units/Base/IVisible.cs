using Godot;
using System;

public interface IVisible : IMapObjectController
{
    //bool Visible { get; set; }

    public VisibilityConroller VisibilityConroller { get; set; }

    void ChangeVision(VisibilityConroller.VisibilityStruct visibilityStruct ,int playerID);

    public int ReturnIndex();

    public void SetVisibility(bool visible);

    public bool ReturnVisible();
}
