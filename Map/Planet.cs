using Godot;
using System;
using System.Collections.Generic;

public class Planet : RigidBody
{

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    public List<PlanetTile> MyProperty { get; set; } = new List<PlanetTile>();

    public string PlanetName { get; set; }

    public enum Type
    {
        Balanced,
        Aggro,
        MegaCity,
        Colony,
        Mine,
        Industrial,
        HeavyIndustrial,
        HellIndustrial
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
}
