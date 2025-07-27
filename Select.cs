using Godot;
using System;
using System.Collections.Generic;

public partial class Select : Node
{

    SelectManager<CollisionObject3D> selectManager;
    PackedScene SelectEffect = (PackedScene)ResourceLoader.Load("res://SelectEffect.tscn");

    Vector3 _destination;

    public delegate void MoveToPosEventHandler(Vector3 pos);

    public event MoveToPosEventHandler OnMove;

    public delegate void MoveToTargetEventHandler(OrderQueue.Target target);

    public event MoveToTargetEventHandler OnTarget;

    public delegate void ClearTargetEventHandler();

    public event ClearTargetEventHandler OnClear;

    public void MoveToPosition(Vector3 destination){
        if(selectManager.SelectedUnits.Count != 0){
            _destination = destination;
            OnMove?.Invoke(destination);
        }
    }

    public void MoveToTarget(OrderQueue.Target target){
        if(selectManager.SelectedUnits.Count != 0){
            OnTarget?.Invoke(target);
        }
    }

    void AddSelectEffect(CollisionObject3D unit){
        var shape = unit.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if(shape != null)
            unit.AddChild(GetSelectEffect(shape.GetAabb().Size));
    }

    Node GetSelectEffect(Vector3 vector3)
    {
        var selectEffectNode = (MeshInstance3D)SelectEffect.Instantiate();
        var mesh = (SphereMesh)selectEffectNode.Mesh;
        var size = vector3.Z > vector3.X ? vector3.Z : vector3.X;
        mesh.Height = size;
        mesh.Radius = size / 2;
        return selectEffectNode;
    }

    void RemoveSelectEffect(){
        foreach(CollisionObject3D c in selectManager.SelectedUnits){
            if(c != null){
                var s = c.GetNode("SelectEffect");
                c.RemoveChild(s);
                s.QueueFree();
            }else{
                selectManager.SelectedUnits.Remove(c);
            }
        }
    }

    public void SelectUnit(CollisionObject3D unit){
        if(!selectManager.SelectedUnits.Contains(unit)){
            RemoveSelectEffect();
            selectManager.SelectUnit(unit);
            if(unit is IMovable ship) 
                SubscribeShip(ship);
            AddSelectEffect(unit);
        }
    }

    void SubscribeShip(IMovable ship){
        OnMove += ship.MoveToPosition;
        OnTarget += ship.MoveToTarget;
        OnClear += ship.ClearTargets;
    }

    void UnsubscribeShip(IMovable ship){
        OnMove -= ship.MoveToPosition;
        OnTarget -= ship.MoveToTarget;
        OnClear -= ship.ClearTargets;
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
        var t = new OrderQueue.Target(target.GlobalPosition,target);
        OnTarget?.Invoke(t);
    }

    public void AddTarget(CollisionObject3D target, CmdPanel.CmdPanelOption task){
        foreach(CollisionObject3D rigidB in selectManager.SelectedUnits){
            if(rigidB is Ship){
                Ship ship = (Ship)rigidB;
                var t = new OrderQueue.Target(target.GlobalPosition,target);
                if(ship.OrderQueue.HasTarget){
                    ship.OrderQueue.AddTarget(t);
                    // ship.Task = task;
                }else{
                    ship.OrderQueue.SetTarget(t);
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
            if(unit is IMovable ship)
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
