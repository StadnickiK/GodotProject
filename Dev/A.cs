using Godot;
using System;

public partial class A : MeshInstance3D
{

    [Export]
    public PackedScene projectileScene = (PackedScene)ResourceLoader.Load("res://Units/Base/RBullet.tscn");

    MovingTarget target = null;

    double time;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        target = GetNode<MovingTarget>("/root/TestScene/KB");
    }

    protected Vector3 DirToTarget(){
        return GlobalTransform.Origin.DirectionTo(target.GlobalTransform.Origin);
    }

    protected void Shoot(){
        var projectile = (RBullet)projectileScene.Instantiate();
        var transform = projectile.Transform;
        var dir = DirToTarget();

        transform.Origin = GlobalTransform.Origin; //+ 1.2f*Scale.z*dir;
        //transform.Rotated(new Vector3(0,0,1), 30);
        projectile.TargetPos = target;
        projectile.Transform = transform;
        projectile.Scale = new Vector3(3, 3, 3);
        GetParent().AddChild(projectile);
        projectile.Launch(dir);
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(double delta)
 {
    time += delta;
    if(time >= 1){
        Shoot();
        time = 0;
    }
 }
}
