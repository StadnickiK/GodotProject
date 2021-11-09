using Godot;
using System;

public interface IMapObjectControllerChanger : IMapObjectController
{
    void ChangeController(Player player);
}
