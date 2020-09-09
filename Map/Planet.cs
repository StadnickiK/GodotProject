using Godot;
using System;
using System.Collections.Generic;

public class Planet : RigidBody
{

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    public List<PlanetTile> PlanetTiles { get; set; } = new List<PlanetTile>();

    public Random Rand { get; set; } = new Random();

    public string PlanetName { get; set; }

    PackedScene PlanetTileScene = null;

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

    void Generate(){
        Size = Rand.Next(5,15);
        Scale *= Size/10;
        Size = Size%2==0 ? Size : ++Size;
        for(int i = 0; i<Size;i++){
            PlanetTile pt =  (PlanetTile)PlanetTileScene.Instance();
            pt.Rand = Rand;
            PlanetTiles.Add(pt);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlanetTileScene = (PackedScene)GD.Load("res://Map/PlanetTile.tscn");
        Generate();
    }
}
