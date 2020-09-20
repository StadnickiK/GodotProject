using Godot;
using System;

public class ExplosionParticles : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public Particles Particle { get; set; }

    // Called when the node enters the scene tree for the first time.

    public void SetPosition(Vector3 position){
        var t = Transform;
        t.origin = position;
        Transform = t;
    }

    public void SetScale(){
        
    }

    public override void _Ready()
    {
        Particle = GetNode<Particles>("Particles");
        Particle.Emitting = true;
    }

    public void _on_Timer_timeout(){
        QueueFree();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
