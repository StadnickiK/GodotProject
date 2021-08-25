using Godot;
using System;

public class SimpleFireControl : Node
{
    public override void _Ready()
    {
        
    }

    public void Start(){
        foreach(Node node in GetChildren()){
            if(node is Spatial fire){
                fire.GetNode<Particles>("Flame").Emitting = true;
            }
        }
    }

    public void Stop(){
        foreach(Node node in GetChildren()){
            if(node is Spatial fire){
                fire.GetNode<Particles>("Flame").Emitting = false;
            }
        }
    }
}
