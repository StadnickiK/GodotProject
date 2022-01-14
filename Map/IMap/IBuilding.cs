using Godot;
using System;

public interface IBuilding : IBuildTime, IBuildCost, IRequirements
{
    string Name { get; set; }
}
