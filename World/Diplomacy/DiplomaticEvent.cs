using Godot;
using System;

public partial class DiplomaticEvent : Node
{
    public string EventName { get; set; }

    public string Description { get; set; }

    public float Score { get; set; } 

    public double Occured { get; set; } 

    public double Duration { get; set; }

    public double Age { get {return GetProcessDeltaTime() - Occured; } }
}
