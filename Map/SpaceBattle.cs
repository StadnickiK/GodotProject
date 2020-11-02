using Godot;
using System;
using System.Collections.Generic;

public class SpaceBattle : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public List<PhysicsBody> Comabatants { get; set; } = new List<PhysicsBody>();

    public List<PhysicsBody> Debris { get; set; } = new List<PhysicsBody>();

    public void SetPosition(Vector3 pos){
        var trans = Transform;
        trans.origin = pos;
        Transform = trans;
    }

    public void AddCombatant(PhysicsBody body){
        Comabatants.Add(body);
    }

    public void AddCombatants(params PhysicsBody[] body){
        foreach(PhysicsBody b in body){
            Comabatants.Add(b);
        }
    }
   
    public void RemoveCombatant(PhysicsBody body){
        Comabatants.Remove(body);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    void GetNodes(){
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
