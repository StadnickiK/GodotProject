using Godot;
using System;
using System.Collections.Generic;

public class Player : Node
{
    public int PlayerID { get; set; }

    public List<Ship> MapObjects { get; set; } = new List<Ship>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerID = GetIndex();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
