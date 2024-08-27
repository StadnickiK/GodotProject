using Godot;
using System;

public partial class Barrel : CharacterBody3D
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
    CharacterBody3D ktargetPos;
    RigidBody3D targetPos;
    PackedScene scene = (PackedScene)ResourceLoader.Load("res://RBullet.tscn");

    RayCast3D ray;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
            z = Position.z;
            ray = GetNode<RayCast3D>("RayCast3D");
    }

    void ShootAnim(float delta){
        if(ammo != 0){
            if(Position.z < -3){
                direction = true;
                var bullet = (RBullet)scene.Instance();
                bullet.Parent = GetParent();
                //GD.Print(Scale);
                Vector3 tDir = Transform3D.origin.DirectionTo(targetPos.GlobalTransform.origin);
                //GD.Print(targetPos.LinearVelocity.Normalized());
                //GD.Print(targetPos.GlobalTransform.origin);

                Vector3 bulletDirection = tDir*100;//GlobalTransform.origin; //+ Transform3D.origin;
                //GD.Print(GlobalTransform.origin);
                //GD.Print(Transform.origin);
                //bulletDirection.y = 0; 
                bullet.Launch(bulletDirection);
                GetNode("/root/World").AddChild(bullet);
                //GetParent().AddChild(bullet);
            }else if(Position.z > -2.4f){
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
        targetPos = GetNode<RigidBody3D>("/root/World/Target");
        ktargetPos = GetNode<CharacterBody3D>("/root/World/KTarget");
        //LookAt(ktargetPosition.GlobalTransform.origin,upDir);
        //LookAt(targetPos.GlobalTransform.origin,upDir);
        
        //ray.LookAt(targetPos.GlobalTransform.origin,upDir);
        //GD.Print(delta);
        if(ray.IsColliding()){
            ShootAnim(delta);
        }
  }
}
