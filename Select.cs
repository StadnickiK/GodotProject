using Godot;
using System;
using System.Collections.Generic;

public class Select : Node
{

    SelectManager<RigidBody> selectManager;
    PackedScene SelectEffect = (PackedScene)ResourceLoader.Load("res://SelectEffect3.tscn");

    Vector3 _destination;

    public void MoveToPosition(Vector3 destination){
        if(selectManager.SelectedUnits.Count != 0){
            _destination = destination;
            foreach(Ship s in selectManager.SelectedUnits){
                s.MoveToPos(destination);
            }
        }
    }

    void AddSelectEffect(RigidBody unit){
            var selectEffectNode = (MeshInstance)SelectEffect.Instance();
            selectEffectNode.Scale = (unit.Scale*2);
            unit.AddChild(selectEffectNode);
    }

    void RemoveSelectEffect(){
        foreach(RigidBody c in selectManager.SelectedUnits){
            c.RemoveChild(c.GetNode("SelectEffect"));
        }
    }

    public void SelectUnit(RigidBody unit){
        if(!selectManager.SelectedUnits.Contains(unit)){
            RemoveSelectEffect();
            selectManager.SelectUnit(unit);
            AddSelectEffect(unit);
        }
    }

    public void AddSelectedUnit(RigidBody unit){
        if(unit.GetNodeOrNull("SelectEffect") == null){
            selectManager.AddSelectedUnit(unit);
            AddSelectEffect(unit);
            GD.Print(selectManager.SelectedUnits.Count);
        }
    }

    public void AddSelectedUnits(List<RigidBody> units){
        selectManager.AddSelectedUnits(units);
    }

    public void AddTarget(RigidBody target){
        foreach(RigidBody k in selectManager.SelectedUnits){
            if(k is Ship){
                Ship ship = (Ship)k;
                if(ship.targetManager.HasTarget){
                    ship.targetManager.AddTarget(target);
                }else{
                    ship.targetManager.SetTarget(target);
                    ship.MoveToTarget(target);
                    GD.Print("settarget");
                }
            }
        }
    }

    public void ClearTarget(){
        foreach(RigidBody k in selectManager.SelectedUnits){
            if(k is Ship){
                Ship ship = (Ship)k;
                ship.targetManager.ClearTargets();
            }
        }
    }


    public void ClearSelection(){
        RemoveSelectEffect();
        selectManager.ClearSelection();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetProcess(false);   
        selectManager = new SelectManager<RigidBody>();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
