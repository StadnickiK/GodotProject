using Godot;
using System;
using System.Collections.Generic;

public class Building : Node, IBuilding, IUpkeep
{

    // public List<Resource> Resources { get; set; } = new List<Resource>();

    [Export]
    public bool IsStarter { get; set; } = false;

    [Export]
    public int BuildTime { get; set; } = 5;
    
    public int CurrentTime { get; set; } = 0;

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

    [Export]
    public Dictionary<string, int> Upkeep { get; set; } = new Dictionary<string, int>();

    [Export]
    public Dictionary<string, int> OperationCost { get; set; } = new Dictionary<string, int>();

    [Export]
    public Godot.Collections.Dictionary<string, string[]> Requirements { get; set; } = new Godot.Collections.Dictionary<string, string[]>();

    [Export]
    public bool Enabled { get; set; } = true;

    [Export]
    public Category Type { get; set; }

    public enum Category
    {
        Mine,
        Storage,
        Production,
        Construction,
        Recruitment,
        Research,
        Growth
    }

    public override void _Ready()
    {
        
    }
}
