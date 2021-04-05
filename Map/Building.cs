using Godot;
using System;
using System.Collections.Generic;

public class Building : Node
{

    public List<Resource> Resources { get; set; } = new List<Resource>();

    public List<Resource> Products { get; set; } = new List<Resource>();

    public List<Resource> BuildCost { get; set; } = new List<Resource>();

    public List<Resource> ProductCost { get; set; } = new List<Resource>();

    public bool HasProductCost { get; set; } = false;

    public int ResourceLimit { get; set; }

    public int BuildTime { get; set; } = 5;

    public int CurrentTime { get; set; } = 0;

    public override void _Ready()
    {
        
    }
}
