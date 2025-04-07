using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class Building : Node, IBuilding, IUpkeep
{
    // public List<Resource> Resources { get; set; } = new List<Resource>();

    //[Export]
    new public string Name { get; set; }

    public int Index { get; set; }

    [Export]
    public bool IsStarter { get; set; } = false;

    [Export]
    public int BuildTime { get; set; } = 5;
    
    public int CurrentTime { get; set; } = 0;

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportProducts { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public System.Collections.Generic.Dictionary<int, int> Products { get; set; } = new System.Collections.Generic.Dictionary<int, int>();
    [Export]
    public Godot.Collections.Dictionary<string, int> ExportBuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public System.Collections.Generic.Dictionary<int, int> BuildCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportProductCost { get; set; } = new Godot.Collections.Dictionary<string, int>();
    public System.Collections.Generic.Dictionary<int, int> ProductCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    // public bool HasProductCost { get; set; } = false;
    [Export]
    public Godot.Collections.Dictionary<string, int> ExportResourceLimits { get; set; } = new Godot.Collections.Dictionary<string, int>();
    public System.Collections.Generic.Dictionary<int, int> ResourceLimits { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportUpkeep { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public System.Collections.Generic.Dictionary<int, int> Upkeep { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportOperationCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

     public List<Resource> OperationCost { get; set; } = new List<Resource>();

    [Export]
    public Godot.Collections.Dictionary<string, Array<string>> ExportRequirements { get; set; } = new Godot.Collections.Dictionary<string, Array<string>>();

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
        Name = base.Name;
    }

    public static System.Collections.Generic.Dictionary<int, int> GetBuildingsProduction(System.Collections.Generic.List<Building> buildings){
        var production = new System.Collections.Generic.Dictionary<int, int>();
        foreach (var building in buildings){
            foreach(var res in building.Products)
            if(production.ContainsKey(res.Key)){
                production[res.Key] = res.Value;
            }else{
                production.Add(res.Key, res.Value);
            }
        }
        return production;
    }

    public static System.Collections.Generic.Dictionary<int, int> GetBuildingsProductionCost(System.Collections.Generic.List<Building> buildings){
        var production = new System.Collections.Generic.Dictionary<int, int>();
        foreach (var building in buildings){
            foreach(var res in building.ProductCost)
                if(production.ContainsKey(res.Key)){
                    production[res.Key] = res.Value;
                }else{
                    production.Add(res.Key, res.Value);
                }
        }
        return production;
    }
}
