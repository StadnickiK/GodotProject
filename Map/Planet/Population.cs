using Godot;
using System;

public class Population : Node
{

    [Export]
    public int Quantity { get; set; } = 1000000;

    [Export]
    public float Growth { get; set; }

    [Export]
    public float GrowthRate { get; set; }
    
    [Export]
    public int Happiness { get; set; }

    //public Race PopRace { get; set; }

    public override void _Ready()
    {
        
    }

}
