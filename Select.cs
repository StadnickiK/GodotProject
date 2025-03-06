using Godot;
using System;
using System.Collections.Generic;

public partial class Select : Node
{

    SelectManager<CollisionObject3D> selectManager;
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

    public void MoveToTarget(Node3D target){
        if(selectManager.SelectedUnits.Count != 0){
            foreach(Ship s in selectManager.SelectedUnits){
                s.MoveToTarget(target);
            }
        }
    }

    void AddSelectEffect(CollisionObject3D unit){
            var selectEffectNode = (MeshInstance3D)SelectEffect.Instantiate();
            selectEffectNode.Scale = (unit.Scale*2);
            unit.AddChild(selectEffectNode);
    }

    void RemoveSelectEffect(){
        foreach(CollisionObject3D c in selectManager.SelectedUnits){
            if(c != null){
                c.RemoveChild(c.GetNode("SelectEffect"));
            }else{
                selectManager.SelectedUnits.Remove(c);
            }
        }
    }

    public void SelectUnit(CollisionObject3D unit){
        if(!selectManager.SelectedUnits.Contains(unit)){
            RemoveSelectEffect();
            selectManager.SelectUnit(unit);
            AddSelectEffect(unit);
        }
    }

    public void AddSelectedUnit(CollisionObject3D unit){
        if(unit.GetNodeOrNull("SelectEffect") == null){
            selectManager.AddSelectedUnit(unit);
            AddSelectEffect(unit);
        }
    }

    public void AddSelectedUnits(List<CollisionObject3D> units){
        selectManager.AddSelectedUnits(units);
    }

    public void AddTarget(CollisionObject3D target){
        foreach(CollisionObject3D rigidB in selectManager.SelectedUnits){
            if(rigidB is Ship){
                Ship ship = (Ship)rigidB;
                if(ship.targetManager.HasTarget){
                    ship.targetManager.AddTarget(new TargetManager<Node3D>.Target(target.GlobalPosition,target));
                }else{
                    ship.targetManager.SetTarget(new TargetManager<Node3D>.Target(target.GlobalPosition,target));
                    ship.MoveToTarget(target);
                }
            }
        }
    }

    public void AddTarget(CollisionObject3D target, CmdPanel.CmdPanelOption task){
        foreach(CollisionObject3D rigidB in selectManager.SelectedUnits){
            if(rigidB is Ship){
                Ship ship = (Ship)rigidB;
                if(ship.targetManager.HasTarget){
                    ship.targetManager.AddTarget(new TargetManager<Node3D>.Target(target.GlobalPosition,target));
                    // ship.Task = task;
                }else{
                    ship.targetManager.SetTarget(new TargetManager<Node3D>.Target(target.GlobalPosition,target));
                    // ship.Task = task;
                    ship.MoveToTarget(target);
                }
            }
        }
    }

    public void ClearTarget(){
        foreach(CollisionObject3D k in selectManager.SelectedUnits){
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
        selectManager = new SelectManager<CollisionObject3D>();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
