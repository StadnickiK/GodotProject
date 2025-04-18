using Godot;
using System;

public interface IConstruct : IBuildTime, IBuildCost, IRequirements
{
    string ConstructName { get; set; }
}
