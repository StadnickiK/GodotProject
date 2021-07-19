using Godot;
using System;
using System.Collections.Generic;

public class Data : Node
{

    private BuildingLoader _BuildingsLoader = null;
    public BuildingLoader BuildingsLoader
    {
        get { return _BuildingsLoader; }
    }

    private ResourcesLoader _ResourcesLoader = null;
    public ResourcesLoader ResourcesLoader
    {
        get { return _ResourcesLoader; }
    }

    private UnitsLoader _UnitsLoader = null;
    public UnitsLoader UnitsLoader
    {
        get { return _UnitsLoader; }
    }
    

    public Godot.Collections.Dictionary<string, Resource> WorldResources { get; } = new Godot.Collections.Dictionary<string, Resource>();

    // public Godot.Collections.Dictionary<string, Unit> WorldUnits { get; } = new Godot.Collections.Dictionary<string, Unit>();

    public List<Unit> WorldUnits { get; set; } = new List<Unit>();
    
    public List<Building> WorldBuildings { get; set; } = new List<Building>();

    public override void _Ready()
    {
        GetNodes();
        
        WorldBuildings = _BuildingsLoader.WorldBuildings;
        WorldUnits = _UnitsLoader.WorldUnits;
    }

    void GetNodes(){
        _ResourcesLoader = GetNode<ResourcesLoader>("ResourcesLoader");
        _UnitsLoader = GetNode<UnitsLoader>("UnitsLoader");
        _BuildingsLoader = GetNode<BuildingLoader>("BuildingLoader");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
