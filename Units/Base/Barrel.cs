using Godot;
using System;

public class Barrel : Turret
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    //float z;
    //bool direction = true;
    //public float ReloadTime = 1;
    [Export(PropertyHint.Range, "0,1,0.01")]
    public float RecoilRange { get; set; } = 0;

    [Export]
    public int RecoilSpeed { get; set; } = 100;

    float timePassed = 0;

    //int Max_ammo = 600;
    //int Ammo = 0;
    [Export]
    new public PackedScene projectileScene = (PackedScene)ResourceLoader.Load("res://Units/Base/RBullet.tscn");

    [Export]
    new public float RotationSpeed { get; set; } = 10;

    RayCast ray;
    // Called when the node enters the scene tree for the first time.

    void _on_Timer_timeout(){
        if(targetManager.HasTarget){
            GD.Print("s");
            Shoot();
        }
    }

    new protected Vector3 DirToTarget(){
        return GlobalTransform.origin.DirectionTo(targetManager.currentTarget.GlobalTransform.origin);
    }

    public override void _Ready()
    {
            ray = GetNode<RayCast>("RayCast");
            ray.CastTo = ray.Transform.origin+new Vector3(0,0,EffectiveRange);
            projectiles = GetNode("/root/World/Projectiles");
            targetManager = new TargetManager<Spatial>();
            _velocityController.RotationSpeed = RotationSpeed;
            SetTimer();
    }

    new void ResetVelocity(){
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Sleeping = true;
    }
    
    void Recoil(PhysicsDirectBodyState state){
        if(Transform.origin.z >= -Scale.z*RecoilRange){
            state.LinearVelocity = -DirToTarget()*RecoilSpeed;
        }else if(Transform.origin.z <= -Scale.z*RecoilRange){
            state.LinearVelocity = DirToTarget()*RecoilSpeed;
        }
    }

    void Update(PhysicsDirectBodyState state){
        if(targetManager.HasTarget){
            Vector3 targetPos = targetManager.currentTarget.GlobalTransform.origin;
            if(targetPos != Vector3.Zero && targetPos != null){
                float angle = _velocityController.GetAngleToTargetOnXAxis(GlobalTransform, targetPos); 
                if(angle > 0.05f || angle < -0.05f ){
                    StopTimer();
                }else{
                    if(targetManager.HasTarget){
                        StartTimer();
                    }
                }
            }else{
                Sleeping = true;
            }
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        Update(state);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  //public override void _Process(float delta)
  //{
  //}
}
