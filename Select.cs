using Godot;
using System;
using System.Collections.Generic;

public partial class Select : Node
{

    SelectManager<CollisionObject3D> selectManager;
    PackedScene SelectEffect = (PackedScene)ResourceLoader.Load("res://SelectEffect3.tscn");

    Vector3 _destination;

    public delegate void MoveToPosEventHandler(Vector3 pos);

    public event MoveToPosEventHandler OnMove;

    public delegate void MoveToTargetEventHandler(TargetManager<Node3D>.Target target);

    public event MoveToTargetEventHandler OnTarget;

    public delegate void ClearTargetEventHandler();

    public event ClearTargetEventHandler OnClear;

    public void MoveToPosition(Vector3 destination){
        if(selectManager.SelectedUnits.Count != 0){
            _destination = destination;
            OnMove?.Invoke(destination);
        }
    }

    public void MoveToTarget(TargetManager<Node3D>.Target target){
        if(selectManager.SelectedUnits.Count != 0){
            OnTarget?.Invoke(target);
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
            if(unit is Ship ship) 
                SubscribeShip(ship);
            AddSelectEffect(unit);
        }
    }

    void SubscribeShip(Ship ship){
        OnMove += ship.MoveToPos;
        OnTarget += ship.MoveToTarget;
        OnClear += ship.targetManager.ClearTargets;
    }

    void UnsubscribeShip(Ship ship){
        OnMove -= ship.MoveToPos;
        OnTarget -= ship.MoveToTarget;
        OnClear -= ship.targetManager.ClearTargets;
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
        var t = new TargetManager<Node3D>.Target(target.GlobalPosition,target);
        OnTarget?.Invoke(t);
    }

    public void AddTarget(CollisionObject3D target, CmdPanel.CmdPanelOption task){
        foreach(CollisionObject3D rigidB in selectManager.SelectedUnits){
            if(rigidB is Ship){
                Ship ship = (Ship)rigidB;
                var t = new TargetManager<Node3D>.Target(target.GlobalPosition,target);
                if(ship.targetManager.HasTarget){
                    ship.targetManager.AddTarget(t);
                    // ship.Task = task;
                }else{
                    ship.targetManager.SetTarget(t);
                    // ship.Task = task;
                    ship.MoveToTarget(t);
                }
            }
        }
    }

    public void ClearTarget(){
        OnClear?.Invoke();
    }


    public void ClearSelection(){
        RemoveSelectEffect();
        foreach(var unit in selectManager.SelectedUnits)
            if(unit is Ship ship)
                UnsubscribeShip(ship);
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
