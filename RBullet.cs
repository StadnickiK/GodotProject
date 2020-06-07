using Godot;
using System;

public class RBullet : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    PackedScene Particle = (PackedScene)ResourceLoader.Load("res://ExplosionParticles.tscn");
    int range = 5;
    Vector3 StartPosition = new Vector3();
    public Node Parent { get; set; }
    public Vector3 Dir { get; set; } = new Vector3(0,0,0);

    void Explode(){
        var temp = (ExplosionParticles)Particle.Instance();
        var t = temp.Transform;
        GetParent().AddChild(temp);
        t.origin = GlobalTransform.origin;
        temp.Scale = new Vector3(0.1f,0.1f,0.1f);
        temp.Transform = t;
    }

    public void Launch(Vector3 direction){
        StartPosition = Transform.origin;
        Dir = direction;
        SetProcess(true);
    }
    // Called when the node enters the scene tree for the first time.

    public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
        float length = (GlobalTransform.origin + StartPosition).Length();
        if(length<0){
            length = (-length);
        }
        if( length > range){
            Explode();
            QueueFree();
        }
        if(Dir != null){
            state.LinearVelocity = Dir;
        }else{
            QueueFree();
        }
        //GD.Print(Dir);
        //GD.Print(LinearVelocity);
        //GetCollidingBodies();
	}

    public override void _Ready()
    {
        
        //Translate(new Vector3(0,10,0));
        SetProcess(false);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
