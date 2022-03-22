using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Data : Node
{

    public override void _Ready()
    {
        GetNodes();
    }

    void GetNodes(){
    }

    /// <summary>
    /// GetNode(nodeName).GetChildren();
    /// </summary>
    /// <param name="nodeName"></param>
    /// <returns></returns>
    public Godot.Collections.Array GetData(string nodeName){
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
            if(node is Building building)
                if(building.Requirements.ContainsKey("Technology"))
                    if(building.Requirements["Technology"].All(elem => requiredTech.Contains(elem))) list.Add(building);
        }
        return list;   
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
