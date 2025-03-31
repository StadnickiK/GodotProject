using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

public partial class Data : Node
{

    public override void _Ready()
    {
        GetNodes();
        LoadBuildingResources();
        LoadUnitResources();
    }

    void GetNodes(){
        GetResources();
        GetBuildings();
        GetUnits();
    }

    public List<Resource> Resources { get; set; } = new List<Resource>();

    void GetResources(){
        var arr = GetData("Resources");
        for(int i = 0; i < arr.Count; i++){
            var resource = (Resource)arr[i];
            if(!Resources.Contains(resource)){
                resource.Index = i;
                Resources.Add(resource);
            }
        }
    }

    public Dictionary<int,int> GetResourcesIndexList(){
        var resources = new Dictionary<int,int>();
		foreach (var res in Resources)
		{
			resources.Add(res.Index, 0);
		}
        return resources;
    }

    public Dictionary<int,int> GetStartResources(){
        var resources = new Dictionary<int,int>();
		foreach (var res in Resources)
		{
			resources.Add(res.Index, 0);
		}
        return resources;
    }

    public List<Building> Buildings { get; set; } = new List<Building>();

    void GetBuildings(){
        var arr = GetData("Buildings");
        foreach(Building resource in arr){
            Buildings.Add(resource);
            resource.Index = Buildings.Count-1;
        }
    }

    void LoadBuildingResources(){
        System.Threading.Tasks.Parallel.ForEach (Buildings, building => {
            building.ResourceLimits = LoadResourceCost(building.ExportResourceLimits);
            building.ProductCost = LoadResourceCost(building.ExportProductCost);
            building.Products = LoadResourceCost(building.ExportProducts);
            building.BuildCost = LoadResourceCost(building.ExportBuildCost);
            building.Upkeep = LoadResourceCost(building.ExportUpkeep);
        });
    }

    void LoadUnitResources(){
        System.Threading.Tasks.Parallel.ForEach (Units, unit => {
            unit.BuildCost = LoadResourceCost(unit.ExportBuildCost);
            unit.Upkeep = LoadResourceCost(unit.ExportUpkeep);
        });
    }

    public Unit GetUnit(int id){
        var unit = (Unit)Units[id].Duplicate();
        unit.UnitName = Units[id].UnitName;
        unit.BuildCost = Units[id].BuildCost;
        unit.Upkeep = Units[id].Upkeep;

        return unit;
    }

    Dictionary<int, int> LoadResourceCost(Godot.Collections.Dictionary<string, int> exportResourceCost){
        var cost = new Dictionary<int, int>();
        string resName = "";
        try
        {
            foreach(var resource in exportResourceCost){
                resName = resource.Key;
                cost.Add(Resources.FirstOrDefault(x => x.ResourceName == resName).Index ,resource.Value);
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Erro searching for given resource name "+resName+"\n"+ex.Message);
            throw;
        }
        

        return cost;
    }

    public List<Technology> Technologies { get; set; } = new List<Technology>();

    void GetTechnologies(){
        var arr = GetData("Technologies");
        foreach(Technology resource in arr){
            Technologies.Add(resource);
        }
    }

    public List<Unit> Units { get; set; } = new List<Unit>();

    void GetUnits(){
        var arr = GetData("Units");
        foreach(Unit unit in arr){
            unit.UnitName = unit.Name;
            Units.Add(unit);
        }
    }

    /// <summary>
    /// GetNode(nodeName).GetChildren();
    /// </summary>
    /// <param name="nodeName"></param>
    /// <returns></returns>
    public Godot.Collections.Array<Node> GetData(string nodeName){
        return GetNode(nodeName).GetChildren();
    }

    public List<Building> GetBuildingsList(){
        var array = GetData("Buildings");
        var list = new List<Building>();
        //if( array != null)
        foreach(var node in array){
            if(node is Building building){
                list.Add(building);
            }
        }
        return list;
    }

    public Node GetData(string dataName, string name){
        return GetNode(dataName + "/" + name);
    }

    public List<Building> GetBuildingsByRequiredTech(string[] requiredTech){
        List<Building> list = new List<Building>();
        foreach(var node in GetData("Buildings")){
            if(node is Building building){
                // if(building.Requirements.ContainsKey("Technology"))
                //     if(building.Requirements["Technology"].All(elem => requiredTech.Contains(elem))) list.Add(building);
            }
        }
        return list;   
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
