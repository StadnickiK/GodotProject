using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuildingManager : Node
{

    float _time = 0;

    public bool BuildingsChanged { get; set; } = false;

    public bool ConstructionListChanged { get; set; } = false;

    TargetManager<IBuilding> ConstructionList = new TargetManager<IBuilding>();

    public IBuilding CurrentConstruction
    {
        get { return ConstructionList.currentTarget; }
    }

    public void ConstructBuilding(IBuilding building){
        if(building != null){
            ConstructionList.AddTarget(building);
            // ConstructionListChanged = true;
        }
    }

    public List<IBuilding> Buildings { get; } = new List<IBuilding>();
    public override void _Ready()
    {
        
    }

    void UpdateConstruction(){
        if(CurrentConstruction != null){
            CurrentConstruction.CurrentTime++;
            if(CurrentConstruction.CurrentTime >= CurrentConstruction.BuildTime){
                Buildings.Add(CurrentConstruction);
                ConstructionList.NextTarget();
                ConstructionListChanged = true;
            }
            BuildingsChanged = true;
        }
    }

    public IBuilding GetLastBuilding(){
        return Buildings.LastOrDefault();
    }

    public override void _Process(float delta){
        _time += delta;
        if(_time >= 1){
            UpdateConstruction();
            _time = 0;
        }
    }

}
