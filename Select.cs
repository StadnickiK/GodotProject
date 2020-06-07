using Godot;
using System;
using System.Collections.Generic;

public class Select : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    SelectManager selectManager;
    PackedScene SelectEffect = (PackedScene)ResourceLoader.Load("res://SelectEffect3.tscn");

    Vector3 _destination;


    void AddSelectEffect(KinematicCube unit){
            var selectEffectNode = (MeshInstance)SelectEffect.Instance();
            selectEffectNode.Scale = (unit.Scale*2);
            unit.AddChild(selectEffectNode);
    }

    void RemoveSelectEffect(){
        foreach(KinematicCube c in selectManager.SelectedUnits){
            c.RemoveChild(c.GetNode("SelectEffect"));
        }
    }

    public void SelectUnit(KinematicCube unit){
        if(!selectManager.SelectedUnits.Contains(unit)){
            RemoveSelectEffect();
            selectManager.SelectUnit(unit);
            AddSelectEffect(unit);
        }
    }

    public void AddSelectedUnit(KinematicCube unit){
        selectManager.AddSelectedUnit(unit);
    }

    public void AddSelectedUnits(List<KinematicCube> units){
        selectManager.AddSelectedUnits(units);
    }

    public void AddTarget(KinematicCube target){
        foreach(KinematicCube k in selectManager.SelectedUnits){
            k.targetManager.SetTarget(target);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
     SetProcess(false);   
     selectManager = new SelectManager();

    }

    Vector3 GetMouseWorldPosition(){
        var ray_length = 1000;
        var mousePos = GetViewport().GetMousePosition();
        var camera = (Camera)GetParent().GetNode("CameraGimbal/InnerGimbal/FreeCamera");
        var from = camera.ProjectRayOrigin(mousePos);
        var to = from + camera.ProjectRayNormal(mousePos) * ray_length;
        var space_state = GetWorld().DirectSpaceState;
        var state = space_state.IntersectRay(from, to);
        Vector3 p = Vector3.Zero;
        if(state.Contains("position")){
            p = (Vector3)state["position"];
        }
        return p;
    }
     public override void _Input(InputEvent inputEvent){
        if(inputEvent is InputEventMouseButton button){
            if((ButtonList)button.ButtonIndex == ButtonList.Right){
                _destination = GetMouseWorldPosition();
                GD.Print("Dest " + _destination);
            }
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
