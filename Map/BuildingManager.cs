using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuildingManager : Node
{

    float _time = 0;

    public bool BuildingsChanged { get; set; } = false;

    public bool ConstructionListChanged { get; set; } = false;

    // TargetManager<Building> ConstructionList = new TargetManager<Building>();

    ConstructionManager _constructions = new ConstructionManager();
    public ConstructionManager Constructions
    {
        get { return _constructions; }
    }
    

    // public Building CurrentConstruction
    // {
    //     get { return _constructions.CurrentConstruction(); }
    // }

    public List<Building> CurrentConstruction(){
        var currentConstruction = _constructions.CurrentConstruction();
        var list = IbuildingToBuilding(currentConstruction);
        return list;
    }

    List<Building> IbuildingToBuilding(List<IBuilding> originalList){
        var list = new List<Building>();
        foreach(IBuilding building in originalList){
            list.Add((Building)building);
        }
        return list;
    }

    public void ConstructBuilding(Building building){
        if(building != null){
            _constructions.ConstructionList.Add(building);
            // ConstructionListChanged = true;
        }
    }

    public List<Building> Buildings { get; } = new List<Building>();
    public override void _Ready()
    {
        
    }

    // void UpdateConstruction(){
    //     if(CurrentConstruction != null){
    //         CurrentConstruction.CurrentTime++;
    //         if(CurrentConstruction.CurrentTime >= CurrentConstruction.BuildTime){
    //             Buildings.Add(CurrentConstruction);
    //             ConstructionList.NextTarget();
    //             ConstructionListChanged = true;
    //         }
    //         BuildingsChanged = true;
    //     }
    // }

    void UpdateConstruction(){
        if(_constructions.ConstructionList.Count > 0){
            var list = _constructions.UpdateConstruction();
            ConstructionListChanged = true;
            if(list.Count>0){
                Buildings.AddRange(IbuildingToBuilding(list));
                BuildingsChanged = true;
            }
        }
    }

    public Building GetLastBuilding(){
        return Buildings.LastOrDefault();
    }

    public bool HasBuilding(Building building){
        if(Buildings.Contains(building)) 
            return true;
        return false;
    }

    public override void _Process(float delta){
        _time += delta;
        if(_time >= 1){
            UpdateConstruction();
            _time = 0;
        }
    }

}
