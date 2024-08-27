using Godot;
using System;

public partial class Star : Node3D
{
    public MeshInstance3D Mesh { get; set; } = null;  
    public override void _Ready()
    {
        Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        SetProcess(false);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
