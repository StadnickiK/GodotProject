using Godot;
using System;
using System.Collections.Generic;

public class SelectManager{
    public KinematicCube MainSelectedUnit { get; set; }
    public List<KinematicCube> SelectedUnits = new List<KinematicCube>();

    public void SelectUnit(KinematicCube unit){
        MainSelectedUnit = unit;
        SelectedUnits.Clear();
        AddSelectedUnit(unit);
        GD.Print(unit.GetIndex());
    }

    public void AddSelectedUnit(KinematicCube unit){
        SelectedUnits.Add(unit);
    }

    public void AddSelectedUnits(List<KinematicCube> units){
        SelectedUnits = units;
    }

    public void ClearSelection(){
        MainSelectedUnit = null;
        SelectedUnits.Clear();
    }

    public void AddTarget(KinematicCube target){
            foreach(KinematicCube c in SelectedUnits){
                if(c != target){
                    c.targetManager.AddTarget(target);
                }
            }
        }
}
