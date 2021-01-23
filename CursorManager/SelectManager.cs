using Godot;
using System;
using System.Collections.Generic;

public class SelectManager<T> : Node{
    public T MainSelectedUnit { get; set; }
    public List<T> SelectedUnits = new List<T>();

    public bool HasSelect { get; set; } = false;

    public override void _Ready()
    {    
        SetProcess(false);
    }

    public void SelectUnit(T unit){
        MainSelectedUnit = unit;
        SelectedUnits.Clear();
        AddSelectedUnit(unit);
        HasSelect = true;
    }

    public void AddSelectedUnit(T unit){
        SelectedUnits.Add(unit);
        HasSelect = true;
    }

    public void AddSelectedUnits(List<T> units){
        SelectedUnits = units;
        HasSelect = true;
    }

    public void ClearSelection(){
        MainSelectedUnit = default(T);
        SelectedUnits.Clear();
        HasSelect = false;
    }

}

/*
    public void AddTarget(T target)
    {
            foreach(T c in SelectedUnits){
                if(!EqualityComparer<T>.Default.Equals(c, target)){
                    // c.targetManager.AddTarget(target);
                }
            }
    }

*/