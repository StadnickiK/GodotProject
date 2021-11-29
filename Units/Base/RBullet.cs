using Godot;
using System;

public class RBullet : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    PackedScene Particle = (PackedScene)ResourceLoader.Load("res://ExplosionParticles.tscn");


    [Export]
    public float Speed { get; set; } = 100;

    [Export]
    int EffectiveRange = 10;

    [Export]
    public int LifeTime { get; set; } = 10;
    Vector3 StartPosition = new Vector3();
    public Node Parent { get; set; }
    public Vector3 Dir { get; set; } = new Vector3(0,0,0);

    Godot.Timer timer; 

    VelocityController _veloctiyController;

    void Explode(){
        var temp = (ExplosionParticles)Particle.Instance();
        var t = temp.Transform;
        GetParent().AddChild(temp);
        t.origin = Transform.origin;
        temp.Scale = new Vector3(0.1f,0.1f,0.1f);
        temp.Transform = t;
    }

    public void Launch(Vector3 direction){
        StartPosition = GlobalTransform.origin;
        Dir = direction;
        SetProcess(true);
    }
    // Called when the node enters the scene tree for the first time.

    void _on_body_entered(Node body){
        Explode();
    }

    void _on_Timer_timeout(){
        Explode();
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
        float length = StartPosition.DistanceTo(GlobalTransform.origin);
        if(length<0){
            length = (-length);
        }
        if( length > EffectiveRange){
            Explode();
            QueueFree();
        }
        if(Dir != null){
            state.LinearVelocity = Dir*Speed;
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
        _veloctiyController = new VelocityController();
        SetProcess(false);
        timer = GetNode<Godot.Timer>("Timer");
        timer.WaitTime = LifeTime;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
