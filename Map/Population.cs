using Godot;
using System;

public class Population : Node
{

    public int Quantity { get; set; }

    public float Growth { get; set; }

    public float GrowthRate { get; set; }
    
    public int Happiness { get; set; }

    //public Race PopRace { get; set; }

    public override void _Ready()
    {
        
    }

}
