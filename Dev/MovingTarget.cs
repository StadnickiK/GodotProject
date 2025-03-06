using Godot;
using System;

public partial class MovingTarget : CharacterBody3D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public Vector3 velocity = new Vector3(0,0,20);

    public Vector3 CurrentVelocity { get; set; } = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }



 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(double delta)
 {  
    if(GlobalTransform.Origin.Z > 40){
        velocity *= -1;
    }
    if(GlobalTransform.Origin.Z < -40){
        velocity *= -1;
    }
    //CurrentVelocity = MoveAndSlide(velocity);
 }
}
