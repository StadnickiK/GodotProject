using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class Building : Node, IBuilding, IUpkeep
{
    // public List<Resource> Resources { get; set; } = new List<Resource>();

    [Export]
    public new string Name { get; set; }

    [Export]
    public bool IsStarter { get; set; } = false;

    [Export]
    public int BuildTime { get; set; } = 5;
    
    public int CurrentTime { get; set; } = 0;

    [Export]
    Godot.Collections.Dictionary<int, int> ExportProducts { get; set; } = new Godot.Collections.Dictionary<int, int>();

    public System.Collections.Generic.Dictionary<int, int> Products { get; set; } = new System.Collections.Generic.Dictionary<int, int>();
    [Export]
     Godot.Collections.Dictionary<int, int> ExportBuildCost { get; set; } = new Godot.Collections.Dictionary<int, int>();

    public System.Collections.Generic.Dictionary<int, int> BuildCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
     Godot.Collections.Dictionary<int, int> ExportProductCost { get; set; } = new Godot.Collections.Dictionary<int, int>();
    public System.Collections.Generic.Dictionary<int, int> ProductCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    // public bool HasProductCost { get; set; } = false;
    [Export]
     Godot.Collections.Dictionary<int, int> ExportResourceLimits { get; set; } = new Godot.Collections.Dictionary<int, int>();
    public System.Collections.Generic.Dictionary<int, int> ResourceLimits { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
     Godot.Collections.Dictionary<int, int> ExportUpkeep { get; set; } = new Godot.Collections.Dictionary<int, int>();

    public System.Collections.Generic.Dictionary<int, int> Upkeep { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
     Godot.Collections.Dictionary<int, int> ExportOperationCost { get; set; } = new Godot.Collections.Dictionary<int, int>();

     public List<Resource> OperationCost { get; set; } = new List<Resource>();

    [Export]
     Godot.Collections.Dictionary<int, Array<int>> ExportRequirements { get; set; } = new Godot.Collections.Dictionary<int, Array<int>>();

     public System.Collections.Generic.Dictionary<int, List<int>> Requirements { get; set; } = new System.Collections.Generic.Dictionary<int, List<int>>();

    [Export]
     public bool Enabled { get; set; } = true;

    [Export]
    public Category Type { get; set; }

    public enum Category
    {
        Mine,
        Production,
        Construction,
        Recruitment,
        Research,
        Growth,
        Storage
    }

    public override void _Ready()
    {

    }
}
