using Godot;
using System;

public class Barrel : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    float z;
    bool direction = true;
    float reloadTime = 1;
    float timePassed = 0;
    int max_ammo = 600;
    int ammo = 0;
    KinematicBody ktargetPos;
    RigidBody targetPos;
    PackedScene scene = (PackedScene)ResourceLoader.Load("res://RBullet.tscn");

    RayCast ray;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
            z = Translation.z;
            ray = GetNode<RayCast>("RayCast");
    }

    void ShootAnim(float delta){
        if(ammo != 0){
            if(Translation.z < -3){
                direction = true;
                var bullet = (RBullet)scene.Instance();
                bullet.Parent = GetParent();
                //GD.Print(Scale);
                Vector3 tDir = Transform.origin.DirectionTo(targetPos.GlobalTransform.origin);
                //GD.Print(targetPos.LinearVelocity.Normalized());
                //GD.Print(targetPos.GlobalTransform.origin);

                Vector3 bulletDirection = tDir*100;//GlobalTransform.origin; //+ Transform.origin;
                //GD.Print(GlobalTransform.origin);
                //GD.Print(Transform.origin);
                //bulletDirection.y = 0; 
                bullet.Launch(bulletDirection);
                GetNode("/root/World").AddChild(bullet);
                //GetParent().AddChild(bullet);
            }else if(Translation.z > -2.4f){
                direction = false;
            }
            if(direction){
                Translate(new Vector3(0,0,0.1f));
            }else{
                Translate(new Vector3(0,0,-0.1f));
            }
            ammo--;
        }else if(timePassed > reloadTime){
            ammo = max_ammo;
            timePassed = 0;
        }else{
            timePassed += delta;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
        var upDir = new Vector3(1, 0, 0);
        targetPos = GetNode<RigidBody>("/root/World/Target");
        ktargetPos = GetNode<KinematicBody>("/root/World/KTarget");
        //LookAt(ktargetPosition.GlobalTransform.origin,upDir);
        //LookAt(targetPos.GlobalTransform.origin,upDir);
        
        //ray.LookAt(targetPos.GlobalTransform.origin,upDir);
        //GD.Print(delta);
        if(ray.IsColliding()){
            ShootAnim(delta);
        }
  }
}
