using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;



public interface IBuildingManager
{
    public BuildingManager BuildingManager { get; }
}

public partial class BuildingManager : Node, IChangeControllerListener
{
    [Export]
    public bool Active { get; set; } = true;

    double _time = 0;

    public delegate void BuildingFinishedEventHandler(List<Building> building);

    public event BuildingFinishedEventHandler BuildingFinished; // subscribe here to event like BuildingManager.BuildingFinished += MyMethod, unsubscribe with -=

    public event BuildingFinishedEventHandler ConstructionListChanged;

    ConstructionManager _constructions = new ConstructionManager();
    public ConstructionManager Constructions
    {
        get { return _constructions; }
    }

    public List<Building> LastBuilding { get; set; } = new List<Building>();

    public List<Building> Buildings { get; } = new List<Building>();

    public HashSet<Building> AvaiableBuildings { get; } = new HashSet<Building>();

    public List<int> CanPay { get; set; } = new List<int>();

    public List<int> CantPay { get; set; } = new List<int>();

    [Signal]
    public delegate void GameAlertEventHandler(World.GameAlert alert);

    public Player Controller { get; set; }

    public bool HasConstruction => Constructions.CurrentConstruction().Count > 0;



    public override void _Ready()
    {
        AddChild(_constructions);
        EndTurnEmitter.Instance.EndTurn += _on_EndTurn;
    }

    public List<Building> CurrentConstruction()
    {
        var currentConstruction = _constructions.CurrentConstruction();
        var Array = IbuildingToBuilding(currentConstruction);
        return Array;
    }

    public void UpdateCanPayBuildings(ResourceManager resourceManager)
    {
        CanPay.Clear();
        CantPay.Clear();
        foreach (var building in AvaiableBuildings)
        {
            if (resourceManager.CanPayCost(building.BuildCost))
            {
                CanPay.Add(building.Index);
            }
            else
            {
                CantPay.Add(building.Index);
            }
        }
    }

    List<Building> IbuildingToBuilding(List<IConstruct> originalArray)
    {
        var Array = new List<Building>();
        foreach (IConstruct building in originalArray)
        {
            if (building is Building)
                Array.Add((Building)building);
        }
        return Array;
    }

    public void ConstructBuilding(Building building)
    {
        if (building != null)
        {
            _constructions.ConstructionList.Add(building);
            AvaiableBuildings.Remove(building);
        }
    }

    public void StopConstruction(Player player, Building building)
    {
        _constructions.ConstructionList.Remove(building);
        player.ResManager.AddResource(building.BuildCost);
        AvaiableBuildings.Add(building);
    }

    public void StopConstruction(Player player, int Position)
    {
        var c = _constructions.ConstructionList[Position];
        _constructions.ConstructionList.RemoveAt(Position);
        player.ResManager.AddResource(c.BuildCost);
        if (c is Building building)
            AvaiableBuildings.Add(building);
    }

    public bool StartBuilding(Building building)
    {
        if (building != null)
            if (!HasBuildingOrConstruct(building)) // check for duplicates
                if (Controller.ResManager.PayCost(building.BuildCost))
                {
                    ConstructBuilding(building);
                    return true;
                }
                else
                {
                    EmitSignal(nameof(GameAlertEventHandler), this);
                }
        return false;
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

    public void UpdateAvaiableBuildings(Building building)
    {
        AvaiableBuildings.Add(building);
    }

    public void UpdateAvaiableBuildings(List<Building> buildings)
    {
        AvaiableBuildings.UnionWith(buildings);
    }

    public void _on_EndTurn(int TurnNumber)
    {
        UpdateConstruction();
    }

    void UpdateConstruction()
    {
        if (_constructions.ConstructionList.Count > 0)
        {
            LastBuilding = IbuildingToBuilding(_constructions.UpdateConstruction());
            if (LastBuilding.Count > 0)
            {
                BuildingFinished?.Invoke(LastBuilding); // call BuildingFinished event with finioshed building list
                Buildings.AddRange(LastBuilding);
                AvaiableBuildings.RemoveWhere(x => LastBuilding.Contains(x));
            }
        }
    }

    public void AddBuildings(List<Building> buildings)
    {
        if (buildings.Count > 0)
        {
            BuildingFinished?.Invoke(buildings); // call BuildingFinished event with finioshed building list
            Buildings.AddRange(buildings);
            AvaiableBuildings.RemoveWhere(buildings.Contains);
        }
    }

    public Building GetLastBuilding()
    {
        return Buildings.LastOrDefault();
    }

    public bool HasBuilding(Building building)
    {
        return Buildings.Contains(building);
    }

    public bool HasBuildingOrConstruct(Building building)
    {
        return Buildings.Contains(building) || Constructions.ConstructionList.Contains(building);
    }

    public bool HasBuilding(string name)
    {
        return Buildings.FirstOrDefault(x => x.Name == name) != null;
    }

    public void ChangeContoller(Player player)
    {
        Controller = player;
    }
}
