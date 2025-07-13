using Godot;
using System;

public partial class MenuPlanet : StaticBody3D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    Gradient gradient = null;

    Random Rand = new Random();

    MeshInstance3D Mesh = null;


    void GenerateMesh(){
        ShaderMaterial material = (ShaderMaterial)Mesh.GetSurfaceOverrideMaterial(0);
        //ShaderMaterial material = new ShaderMaterial();
        var noise = new NoiseTexture3D();
        // noise = (NoiseTexture3D)material.GetShaderParameter("noise");//new FastNoiseLite();
        //var tempGradient = (GradientTexture2D)material.GetShaderParameter("gradient");
        //tempGradient.Gradient = gradient;
        //noise.Noise.Seed = Rand.Next(-1000,1000);
        //GradientTexture texture = new GradientTexture();
        //texture.Gradient = gradient;
        //material.SetShaderParameter("noise", noise);
        //material.SetShaderParameter("gradient", tempGradient);
        //Mesh.MaterialOverride = material;
        //Mesh.SetSurfaceMaterial(0, material);
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        GenerateMesh();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
