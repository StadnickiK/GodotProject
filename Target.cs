using Godot;
using System;

public class Target : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    int y = 0;
    int z = -10;
    int x = 0;
    public override void _Ready()
    {
        SetProcess(true);
    }
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
        
        if(Transform.origin.z > 12 && Transform.origin.x > 6){
            x = 0;
            z = -10; 
        }

        if(Transform.origin.z < -9 && Transform.origin.x > 6){
            x = -10;
            z = 0;
        }

        if(Transform.origin.z < -9 && Transform.origin.x < -16){
            x = 0;
            z = 10;
        }

        if(Transform.origin.z > 12 && Transform.origin.x < -16){
            x = 10;
            z = 0;
        }

/*
        var bodies= GetCollidingBodies();
        if(bodies.Count != 0){
            GD.Print("h");
        }
*/
        var upDir = new Vector3(x, y, z);
        var pos = new Vector3(0, 0, 0);
        //state.AddForce(upDir,pos);
        state.LinearVelocity = upDir; //new Vector3(-10, 0, -10);
        //state.AngularVelocity = upDir;
        //GD.Print(AngularVelocity);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//public override void _Process(float delta)
//  {
//  }
}
