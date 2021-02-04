using Godot;
using System;

public class Camera : Godot.Camera
{

    [Export]
    public float MinZoom { get; set; } = 2.0f;
    [Export]
    public float MaxZoom { get; set; } = 50.0f;
    [Export]
    public float ZoomSpeed { get; set; } = 2f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        float z = (MaxZoom - MinZoom)/2;
        AddOrigin(new Vector3(0,0,z));
    }
public override void _Input(InputEvent inputEvent){
    if(inputEvent is InputEventMouseButton button){
        if((ButtonList)button.ButtonIndex == ButtonList.WheelUp){
            if(Transform.origin.z > MinZoom){
                AddOrigin(new Vector3(0,0,-ZoomSpeed));
            }
        }
        if((ButtonList)button.ButtonIndex == ButtonList.WheelDown){
            if(Transform.origin.z < MaxZoom){
                AddOrigin(new Vector3(0,0,ZoomSpeed));
            }
        }
    }
}

void AddOrigin(Vector3 position){
    var temp = Transform;
    temp.origin += position;
    Transform = temp;
}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
