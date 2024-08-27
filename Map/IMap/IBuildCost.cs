using Godot;
using System;

public interface IBuildCost
{
    System.Collections.Generic.Dictionary<int, int> BuildCost { get; set; }
    
}
