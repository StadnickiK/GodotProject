using Godot;
using System;
using System.Collections.Generic;

public class Player : Node
{
    public int PlayerID { get; set; }

    public string PlayerName { get; set; }

    public bool MapObjectsChanged { get; set; } = true;

    private List<PhysicsBody> _MapObejcts = new List<PhysicsBody>();
    public List<PhysicsBody> MapObjects
    {
        get { return _MapObejcts; }
    }

    public PhysicsBody GetMapObjectByName(string name){
        foreach(PhysicsBody body in _MapObejcts){
            GD.Print(body.Name+"+" +" "+name+"+");
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
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
