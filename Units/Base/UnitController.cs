using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UnitController : Node
{
    public List<Unit> UnitsList { get; } = new List<Unit>();

    public UpkeepComponent UpkeepComponent { get; set; }

    public delegate void ChangeUpkeepEventHandler(Dictionary<int, int> upkeep);

    public event ChangeUpkeepEventHandler AddUpkeep;

    public event ChangeUpkeepEventHandler RemoveUpkeep;

    public event World.SplitShipEventHandler SplitShip;

    public event World.FreeShipEventHandler FreeShip;

    public event Node2DPool.FreeNodeEventHandler FreeUnit;

    public delegate void NoUnitsEventHandler();

    public event NoUnitsEventHandler NoUnits;

    [Export]
    public int MaxUnits { get; set; } = 20;

    public int Count { get { return UnitsList.Count; } }
    
    public bool HasUnits { get { return UnitsList.Count > 0; } }

	public override void _Ready()
    {
        UpkeepComponent = GetNodeOrNull<UpkeepComponent>("UpkeepComponent");
    }

    public void AddUnit(Unit unit){
        AddChild(unit);
        UnitsList.Add(unit);
        UpkeepComponent?.UpdateUpkeep(unit);
        AddUpkeep?.Invoke(unit.Upkeep);
    }

    public void AddUnit(List<Unit> units){
        foreach(Unit unit in units)
            AddUnit(unit);
    }

    public void RemoveUnit(int unitID){
        UpkeepComponent?.RemoveUpkeep(UnitsList[unitID]);
        RemoveUpkeep?.Invoke(UnitsList[unitID].Upkeep);
        RemoveChild(UnitsList[unitID]);
        UnitsList.RemoveAt(unitID);
    }

    public void RemoveUnit(Unit unit){
        UpkeepComponent?.RemoveUpkeep(unit);
        RemoveUpkeep?.Invoke(unit.Upkeep);
        RemoveChild(unit);
        UnitsList.Remove(unit);
    }

    public void RemoveUnit(){
        for (int i = Count - 1; i >= 0 ; i--)
        {
            UpkeepComponent?.RemoveUpkeep(UnitsList[i]);
            RemoveUpkeep?.Invoke(UnitsList[i].Upkeep);
            UnitsList[i].QueueFree();
            UnitsList.RemoveAt(i);
        }
    }

    public void ClearUnits()
    {
        for (int i = UnitsList.Count - 1; i >= 0; i--)
        {
            if (!UnitsList[i].HasHitpoints)
            {
                FreeUnit?.Invoke(UnitsList[i]);
                RemoveUnit(UnitsList[i]);
            }
        }
        CheckNoUnits();
    }

    void CheckNoUnits()
    {
        if (!HasUnits) NoUnits?.Invoke();
    }

    public void TransferUnit(UnitController unitController, int unitID)
    {
        unitController.AddUnit(UnitsList[unitID]);
        RemoveUnit(unitID);
    }

    public void TransferUnit(UnitController unitController, List<int> unitIDs)
    {
        var units = UnitsList.Where(x => unitIDs.Contains(x.GetIndex())).ToArray();
        for (int i = 0; i < unitIDs.Count; i++)
        {
            TransferUnit(unitController, unitIDs[i]);
        }
    }

    public void TransferUnit(UnitController unitController){
        foreach(Unit unit in UnitsList){
            UpkeepComponent?.RemoveUpkeep(unit);
            RemoveUpkeep?.Invoke(unit.Upkeep);
            RemoveChild(unit);
            unitController.AddUnit(unit);
        }
        UnitsList.Clear();
    }

    public List<Unit> GetUnitsForTransfer(List<int> unitIDs){
        var list = new List<Unit>();
        for (int i = unitIDs.Count - 1; i >= 0 ; i--)
        {
            list.Add(UnitsList[i]);
            RemoveUnit(i);
        }
        return list;
    }

    public void RemoveUnits(List<Unit> units){
        for (int i = units.Count - 1; i >= 0; i--)
        {
            RemoveUnit(units[i]);
        }
    }
    
    void Merge(Ship ship){
            if(Count + ship.UnitController.Count < ship.UnitController.MaxUnits){
                TransferUnit(ship.UnitController);
                FreeShip?.Invoke(ship);
            }
    }

}
