using Godot;
using System;

public partial class Star : Node3D
{
    [Export]
    int Radius = 10;

    [Export]
    int Height = 18;

    public Random Rand { get; set; } = new Random();

    public MeshInstance3D Mesh { get; set; } = null;  
    public override void _Ready()
    {
        
        Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        var sphere = (SphereMesh)Mesh.Mesh;
        sphere.Radius = Radius;
        sphere.Height = Height;
        //Mesh.Mesh = sphere;
        SetProcess(false);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
