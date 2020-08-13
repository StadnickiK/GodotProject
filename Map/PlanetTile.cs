using Godot;
using System;

public class PlanetTile : Spatial, IGenerator<PlanetTile>
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    int Size = 10;

    public enum Resource
    {
        None,
        Metal,
        RareMetal,
        Gas,
        RareGas
    }


    public PlanetTile Generate(){
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
