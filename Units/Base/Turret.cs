using Godot;
using System;
using System.Collections.Generic;

public partial class Turret : CharacterBody3D, IStatManager
{
    protected VelocityController _velocityController = null;

    public ProjectileFactory ProjectileFactory { get; set; }

    public List<MeshInstance3D> Barrels { get; set; } = new List<MeshInstance3D>();

    public List<string> Projectiles { get; set; } = new List<string>();

    public MeshInstance3D TurretMesh { get; set; }

    public ITargetable Target { get; set; }

    Vector3 IdleRotation = Vector3.Zero;

    public Node3D TransformParent { get; set; }

    public StatManager StatManager { get; set; }

    public int Index { get; set; }

    public void RemoveTarget()
    {
        Target = null;
    }

    public TurretState State { get; set; } = TurretState.Idle;

    public enum TurretState
    {
        Idle,
        Tracking,

        Reset
    }

    protected void ResetVelocity()
    {
        _velocityController.ResetSpeed();
        // LinearVelocity = Vector3.Zero;
        // AngularVelocity = Vector3.Zero;
        // Sleeping = true;
    }

    protected Vector3 DirToTarget(){
        return GlobalTransform.Origin.DirectionTo(Target.GlobalPosition);
    }

    public void Shoot(ITargetable target, IDamagable Source){
        foreach (var item in Barrels)
        {
            var projectile = ProjectileFactory.CreateLaser(item.GlobalPosition, target);
            projectile.CollisionObject3D = this;
            projectile.Source = Source;
            projectile.Show();
            // var transform = projectile.Transform;
            //var dir = DirToTarget();
            //projectile.Shoot(item.GlobalPosition, Target, dir);
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
        _velocityController = GetNode<VelocityController>("VelocityController");
        StatManager = GetNode<StatManager>("StatManager");
        //_ConnectSignal();
        TurretMesh = GetNode<MeshInstance3D>("TurretMesh");
    }
    
    void UpdateYrotation(Vector3 targetPos, double delta){
        float angleY = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
        if(angleY > _velocityController.RotationTolerance || angleY < - _velocityController.RotationTolerance ){
            Rotation += _velocityController.GetAngularVelocity(GlobalTransform, targetPos) * (float)delta;
        }else{
            ResetVelocity();
            if(State == TurretState.Reset) State = TurretState.Idle;
        }
    }

    void UpdateYrotation2(Vector3 targetPos, double delta){
        float angleY = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
        if(angleY > _velocityController.RotationTolerance || angleY < - _velocityController.RotationTolerance ){
            Rotation += _velocityController.GetAngularVelocity(GlobalTransform, targetPos, -_velocityController.Forward) * (float)delta;
        }else{
            ResetVelocity();
            if(State == TurretState.Reset) State = TurretState.Idle;
        }
    }

    public void UpdateTarget(ITargetable targetable)
    {
        Target = targetable;
        State = TurretState.Tracking;
    }

    public void ResetTarget()
    {
        State = TurretState.Reset;
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
    public override void _Process(double delta)
    {
        switch (State)
        {   
            case TurretState.Tracking:
                UpdateYrotation2(Target.GlobalPosition, delta);
                break;
            case TurretState.Reset:
                Vector3 locallyRotated = Transform.Basis * _velocityController.Forward * 10;

                // Then rotate by parent rotation
                Vector3 worldOffset = TransformParent.GlobalTransform.Basis * locallyRotated;

                Vector3 worldPosition = TransformParent.GlobalPosition + worldOffset;

                UpdateYrotation(worldPosition, delta);
            break;
            default:
                break;
        }
            
    }
}
