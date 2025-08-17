using Godot;
using System;
using System.Collections.Generic;

public partial class SelectManager<T> : Node
{
    public T MainSelectedUnit { get; set; }
    public HashSet<T> SelectedUnits = new HashSet<T>();

    public bool HasSelect { get => SelectedUnits.Count > 0;  }

    public override void _Ready()
    {
        SetProcess(false);
    }

    public void SelectUnit(T unit)
    {
        MainSelectedUnit = unit;
        SelectedUnits.Clear();
        AddSelectedUnit(unit);
    }

    public void AddSelectedUnit(T unit)
    {
        SelectedUnits.Add(unit);
    }

    public void AddSelectedUnits(List<T> units)
    {
        SelectedUnits.UnionWith(units);
    }

    public void ClearSelection()
    {
        MainSelectedUnit = default(T);
        SelectedUnits.Clear();
    }

    public void DeselectUnit(T unit)
    {
        SelectedUnits.Remove(unit);
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