using Godot;
using System;
using System.Collections.Generic;

public class BuildingManager : Node
{

    float _time = 0;

    public bool BuildingsChanged { get; set; } = false;

    public bool ConstructionListChanged { get; set; } = false;

    TargetManager<Building> ConstructionList = new TargetManager<Building>();

    public Building CurrentConstruction
    {
        get { return ConstructionList.currentTarget; }
    }

    public void ConstructBuilding(Building building){
        if(building != null){
            //to do paycost
            ConstructionList.AddTarget(building);
            ConstructionListChanged = true;
        }
    }

    public List<Building> Buildings { get; } = new List<Building>();
    public override void _Ready()
    {
        
    }

    void UpdateConstruction(){
            CurrentConstruction.CurrentTime++;
            if(CurrentConstruction.CurrentTime >= CurrentConstruction.BuildTime){
                Buildings.Add(CurrentConstruction);
                ConstructionList.NextTarget();
                BuildingsChanged = true;
            }
    }

    public override void _Process(float delta){
        _time += delta;
        if(_time >= 1){
            UpdateConstruction();
            _time = 0;
        }
    }

}
