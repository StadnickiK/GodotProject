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

    public Godot.Collections.Array GetData(string nodeName){
        return GetNode(nodeName).GetChildren();
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
