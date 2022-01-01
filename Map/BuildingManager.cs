using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuildingManager : Node
{

    float _time = 0;

    public bool BuildingsChanged { get; set; } = false; // changed when build is finished

    public bool ConstructionListChanged { get; set; } = false; // changed when building current build time changes

    ConstructionManager _constructions = new ConstructionManager();
    public ConstructionManager Constructions
    {
        get { return _constructions; }
    }

    public List<Building> LastBuilding { get; set; } = new List<Building>();

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
        AddChild(_constructions);
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
            LastBuilding = IbuildingToBuilding(_constructions.UpdateConstruction());
            ConstructionListChanged = true;
            if(LastBuilding.Count>0){
                Buildings.AddRange(LastBuilding);
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
