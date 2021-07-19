using Godot;
using System;

public interface IBuildCost
{
    Godot.Collections.Dictionary<string, int> BuildCost { get; set; }
    
}
