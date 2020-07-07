using Godot;
using System;
using System.Collections.Generic;

public class SelectManager<T>{
    public T MainSelectedUnit { get; set; }
    public List<T> SelectedUnits = new List<T>();

    public void SelectUnit(T unit){
        MainSelectedUnit = unit;
        SelectedUnits.Clear();
        AddSelectedUnit(unit);
    }

    public void AddSelectedUnit(T unit){
        SelectedUnits.Add(unit);
    }

    public void AddSelectedUnits(List<T> units){
        SelectedUnits = units;
    }

    public void ClearSelection(){
        MainSelectedUnit = default(T);
        SelectedUnits.Clear();
    }

    public void AddTarget(T target)
    {
            foreach(T c in SelectedUnits){
                if(!EqualityComparer<T>.Default.Equals(c, target)){
                    // c.targetManager.AddTarget(target);
                }
            }
    }
}
