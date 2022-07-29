using Godot;
using System;

public class Star : Spatial
{
    public MeshInstance Mesh { get; set; } = null;  
    public override void _Ready()
    {
        Mesh = GetNode<MeshInstance>("MeshInstance");
        SetProcess(false);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
