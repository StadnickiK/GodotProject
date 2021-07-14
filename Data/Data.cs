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
    

    public Godot.Collections.Dictionary<string, Resource> WorldResources { get; } = new Godot.Collections.Dictionary<string, Resource>();
    
    public List<Building> WorldBuildings { get; set; } = new List<Building>();

    public override void _Ready()
    {
        GetNodes();
        
        WorldBuildings = _BuildingsLoader.WorldBuildings;
    }

    void GetNodes(){
        _ResourcesLoader = GetNode<ResourcesLoader>("ResourcesLoader");
        _BuildingsLoader = GetNode<BuildingLoader>("BuildingLoader");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
