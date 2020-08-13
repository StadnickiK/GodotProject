using Godot;
using System;

public class StarSystem : Spatial, IGenerator<StarSystem>
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public StarSystem Generate(){
        StarSystem temp = new StarSystem();
        return temp;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
