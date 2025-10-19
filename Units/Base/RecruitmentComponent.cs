using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IRecruitmentComponent
{
    public RecruitmentComponent RecruitmentComponent { get; set; }
}

public partial class RecruitmentComponent : Node, IChangeControllerListener, RecruitmentFinishedListener
{
    public delegate void RecruitmentComponentEventHandler();

    public event RecruitmentComponentEventHandler UpdateAvaialableUnitsEvent;

    public event RecruitmentComponentEventHandler RemoveAvaialableUnitsEvent;

    public delegate void CurrentlyRecruitedUnitsEventHandler(RecruitmentComponent recruitmentComponent);

    public event CurrentlyRecruitedUnitsEventHandler UpdateCurrentlyRecruitedUnitsEvent;

    public event CurrentlyRecruitedUnitsEventHandler RemoveCurrentlyRecruitedUnitsEvent;

    public Dictionary<RecruitmentManager, List<Unit>> AvaialableUnitsMap { get; } = new Dictionary<RecruitmentManager, List<Unit>>();

    public List<Unit> CurrentlyRecruitedUnits { get; set; } = new List<Unit>();
    public Player Controller { get; set; }

    public UnitController UnitController { get; set; }

    void ControllerChanged(Player player)
    {
        Controller = player;
    }

    public void SetAvaialableUnits(RecruitmentManager recruitmentManager, List<Unit> Units)
    {
        if (AvaialableUnitsMap.ContainsKey(recruitmentManager))
        {
            AvaialableUnitsMap[recruitmentManager] = Units;
        }
        else
        {
            AvaialableUnitsMap.Add(recruitmentManager, Units);
            recruitmentManager.RecruitmentFinished -= RecruitmentFinished;
            recruitmentManager.RecruitmentFinished += RecruitmentFinished;
        }
        UpdateAvaialableUnitsEvent?.Invoke();
        
    }

    public void RemoveAvaialableUnits(RecruitmentManager recruitmentManager)
    {
        AvaialableUnitsMap.Remove(recruitmentManager);
        recruitmentManager.RecruitmentFinished -= RecruitmentFinished;
        RemoveAvaialableUnitsEvent?.Invoke();
    }

    public void AddCurrentlyRecruitedUnits(Unit Units)
    {
        CurrentlyRecruitedUnits.Add(Units);
        UpdateCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }

    public void RemoveCurrentlyRecruitedUnits(Unit Units)
    {
        CurrentlyRecruitedUnits.Remove(Units);
        UpdateCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }

    public void RemoveCurrentlyRecruitedUnitsAt(List<int> UnitsPositions)
    {
        foreach (int pos in UnitsPositions)
        {
            CurrentlyRecruitedUnits.RemoveAt(pos);
        }
        RemoveCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }

    public bool StartConstruction(RecruitmentManager recruitmentManager, Unit unit)
    {
        var unitDuplicate = (Unit)unit.Duplicate();
        AddChild(unitDuplicate);
        if (recruitmentManager.StartConstruction(unitDuplicate))
        {
            AddCurrentlyRecruitedUnits(unitDuplicate);
            return true;
        }
        unitDuplicate.QueueFree();
        return false;   
    }

    public void StopConstruction(int Position)
    {
        AvaialableUnitsMap.Keys.ElementAt(0).StopConstruction(Position);
        CurrentlyRecruitedUnits.RemoveAt(Position);
        UpdateCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }

    public void ChangeContoller(Player player)
    {
        Controller = player;
    }

    public void RecruitmentFinished(RecruitmentManager recruitmentManager, List<IConstruct> constructs)
    {
        foreach (var item in constructs)
            if (item is Unit unit && CurrentlyRecruitedUnits.Contains(unit))
            {
                UnitController.AddUnit(unit);
                RemoveCurrentlyRecruitedUnits(unit);
            }
        
    }
}
