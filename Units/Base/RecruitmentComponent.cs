using Godot;
using System;
using System.Collections.Generic;

public partial class RecruitmentComponent : Node
{
    public delegate void RecruitmentComponentEventHandler(List<int> Units);

    public event RecruitmentComponentEventHandler UpdateAvaialableUnitsEvent;

    public event RecruitmentComponentEventHandler RemoveAvaialableUnitsEvent;

    public event RecruitmentComponentEventHandler UpdateRecruitedUnitsEvent;

    public event RecruitmentComponentEventHandler RemoveRecruitedUnitsEvent;
    
    public List<int> AvaialableUnits { get; set; } = new List<int>();

    public RecruitmentManager RecruitmentManager { get; set; }

    public List<int> RecruitedUnits { get; set; } = new List<int>();

    public void AddAvaialableUnits(List<int> Units){
        AvaialableUnits.AddRange(Units);
        UpdateAvaialableUnitsEvent?.Invoke(Units);
    }

    public void RemoveAvaialableUnits(List<int> Units){
        foreach(int UnitsToRemove in Units){
            AvaialableUnits.Remove(UnitsToRemove);
        }
        RemoveAvaialableUnitsEvent?.Invoke(Units);
    }

    public void AddRecruitedUnits(List<int> Units){
        RecruitedUnits.AddRange(Units);
        UpdateAvaialableUnitsEvent?.Invoke(Units);
    }

    public void RemoveRecruitedUnitsAt(List<int> Units){
        foreach(int pos in Units){
            RecruitedUnits.RemoveAt(pos);
        }
        RemoveAvaialableUnitsEvent?.Invoke(Units);
    }
}
