using Godot;
using System;

public class KinematicBody : Godot.KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    int dir = 0;
    bool up = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
      if(Transform.origin.y > 15){
          up = false;
      }else if(Transform.origin.y< -15){
          up = true;
      }
    
    if(up == true){
        Translate(new Vector3(0,0.1f,0));
    }else{
        Translate(new Vector3(0,-0.1f,0));
    }

    if(Transform.origin.z < -10 && Transform.origin.x > 17){
        dir = 0;     
    }else if(Transform.origin.z > 11 && Transform.origin.x > 17){
        dir = 1;
    }else if(Transform.origin.z < -10 && Transform.origin.x < -5){
        dir = 2;
    }else if(Transform.origin.z > 11 && Transform.origin.x < -5){
        dir = 3;
    }
    if(dir == 0){
        Translate(new Vector3(-0.1f,0,0));
    }else if(dir == 1){
        Translate(new Vector3(0,0,-0.1f)); 
    }else if(dir == 2){
        Translate(new Vector3(0,0,0.1f)); 
    }else if(dir == 3){
        Translate(new Vector3(0.1f,0,0)); 
    }
  }
}
