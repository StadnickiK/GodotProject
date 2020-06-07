using Godot;
using System;

public class InnerGimbal : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    bool drag = false;


    [Export]
    public float RotationSpeed { get; set; } = 0.7f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }
public override void _Input(InputEvent inputEvent){
    if(inputEvent is InputEventMouseButton button){
        if((ButtonList)button.ButtonIndex == ButtonList.Right){
            if(drag != true){
                drag = true;
            }else{
                drag = false;
            }
            //MouseCameraControl(inputEvent);
        }
    }

    if(inputEvent is InputEventMouseMotion motion && drag){
            //GlobalRotate(new Vector3(1,0,0), Mathf.Deg2Rad(motion.Relative.y*RotationSpeed));
            RotateX(Mathf.Deg2Rad(motion.Relative.y*RotationSpeed));
            var rot = RotationDegrees;
            rot.x = Mathf.Clamp(rot.x, -80,80);
            RotationDegrees = rot;
    }
}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
