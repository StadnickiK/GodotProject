using Godot;
using System;
using System.Collections.Generic;

public class Building : Node
{

    public List<Resource> Resources { get; set; } = new List<Resource>();

    public List<Resource> Products { get; set; } = new List<Resource>();

    public List<Resource> BuildCost { get; set; } = new List<Resource>();

    public List<Resource> ProductCost { get; set; } = new List<Resource>();

    public int BuildTime { get; set; } = 5;

    public int CurrentTime { get; set; } = 0;

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
