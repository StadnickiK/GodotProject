using Godot;
using System;
using System.Collections.Generic;

public class Select : Node
{

    SelectManager<CollisionObject> selectManager;
    PackedScene SelectEffect = (PackedScene)ResourceLoader.Load("res://SelectEffect3.tscn");

    Vector3 _destination;

    public void MoveToPosition(Vector3 destination){
        if(selectManager.SelectedUnits.Count != 0){
            _destination = destination;
            foreach(Node node in selectManager.SelectedUnits){
                if(node is Ship ship)
                    ship.MoveToPos(destination);
            }
        }
    }

    public void MoveToTarget(Spatial target){
        if(selectManager.SelectedUnits.Count != 0){
            foreach(Ship s in selectManager.SelectedUnits){
                s.MoveToTarget(target);
            }
        }
    }

    void AddSelectEffect(CollisionObject unit){
            var selectEffectNode = (MeshInstance)SelectEffect.Instance();
            selectEffectNode.Scale = (unit.Scale*2);
            unit.AddChild(selectEffectNode);
    }

    void RemoveSelectEffect(){
        foreach(CollisionObject c in selectManager.SelectedUnits){
            if(c != null){
                c.RemoveChild(c.GetNode("SelectEffect"));
            }else{
                selectManager.SelectedUnits.Remove(c);
            }
        }
    }

    public void SelectUnit(CollisionObject unit){
        if(!selectManager.SelectedUnits.Contains(unit)){
            RemoveSelectEffect();
            selectManager.SelectUnit(unit);
            AddSelectEffect(unit);
        }
    }

    public void AddSelectedUnit(CollisionObject unit){
        if(unit.GetNodeOrNull("SelectEffect") == null){
            selectManager.AddSelectedUnit(unit);
            AddSelectEffect(unit);
        }
    }

    public void AddSelectedUnits(List<CollisionObject> units){
        selectManager.AddSelectedUnits(units);
    }

    public void AddTarget(CollisionObject target){
        foreach(CollisionObject rigidB in selectManager.SelectedUnits){
            if(rigidB is Ship){
                Ship ship = (Ship)rigidB;
                if(ship.targetManager.HasTarget){
                    ship.targetManager.AddTarget(target);
                }else{
                    ship.targetManager.SetTarget(target);
                    ship.MoveToTarget(target);
                }
            }
        }
    }

    public void AddTarget(CollisionObject target, CmdPanel.CmdPanelOption task){
        foreach(CollisionObject rigidB in selectManager.SelectedUnits){
            if(rigidB is Ship){
                Ship ship = (Ship)rigidB;
                if(ship.targetManager.HasTarget){
                    ship.targetManager.AddTarget(target);
                    ship.Task = task;
                }else{
                    ship.targetManager.SetTarget(target);
                    ship.Task = task;
                    ship.MoveToTarget(target);
                }
            }
        }
    }

    public void ClearTarget(){
        foreach(CollisionObject k in selectManager.SelectedUnits){
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

    public bool HasSelected(){
        return selectManager.HasSelect;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // SetProcess(false);   
        selectManager = new SelectManager<CollisionObject>();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
