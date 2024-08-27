using Godot;
using System;

public partial class RBullet : RigidBody3D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    PackedScene Particle = (PackedScene)ResourceLoader.Load("res://ExplosionParticles.tscn");


    [Export]
    public float Speed { get; set; } = 50;

    [Export]
    int EffectiveRange = 150;

    [Export]
    public int LifeTime { get; set; } = 1500;
    Vector3 StartPosition = new Vector3();
    public Node Parent { get; set; }
    public Vector3 Dir { get; set; } = new Vector3(0,0,0);

    public MovingTarget TargetPos { get; set; }

    Vector3 velocity;

    Godot.Timer timer; 

    VelocityController _veloctiyController;

    void Explode(){
        var temp = (ExplosionParticles)Particle.Instantiate();
        var t = temp.Transform;
        GetParent().AddChild(temp);
        t.Origin = GlobalTransform.Origin;
        temp.Scale = new Vector3(0.1f,0.1f,0.1f);
        temp.GlobalTransform = t;
        QueueFree();
    }

    public void Launch(Vector3 direction){
        StartPosition = GlobalTransform.Origin;
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

    // https://www.youtube.com/watch?v=_kA1fbBH4ug

    Vector3 CalcArcVelocity(Vector3 a, Vector3 b, Vector3 velocity){

        var vel = new Vector3();
        var targetVel = TargetPos.CurrentVelocity;
        var desired = GlobalTransform.Origin.DirectionTo(TargetPos.GlobalTransform.Origin + targetVel/4) * Speed;
        vel = (desired - velocity).Normalized() * Speed * 4;

        return vel;
    }

    bool check = true;

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
        float length = StartPosition.DistanceTo(GlobalTransform.Origin);
        if(length<0){
            length = (-length);
        }
        if( length > EffectiveRange){
            Explode();
            QueueFree();
        }
        if(check){
            state.LinearVelocity = velocity;
            check = false;
        }
        if(Dir != Vector3.Zero){
            deltaTime= GetProcessDeltaTime();
            var x = (CalcArcVelocity(GlobalTransform.Origin, TargetPos.GlobalTransform.Origin, state.LinearVelocity));
            var y = x * (float)deltaTime;
            state.LinearVelocity += y;
            // state.AngularVelocity = new Vector3(0,0,-10);//Dir*Speed;
        }else{
            QueueFree();
        }
        //GD.Print(Dir);
        // GD.Print(LinearVelocity);
        //GetCollidingBodies();
	}

    Random rand = new Random();

    public override void _Ready()
    {
        var randomParam = rand.Next(-20, 20);

        velocity = GlobalTransform.Origin.DirectionTo(TargetPos.GlobalTransform.Origin+new Vector3(0,190, 0)) * Speed;
        // velocity = GlobalTransform.Origin.DirectionTo(TargetPos+new Vector3(0,randomParam*2, randomParam)) * Speed;
        //Translate(new Vector3(0,10,0));
        _veloctiyController = new VelocityController();
        SetProcess(true);
        timer = GetNode<Godot.Timer>("Timer");
        timer.WaitTime = LifeTime;
    }

    double deltaTime = 0;

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(double delta)
 {
    deltaTime = delta;   
 }
}
