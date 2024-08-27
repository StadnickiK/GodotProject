using Godot;
using System;

public partial class InnerGimbal : Node3D
{
    bool drag = false;


    [Export]
    public float RotationSpeed { get; set; } = 0.7f;
    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent inputEvent){
        if(inputEvent is InputEventMouseButton button){
            if(button.ButtonIndex == MouseButton.Right){
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
                RotateX(Mathf.DegToRad(motion.Relative.Y*RotationSpeed));
                var rot = RotationDegrees;
                rot.X = Mathf.Clamp(rot.X, -80,80);
                RotationDegrees = rot;
        }
    } 

     public override void _Process(double delta)
    {
          
    }
}
