using Godot;
using System;
using System.Collections.Generic;

public partial class Select : Node
{

    SelectManager<ISelection> selectManager;

    Vector3 _destination;

    public delegate void MoveToPosEventHandler(Vector3 pos);

    public event MoveToPosEventHandler OnMove;

    public delegate void MoveToTargetEventHandler(OrderQueue.Target target);

    public event MoveToTargetEventHandler OnTarget;

    public delegate void ClearTargetEventHandler();

    public event ClearTargetEventHandler OnClear;
    
    public bool HasMany { get => selectManager.SelectedUnits.Count > 1; }

    public HashSet<ISelection> SelectedUnits { get { return selectManager.SelectedUnits;} }

    Vector3 CalculateFormationCenter()
    {
        Vector3 center = new Vector3();
        foreach (var unit in selectManager.SelectedUnits)
        {
            center += unit.GlobalPosition;
        }
        return center / selectManager.SelectedUnits.Count;
    }

    public void MoveToPosition(Vector3 destination)
    {
        if (!HasMany)
        {
            _destination = destination;
            OnMove?.Invoke(destination);
        }
        else
        {
            var formationCenter = CalculateFormationCenter();
            foreach (var unit in selectManager.SelectedUnits)
            {
                unit.MoveToPosition(destination + (unit.GlobalPosition - formationCenter));
            }
        }
    }

    public void MoveToTarget(OrderQueue.Target target){
        if(selectManager.SelectedUnits.Count != 0){
            OnTarget?.Invoke(target);
        }
    }

    void AddSelectEffect(){
        foreach(var c in selectManager.SelectedUnits)
                c.Select();
    }

    void AddSelectEffect(ISelection c){
        c.Select();
    }

    void RemoveSelectEffect(){
        foreach(var c in selectManager.SelectedUnits)
                c.Deselect();
    }

    void RemoveSelectEffect(ISelection unit){
        unit.Deselect();
    }

    void SubscribeShip(ISelection ship){
        OnMove += ship.MoveToPosition;
        OnTarget += ship.MoveToTarget;
        OnClear += ship.ClearTargets;
    }

    void UnsubscribeShip(ISelection ship){
        OnMove -= ship.MoveToPosition;
        OnTarget -= ship.MoveToTarget;
        OnClear -= ship.ClearTargets;
    }

    public void SelectUnit(ISelection unit){
        if(!selectManager.SelectedUnits.Contains(unit)){
            ClearSelection();
            selectManager.SelectUnit(unit);
            UpdateSelection(unit);
        }
    }

    public void DeselectUnit(ISelection unit){
        if(selectManager.SelectedUnits.Contains(unit)){
            UnsubscribeShip(unit);
            selectManager.DeselectUnit(unit);
            RemoveSelectEffect(unit);
        }
    }

    public void AddSelectedUnit(ISelection unit)
    {
        if (!selectManager.SelectedUnits.Contains(unit))
        {
            selectManager.AddSelectedUnit(unit);
            UpdateSelection(unit);
        }
    }

    void UpdateSelection(ISelection unit)
    {
        RemoveSelectEffect();
        SubscribeShip(unit);
        AddSelectEffect();
    }

    public void AddTarget(INode3D target){
        var t = new OrderQueue.Target(target.GlobalPosition,target);
        OnTarget?.Invoke(t);
    }

    public void AddTarget(INode3D target, CmdPanel.CmdPanelOption task){
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
        foreach (var unit in selectManager.SelectedUnits)
        {
            UnsubscribeShip(unit);
            unit.Deselect();
        }
        selectManager.ClearSelection();
    }

    public bool HasSelected(){
        return selectManager.HasSelect;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // SetProcess(false);   
        selectManager = new SelectManager<ISelection>();
        AddChild(selectManager);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
