using Godot;
using System;

public partial class SimpleFireControl : Node
{
    public override void _Ready()
    {
        
    }

    public void Start(){
        foreach(Node node in GetChildren()){
            if(node is Node3D fire){
                fire.GetNode<GpuParticles3D>("Flame").Emitting = true;
            }
        }
    }

    public void Stop(){
        foreach(Node node in GetChildren()){
            if(node is Node3D fire){
                fire.GetNode<GpuParticles3D>("Flame").Emitting = false;
            }
        }
    }
}
