using Godot;
using System;

public interface IRequirements
{
    Godot.Collections.Dictionary<string, string[]> Requirements { get; set; }

}
