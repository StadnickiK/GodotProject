using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UnitController : Node3D
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

    public delegate void UnitAddedEventHandler(Unit unit);

    public event UnitAddedEventHandler UnitAdded;

    public delegate void UnitsChangedEventHandler(List<Unit> units);

    public event UnitsChangedEventHandler UnitsRemoved;

    public event UnitsChangedEventHandler UnitsToTransferChanged;

    [Export]
    public int MaxUnits { get; set; } = 20;

    public int Count { get { return UnitsList.Count; } }

    public bool HasUnits { get { return UnitsList.Count > 0; } }

    public List<Unit> UnitsToTransfer { get; private set; } = new List<Unit>();

    Node Parent;

    public override void _Ready()
    {
        UpkeepComponent = GetNodeOrNull<UpkeepComponent>("UpkeepComponent");
        Parent = GetParent();
        SplitShip -= World.Instance.SplitShip;
        SplitShip += World.Instance.SplitShip;
    }

    public void AddUnit(Unit unit)
    {
        unit.GetParent()?.RemoveChild(unit);
        AddChild(unit);
        UnitsList.Add(unit);
        UpkeepComponent?.UpdateUpkeep(unit);
        AddUpkeep?.Invoke(unit.Upkeep);
        UnitAdded?.Invoke(unit);
    }

    public void AddUnit(List<Unit> units)
    {
        foreach (Unit unit in units)
            AddUnit(unit);
    }

    public void RemoveUnit(int unitID)
    {
        UpkeepComponent?.RemoveUpkeep(UnitsList[unitID]);
        RemoveUpkeep?.Invoke(UnitsList[unitID].Upkeep);
        RemoveChild(UnitsList[unitID]);
        UnitsList.RemoveAt(unitID);
        UnitsRemoved?.Invoke(UnitsList);
    }

    public void RemoveUnit(Unit unit)
    {
        UpkeepComponent?.RemoveUpkeep(unit);
        RemoveUpkeep?.Invoke(unit.Upkeep);
        RemoveChild(unit);
        UnitsList.Remove(unit);
        UnitsRemoved?.Invoke(UnitsList);
    }

    public void RemoveUnit()
    {
        for (int i = Count - 1; i >= 0; i--)
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
        if (!HasUnits)
            NoUnits?.Invoke();
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

    public void TransferUnit(UnitController target)
    {
        foreach (Unit unit in UnitsList)
        {
            UpkeepComponent?.RemoveUpkeep(unit);
            RemoveUpkeep?.Invoke(unit.Upkeep);
            RemoveChild(unit);
            target.AddUnit(unit);
        }
        Clear();
    }

    public void Clear()
    {
        UnitsList.Clear();
        UnitsToTransfer.Clear();
        UnitsToTransferChanged?.Invoke(UnitsToTransfer);
    }

    public void TransferUnits(UnitController target, List<Unit> targetUnits)
    {
        RemoveUnits(UnitsToTransfer);
        target.AddUnit(UnitsToTransfer);
        target.RemoveUnits(targetUnits);
        UnitsToTransfer.Clear();
        UnitsToTransferChanged?.Invoke(UnitsToTransfer);
        AddUnit(targetUnits);
    }

    public void _on_UpdateUnitsToTransfer(List<Unit> unitsToTransfer)
    {
        UnitsToTransfer = unitsToTransfer;
        UnitsToTransferChanged?.Invoke(UnitsToTransfer);
    }

    public void RemoveUnits(List<Unit> units)
    {
        for (int i = units.Count - 1; i >= 0; i--)
        {
            RemoveUnit(units[i]);
        }
    }


    public void Split(OrderQueue.Target target, Player Controller){
        if(UnitsToTransfer.Count > 0 && UnitsToTransfer.Count < UnitsList.Count){
                RemoveUnits(UnitsToTransfer);
                SplitShip?.Invoke(new ShipModel(){
                    Target = target,
                    Name = Parent.Name,
                    Parent = Parent.GetParent(),
                    Controller = Controller,
                    Position = GlobalPosition,
                    Units = UnitsToTransfer,
                    Visible = this.Visible,
                });
            UnitsToTransfer.Clear();
            UnitsToTransferChanged?.Invoke(UnitsToTransfer);
        }
    }

}
