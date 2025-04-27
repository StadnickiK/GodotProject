using Godot;
using System;

public interface IMapObjectController{    
    Player Controller { get; set; }
}

public interface IExtendedMapObjectController : IMapObjectController
{    
    public delegate void ControllerChangedEventHandler(Player controller);

    public event ControllerChangedEventHandler ControllerChanged;
}
