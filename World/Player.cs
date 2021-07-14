using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : Node
{
    public int PlayerID { get; set; }

    public string PlayerName { get; set; }

    public bool IsLocal { get; set; } = false;

    public bool MapObjectsChanged { get; set; } = true;

    [Export]
    public int TimeStep { get; set; } = 1;

    float _time = 0;

    private List<PhysicsBody> _MapObejcts = new List<PhysicsBody>();
    public List<PhysicsBody> MapObjects
    {
        get { return _MapObejcts; }
    }

    // private Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
    // public Dictionary<string, Resource> Resources
    // {
    //     get { return _resources; }
    // }

    private ResourceManager _resourceManager = new ResourceManager();

    public ResourceManager ResManager 
    { 
        get { return _resourceManager; } 
    }

    private Dictionary<string, int> _resourceLimits = new Dictionary<string, int>();
    public Dictionary<string, int> ResourceLimits
    {
        get { return _resourceLimits; }
    }

    public bool ResourcesChanged { get; set; } = false;

    // public bool PayCost(List<Resource> BuildCost){
    //             foreach(Resource resource in BuildCost){
    //                 if(Resources.ContainsKey(resource.Name)){
    //                     if(Resources[resource.Name].Value < resource.Quantity){
    //                         return false;
    //                     }
    //                 }else{
    //                     return false;
    //                 }
    //             }
    //     foreach(Resource resource in BuildCost){
    //         Resources[resource.Name].Value -= resource.Quantity;
    //     }
    //     return true;
    // }

    public PhysicsBody GetMapObjectByName(string name){
        foreach(PhysicsBody body in _MapObejcts){
            if(body.Name == name){
                return body;
            }
        }
        return null;
    }    

    public void AddMapObject(PhysicsBody mapObject){
        MapObjects.Add(mapObject);
        MapObjectsChanged = true;
    }

    public void RemoveMapObject(PhysicsBody mapObject){
        MapObjects.Remove(mapObject);
        MapObjectsChanged = true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerID = GetIndex();
        PlayerName = "Player "+ PlayerID;
        Name = PlayerName;

        // for(int i = 0; i<5; i++){
        //     var resource = new Resource();
        //     resource.Name = "resource "+i;
        //     resource.Value = i * 10;
        //     Resources.Add(resource.Name, resource);
        //     ResourcesChanged = true;
        // }
    }

    // void UpdateTempResources(){
    //         Resources.Clear();
    //         foreach(IResourceManager resManager in MapObjects.Where( x => x is IResourceManager )){
    //                 if(resManager.ResourcesManager.ResourcesChanged){
    //                     foreach(Resource resource in resManager.ResourcesManager.Resources.Values){
    //                         if(Resources.ContainsKey(resource.Name)){
    //                             Resources[resource.Name].Value += resource.Value;
    //                         }else{
    //                             Resources.Add(resource.Name,resource);
    //                         }
    //                     }
    //                     resManager.ResourcesManager.ResourcesChanged = false;
    //                 }
    //         }
    //         ResourcesChanged = true;
    // }

    void UpdatePlayerResources(){
        foreach(Planet planet in MapObjects.Where( x => x is Planet )){
                _resourceManager.UpdateResources(planet.BuildingsManager.Buildings);
                UpdateResourceLimit(planet);
                ResourcesChanged = true;
        }
    }

    public void InitResourceLimit(){
        foreach(Planet planet in MapObjects.Where( x => x is Planet )){
                _resourceManager.UpdateResourceLimit(planet.BuildingsManager.Buildings);
        }
    }

    void UpdateResourceLimit(){
        foreach(Planet planet in MapObjects.Where( x => x is Planet )){
            UpdateResourceLimit(planet);
        }
    }

    public void UpdateResourceLimit(Planet planet){
            if(planet.BuildingsChanged){
                _resourceManager.UpdateResourceLimit(planet.BuildingsManager.GetLastBuilding());
                planet.BuildingsChanged = false;
            }
    }

    public override void _Process(float delta){
        _time += delta;
        if(_time >= TimeStep){
            UpdatePlayerResources();
            // UpdateTempResources();
            _time = 0;
        }
    }

}
