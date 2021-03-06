using Godot;
using System;

public class WorldCursorControl : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Select select;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        select = GetNode<Select>("/root/World/WorldCursorControl/Select");;
    }

    public void _SelectUnit(RigidBody unit){
        select.AddSelectedUnit(unit);
    }

    public void _SelectTarget(RigidBody unit){;
        select.AddTarget(unit);
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
        if(inputEvent is InputEventMouseButton button){ // mouse
            if((ButtonList)button.ButtonIndex == ButtonList.Right && select != null){   // right click
                select.MoveToPosition(GetMouseWorldPosition());
                select.ClearTarget();
            }
            if((ButtonList)button.ButtonIndex == ButtonList.Left && select != null){    // left click
                select.ClearSelection();
            }
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
