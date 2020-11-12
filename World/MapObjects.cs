using Godot;
using System;
using System.Collections.Generic;

public class MapObjects : Node
{
    public List<Ship> Ships { get; set; } = new List<Ship>();

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
