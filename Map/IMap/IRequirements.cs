using Godot;
using System.Collections.Generic;

public interface IRequirements
{
    Godot.Collections.Dictionary<string, List<string>> Requirements { get; set; }

}
