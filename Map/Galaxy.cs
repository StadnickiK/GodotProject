using Godot;
using System;
using System.Collections.Generic;

public class Galaxy : Spatial, IGenerator<Galaxy>
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    int size = 10;

    public List<StarSystem> StarSystems { get; set; }


    public enum Type
    {
        Elliptical,
        Spiral,
        Irregular
    }

    public Galaxy Generate(){
        return this;
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
