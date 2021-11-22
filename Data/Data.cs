using Godot;
using System;
using System.Collections.Generic;

public class Data : Node
{

    private UnitsLoader _UnitsLoader = null;
    public UnitsLoader UnitsLoader
    {
        get { return _UnitsLoader; }
    }
    

    public Godot.Collections.Dictionary<string, Resource> WorldResources { get; } = new Godot.Collections.Dictionary<string, Resource>();

    // public Godot.Collections.Dictionary<string, Unit> WorldUnits { get; } = new Godot.Collections.Dictionary<string, Unit>();

    public List<Unit> WorldUnits { get; set; } = new List<Unit>();
    
    public List<Building> WorldBuildings { get; set; } = new List<Building>();

    public override void _Ready()
    {
        GetNodes();
        WorldUnits = _UnitsLoader.WorldUnits;
    }

    void GetNodes(){
        _UnitsLoader = GetNode<UnitsLoader>("UnitsLoader");
        GetChildren();
    }

    public Godot.Collections.Array GetData(string nodeName){
        return GetNode(nodeName).GetChildren();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
