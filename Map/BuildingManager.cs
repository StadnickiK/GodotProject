using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class BuildingManager : Node
{

    double _time = 0;

    public bool BuildingsChanged { get; set; } = false; // changed when build is finished

    public delegate void BuildingFinishedEventHandler(List<Building> building);

    public event BuildingFinishedEventHandler BuildingFinished; // subscribe here to event like BuildingManager.BuildingFinished += MyMethod, unsubscribe with -=

    public bool ConstructionListChanged { get; set; } = false; // changed when building current build time changes

    ConstructionManager _constructions = new ConstructionManager();
    public ConstructionManager Constructions
    {
        get { return _constructions; }
    }

    public List<Building> LastBuilding { get; set; } = new List<Building>();

    public List<Building> CurrentConstruction(){
        var currentConstruction = _constructions.CurrentConstruction();
        var Array = IbuildingToBuilding(currentConstruction);
        return Array;
    }

    List<Building> IbuildingToBuilding(List<IBuilding> originalArray){
        var Array = new List<Building>();
        foreach(IBuilding building in originalArray){
            Array.Add((Building)building);
        }
        return Array;
    }

    public void ConstructBuilding(Building building){
        if(building != null){
            _constructions.ConstructionList.Add(building);
            // ConstructionArrayChanged = true;
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
    //             ConstructionArray.NextTarget();
    //             ConstructionArrayChanged = true;
    //         }
    //         BuildingsChanged = true;
    //     }
    // }

    void UpdateConstruction(){
        if(_constructions.ConstructionList.Count > 0){
            LastBuilding = IbuildingToBuilding(_constructions.UpdateConstruction());
            ConstructionListChanged = true;
            BuildingFinished?.Invoke(LastBuilding); // call BuildingFinished event with finioshed building list
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

    public bool HasBuilding(string name){
        return (Buildings.FirstOrDefault(x => x.Name == name) != null);
    }

    public override void _Process(double delta){
        _time += delta;
        if(_time >= 1){
            UpdateConstruction();
            _time = 0;
        }
    }

}
