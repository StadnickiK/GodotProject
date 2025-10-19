using Godot;
using System;

public interface IConstruct : IBuildTime, IBuildCost, IRequirements, INode
{
    string ConstructName { get; set; }
}
