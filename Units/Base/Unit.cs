using Godot;
using System;
using System.Collections.Generic;

public class Unit : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    public int ID_Owner { get; set; }

    public bool HasHitpoints { get; set; } = true;  

    public List<BaseStat> Stats { get; set; } = new List<BaseStat>();

    public void CalculateDamage(Unit unit){}

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
