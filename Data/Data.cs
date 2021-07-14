using Godot;
using System;
using System.Collections.Generic;

public class Data : Node
{
    private JSONLoader _resources;
    public JSONLoader Resources
    {
        get { return _resources; }
    }

    private BuildingLoader _BuildingsLoader = null;
    public BuildingLoader BuildingsLoader
    {
        get { return _BuildingsLoader; }
    }
    

    public Godot.Collections.Dictionary<string, Resource> WorldResources { get; } = new Godot.Collections.Dictionary<string, Resource>();
    
    public List<Building> WorldBuildings { get; set; } = new List<Building>();

    public override void _Ready()
    {
        GetNodes();
        TranslateJSON();
        WorldBuildings = _BuildingsLoader.WorldBuildings;
    }

    void GetNodes(){
        _resources = GetNode<JSONLoader>("Resources");
        _BuildingsLoader = GetNode<BuildingLoader>("BuildingLoader");
    }

    void TranslateJSON(){
        if(Resources.Data != null){
            foreach(var resName in Resources.Data.Keys){
                string name = (string)resName;
                var resData = (Godot.Collections.Dictionary)Resources.Data[name];
                Resource.Type type;
                Enum.TryParse((string)resData["Type"], out type);
                var res = new Resource(){
                    Name = name,
                    ResourceType = type,
                    Rarity = (int)(float)resData["Rarity"],
                    IsStarter = (bool)resData["Starter"]
                };
                WorldResources.Add(res.Name, res);
                // GD.Print(res);
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
