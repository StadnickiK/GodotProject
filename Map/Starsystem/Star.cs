using Godot;
using System;

public partial class Star : Node3D
{
    [Export]
    int Size = 50;

    public Random Rand { get; set; } = new Random();

    public MeshInstance3D Mesh { get; set; } = null;  
    public override void _Ready()
    {
        Size = Rand.Next(6,10);
        Scale *= Size;
        Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        SetProcess(false);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
