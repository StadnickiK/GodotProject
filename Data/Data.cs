using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Data : Node
{

    public override void _Ready()
    {
        GetNodes();
    }

    void GetNodes(){
        GetResources();
        GetBuildings();
    }

    public Dictionary<int, Resource> Resources { get; set; } = new Dictionary<int, Resource>();

    void GetResources(){
        var arr = GetData("Resources");
        foreach(Resource resource in arr){
            if(!Resources.ContainsKey(resource.Index)){}
                Resources.Add(resource.Index, resource);
        }
    }

    public List<Building> Buildings { get; set; } = new List<Building>();

    void GetBuildings(){
        var arr = GetData("Buildings");
        foreach(Building resource in arr){
            Buildings.Add(resource);
        }
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
        foreach(Unit resource in arr){
            Units.Add(resource);
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
