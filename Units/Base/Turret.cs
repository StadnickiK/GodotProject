using Godot;
using System;

 public partial class Turret : Ship
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    protected VelocityController _velocityController = null;

    [Export]
    public int EffectiveRange { get; set; } = 10;

    [Export]
    public PackedScene projectileScene = (PackedScene)ResourceLoader.Load("res://Units/Base/RBullet.tscn");

    [Export]
    PackedScene barrelScene = null;

    [Export]
    public float RotationSpeed { get; set; } = 1;

    [Export(PropertyHint.Range, "1,10000,1,or_greater")]
    public int FireRate { get; set; } = 60;

    protected Node projectiles;

    protected Godot.Timer timer;

    protected bool timerStarted = false;

    Barrel barrel = null;

    Node3D muzzle = null;

    // Called when the node enters the scene tree for the first time.

    protected void StartTimer(){
        timer.Start();
        timerStarted = true;
    }

    protected void StopTimer(){
        timer.Stop();
        timerStarted = false;
    }

    void _Timer_timeout(){
        if(targetManager.HasTarget){
            Shoot();
        }
    }

    protected void SetTimer(){
        timer = GetNode<Godot.Timer>("Timer");
        timer.WaitTime = 60/(float)FireRate;
    }

    new protected void ResetVelocity(){
        _velocityController.ResetSpeed();
        // LinearVelocity = Vector3.Zero;
        // AngularVelocity = Vector3.Zero;
        // Sleeping = true;
    }

    protected Vector3 DirToTarget(){
        return GlobalTransform.Origin.DirectionTo(targetManager.currentTarget.Point);
    }

    protected void Shoot(){
        var projectile = (RBullet)projectileScene.Instantiate();
        var transform = projectile.Transform;
        var dir = DirToTarget();

        transform.Origin = muzzle.GlobalTransform.Origin + 1.2f*Scale.Z*dir;
        projectile.Transform = transform;
        projectiles.AddChild(projectile);
        projectile.Launch(dir);
    }

    // public void _IntegrateForces(PhysicsDirectBodyState3D state){

    //     if(targetManager.HasTarget){
    //         Vector3 targetPos = targetManager.currentTarget.GlobalTransform.Origin;
    //         if(targetPos != Vector3.Zero){
    //             UpdateYrotation(state, targetPos);
    //             UpdateMuzzle(targetPos);
    //         }else{
    //             Sleeping = true;
    //         }
    //     }
    // }

    void ReadyBarrel(){
        if(barrelScene != null){
            barrel = (Barrel)barrelScene.Instantiate();
            muzzle.AddChild(barrel);
            barrel.Translate(muzzle.Transform.Origin+new Vector3(0,0,Scale.Z*2f));
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _velocityController = new VelocityController();
        _velocityController.RotationSpeed = RotationSpeed;
        targetManager = new TargetManager<Node3D>();
        _velocityController.Mass = 10;
        //_ConnectSignal();
        muzzle = GetNode<Node3D>("Muzzle");
        ReadyBarrel();

        projectiles = GetNode("/root/World/Projectiles");
        SetTimer();
    }
    
        void UpdateYrotation(PhysicsDirectBodyState3D state, Vector3 targetPos){
        float angleY = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
                if(angleY > 0.05f || angleY < -0.05f ){
                    state.AngularVelocity = _velocityController.GetAngularVelocity(GlobalTransform,targetPos);
                    StopTimer();
                    if(barrel != null){
                        barrel.targetManager.ClearTargets();
                    }
                }else{
                    if(targetManager.HasTarget){
                        if(barrel != null){
                            barrel.targetManager.AddTarget(targetManager.currentTarget);
                        }else{
                            if(!timerStarted){
                                StartTimer();
                            }
                        }
                    }
                    ResetVelocity();
                }
    }

    void UpdateMuzzle(Vector3 targetPos){
        float angle = _velocityController.GetAngleToTargetOnXAxis(muzzle.GlobalTransform, targetPos); 
        GD.Print(angle);
        if(angle > 0.1f || angle < -0.1f ){
            if(muzzle != null){
                if(angle<Math.PI && angle > -Math.PI)
                    muzzle.Rotation += new Vector3(angle/10,0,0);
            }
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
