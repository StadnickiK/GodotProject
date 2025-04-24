using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RecruitmentComponent : Node, IMapObjectController
{
    public delegate void RecruitmentComponentEventHandler(RecruitmentManager recruitmentManager,List<int> Units);

    public event RecruitmentComponentEventHandler UpdateAvaialableUnitsEvent;

    public event RecruitmentComponentEventHandler RemoveAvaialableUnitsEvent;

     public delegate void CurrentlyRecruitedUnitsEventHandler(RecruitmentComponent recruitmentComponent);

    public event CurrentlyRecruitedUnitsEventHandler UpdateCurrentlyRecruitedUnitsEvent;

    public event CurrentlyRecruitedUnitsEventHandler RemoveCurrentlyRecruitedUnitsEvent;

    public Dictionary<RecruitmentManager, List<int>> AvaialableUnitsMap { get;} = new Dictionary<RecruitmentManager, List<int>>();

    public List<int> CurrentlyRecruitedUnits { get; set; } = new List<int>();
    public Player Controller { get; set; }

    public override void _Ready()
    {
        var parent = GetParent();
        GetController(parent);
    }

    void GetController(Node parent){
        if(parent is IMapObjectController controller)
            Controller = controller.Controller;
    }

    public void AddAvaialableUnits(RecruitmentManager recruitmentManager, List<int> Units){
        AvaialableUnitsMap[recruitmentManager].AddRange(Units);
        UpdateAvaialableUnitsEvent?.Invoke(recruitmentManager, Units);
    }

    public void SetAvaialableUnits(RecruitmentManager recruitmentManager, List<int> Units){
        if(AvaialableUnitsMap.ContainsKey(recruitmentManager)){
            AvaialableUnitsMap[recruitmentManager] = Units;
        }else{
            AvaialableUnitsMap.Add(recruitmentManager, Units);
        }
        UpdateAvaialableUnitsEvent?.Invoke(recruitmentManager, Units);
    }

    public void RemoveAvaialableUnits(RecruitmentManager recruitmentManager, List<int> Units){        
        AvaialableUnitsMap.Remove(recruitmentManager);        
        RemoveAvaialableUnitsEvent?.Invoke(recruitmentManager, Units);
    }

    public void AddCurrentlyRecruitedUnits(List<int> Units){
        CurrentlyRecruitedUnits.AddRange(Units);
        UpdateCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }

    public void RemoveCurrentlyRecruitedUnitsAt(List<int> UnitsPositions){
        foreach(int pos in UnitsPositions){
            CurrentlyRecruitedUnits.RemoveAt(pos);
        }
        RemoveCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }

    public bool StartConstruction(Unit unit){
        if(AvaialableUnitsMap.Keys.ElementAt(0).StartConstruction(Controller, unit)){
            AddCurrentlyRecruitedUnits(new List<int>() {unit.Index});
            return true;
        }else{
            return false;
        }
    }

    public void StopConstruction(int Position){
        AvaialableUnitsMap.Keys.ElementAt(0).StopConstruction(Controller, Position);
        CurrentlyRecruitedUnits.RemoveAt(Position);
        UpdateCurrentlyRecruitedUnitsEvent?.Invoke(this);
    }
}
