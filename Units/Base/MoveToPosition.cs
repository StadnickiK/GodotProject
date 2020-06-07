using Godot;
using System;

public class MoveToPosition : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    Vector3 dir = new Vector3();

    Vector3 targetPos = Vector3.Zero;

    VelocityController _velocityController = new VelocityController();

    void UpdateLinearVelocity(){
            LinearVelocity = _velocityController.GetAcceleratedVelocity(LinearVelocity,GlobalTransform.origin,targetPos);
            GD.Print("LV "+LinearVelocity);
    }

    void ResetVelocity(){
        _velocityController.ResetSpeed();
        LinearVelocity = Vector3.Zero;
        targetPos = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Sleeping = true;
    }

    public void MoveToPos(Vector3 destination){
        Sleeping = false;
        targetPos = destination;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state){
        if(targetPos != Vector3.Zero && targetPos != null){
            state.AngularVelocity = _velocityController.GetAngularVelocity(Transform,targetPos);
            UpdateLinearVelocity();
            if(LinearVelocity != Vector3.Zero){
                if(LinearVelocity < new Vector3(0.001f,0.001f,0.001f) && 
                LinearVelocity > new Vector3(-0.001f,-0.001f,-0.001f)){
                    ResetVelocity();
                }  
            }
        }else{
            Sleeping = true;
        }
    }

    public override void _Input(InputEvent inputEvent){
        if(inputEvent is InputEventMouseButton button){
            if((ButtonList)button.ButtonIndex == ButtonList.Right){
                targetPos = GetMouseWorldPosition();
                Sleeping = false;
                GD.Print("target "+targetPos);
            }
        }
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
