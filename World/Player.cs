using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : Node
{
    public int PlayerID { get; set; }

    public string PlayerName { get; set; }

    public bool IsLocal { get; set; } = true;

    public bool MapObjectsChanged { get; set; } = true;

    [Export]
    public int TimeStep { get; set; } = 1;

    float _time = 0;

    private List<PhysicsBody> _MapObejcts = new List<PhysicsBody>();
    public List<PhysicsBody> MapObjects
    {
        get { return _MapObejcts; }
    }

    private Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
    public Dictionary<string, Resource> Resources
    {
        get { return _resources; }
    }

    public bool ResourcesChanged { get; set; } = false;

    public PhysicsBody GetMapObjectByName(string name){
        foreach(PhysicsBody body in _MapObejcts){
            //GD.Print(body.Name+"+" +" "+name+"+");
            if(body.Name == name){
                return body;
            }
        }
        return null;
    }    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerID = GetIndex();
        PlayerName = "Player "+ PlayerID;

        for(int i = 0; i<5; i++){
            var resource = new Resource();
            resource.Name = "resource "+i;
            //resource.Quantity = i * 1000;
            Resources.Add(resource.Name, resource);
            ResourcesChanged = true;
        }

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta){
        _time += delta;
        if(_time >= TimeStep){
            Resources.Clear();
            foreach(Planet planet in MapObjects.Where( x => x is Planet )){
                if(planet.ResourcesChanged){
                    foreach(Resource resource in planet.Resources.Values){
                        if(Resources.ContainsKey(resource.Name)){
                            //Resources[resource.Name].Quantity += resource.Quantity;
                            Resources[resource.Name].Value += resource.Value;
                        }else{
                            Resources.Add(resource.Name,resource);
                        }
                    }
                    planet.ResourcesChanged = false;
                }
            }
            ResourcesChanged = true;
            _time = 0;
        }
    }

}
