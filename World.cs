using Godot;
using System;

public class World : Spatial
{
    
    Spatial Galaxy = null;
    Spatial Camera = null;


    void _on_CameraLookAt(Vector3 position){
        GD.Print(position);
        Camera.LookAt(position, new Vector3(0,1,0));
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Galaxy = (Spatial)GetNode("Galaxy");
        Camera = (Spatial)GetNode("CameraGimbal");
        Galaxy.Connect("CameraLookAt",this, nameof(_on_CameraLookAt));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
