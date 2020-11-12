using Godot;
using System;

public class MenuPlanet : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    Gradient gradient = null;

    Random Rand = new Random();

    MeshInstance Mesh = null;


    void GenerateMesh(){
        ShaderMaterial material = (ShaderMaterial)Mesh.GetSurfaceMaterial(0);
        //ShaderMaterial material = new ShaderMaterial();
        NoiseTexture noise = new NoiseTexture();
        noise = (NoiseTexture)material.GetShaderParam("noise");//new OpenSimplexNoise();
        var tempGradient = (GradientTexture)material.GetShaderParam("gradient");
        tempGradient.Gradient = gradient;
        noise.Noise.Seed = Rand.Next(-1000,1000);
        //GradientTexture texture = new GradientTexture();
        //texture.Gradient = gradient;
        material.SetShaderParam("noise", noise);
        material.SetShaderParam("gradient", tempGradient);
        //Mesh.MaterialOverride = material;
        //Mesh.SetSurfaceMaterial(0, material);
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Mesh = GetNode<MeshInstance>("MeshInstance");
        GenerateMesh();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
