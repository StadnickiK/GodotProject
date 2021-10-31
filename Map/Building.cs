using Godot;
using System;
using System.Collections.Generic;

public class Building : Node, IBuilding
{

    // public List<Resource> Resources { get; set; } = new List<Resource>();

    [Export]
    public bool IsStarter { get; set; } = false;

    [Export]
    public Godot.Collections.Dictionary<string, int> Products { get; set; } = new Godot.Collections.Dictionary<string, int>();

    // public List<Resource> Products { get; set; } = new List<Resource>();
    [Export]
    public Godot.Collections.Dictionary<string, int> BuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    // public List<Resource> BuildCost { get; set; } = new List<Resource>();
    [Export]
    public Godot.Collections.Dictionary<string, int> ProductCost { get; set; } = new Godot.Collections.Dictionary<string, int>();
    // public List<Resource> ProductCost { get; set; } = new List<Resource>();

    // public bool HasProductCost { get; set; } = false;
    [Export]
    public Godot.Collections.Dictionary<string, int> ResourceLimits { get; set; } = new Godot.Collections.Dictionary<string, int>();

    // public int ResourceLimit { get; set; }
    [Export]
    public int BuildTime { get; set; } = 5;
    
    [Export]
    public int CurrentTime { get; set; } = 0;

    public override void _Ready()
    {
        
    }
}
