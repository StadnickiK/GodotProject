using Godot;
using System;

public interface IMergable : INode, IMapObjectController
{
    public void Merge(UnitController unitController, ISelectMapObject selectMapObject = null);
}
