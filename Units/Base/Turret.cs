using Godot;
using System;
using System.Collections.Generic;

public partial class Turret : CharacterBody3D
{
    protected VelocityController _velocityController = null;

    [Export]
    public int EffectiveRange { get; set; } = 10;

    public ProjectileFactory ProjectileFactory { get; set; }

    [Export]
    public float RotationSpeed { get; set; } = 1;

    [Export(PropertyHint.Range, "1,10000,1,or_greater")]
    public int FireRate { get; set; } = 60;
    protected bool timerStarted = false;

    public List<MeshInstance3D> Barrels { get; set; } = new List<MeshInstance3D>();

    public MeshInstance3D TurretMesh { get; set; }

    public OrderQueue.Target Target { get; set; }

    public void SetTarget(OrderQueue.Target target)
    {
        Target = target;
    }

    public void RemoveTarget()
    {
        Target = null;
    }

    protected void ResetVelocity()
    {
        _velocityController.ResetSpeed();
        // LinearVelocity = Vector3.Zero;
        // AngularVelocity = Vector3.Zero;
        // Sleeping = true;
    }

    protected Vector3 DirToTarget(){
        return GlobalTransform.Origin.DirectionTo(Target.Point);
    }

    protected void Shoot(){
        foreach (var item in Barrels)
        {
            var projectile = ProjectileFactory.GetLaser(this, item.GlobalPosition);
            // var transform = projectile.Transform;
            var dir = DirToTarget();
            projectile.Shoot(dir);
        }
        //transform.Origin = Barrel.GlobalTransform.Origin + 1.2f*Scale.Z*dir;
        //projectile.Transform = transform;
        //projectiles.AddChild(projectile);
        
    }

    // public void _IntegrateForces(PhysicsDirectBodyState3D state){

    //     if(OrderQueue.HasTarget){
    //         Vector3 targetPos = OrderQueue.currentTarget.GlobalTransform.Origin;
    //         if(targetPos != Vector3.Zero){
    //             UpdateYrotation(state, targetPos);
    //             UpdateMuzzle(targetPos);
    //         }else{
    //             Sleeping = true;
    //         }
    //     }
    // }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Init();
    }

    public void Init()
    {
        _velocityController = new VelocityController();
        _velocityController.RotationSpeed = RotationSpeed;
        _velocityController.Mass = 10;
        //_ConnectSignal();
        TurretMesh = GetNode<MeshInstance3D>("TurretMesh");
    }
    
        void UpdateYrotation(PhysicsDirectBodyState3D state, Vector3 targetPos){
        float angleY = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
        // if(angleY > 0.05f || angleY < -0.05f ){
        //     state.AngularVelocity = _velocityController.GetAngularVelocity(GlobalTransform,targetPos);
        //     StopTimer();
        //     if(barrel != null){
        //         barrel.OrderQueue.ClearTargets();
        //     }
        // }else{
        //     if(OrderQueue.HasTarget){
        //         if(barrel != null){
        //             barrel.OrderQueue.AddTarget(OrderQueue.currentTarget);
        //         }else{
        //             if(!timerStarted){
        //                 StartTimer();
        //             }
        //         }
        //     }
        //     ResetVelocity();
        // }
    }

    void UpdateMuzzle(Vector3 targetPos){
        // float angle = _velocityController.GetAngleToTargetOnXAxis(Barrel.GlobalTransform, targetPos); 
        // GD.Print(angle);
        // if(angle > 0.1f || angle < -0.1f ){
        //     if(Barrel != null){
        //         if(angle<Math.PI && angle > -Math.PI)
        //             Barrel.Rotation += new Vector3(angle/10,0,0);
        //     }
        // }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
