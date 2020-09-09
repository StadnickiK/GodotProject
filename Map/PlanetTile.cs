using Godot;
using System;
using System.Collections.Generic;

public class PlanetTile : Spatial
{

    [Export]
    int Size = 10;

    public List<Building> Buildings { get; set; } = new List<Building>();

    public List<Resource> Resources { get; set; } = new List<Resource>();

    public Random Rand { get; set; } = new Random();

    private Terrain _terrainType = Terrain.Plains;
    public Terrain TerrainType
    {
        get { return _terrainType; }
        set { _terrainType = value; }
    }
    
    PackedScene BuildingScene = null;
    PackedScene ResourceScene = null;

    public enum Terrain
    {
        Urban,
        Plains,
        Ruins,
        Forest,
        Swamp,
        Mountains
    }

    void Generate(){
        TerrainType = (Terrain)Rand.Next(0,5);
        Resources.Add(new Resource());
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BuildingScene = (PackedScene)GD.Load("res://Map/Building.tscn");
        ResourceScene = (PackedScene)GD.Load("res://Map/Resource.tscn");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
