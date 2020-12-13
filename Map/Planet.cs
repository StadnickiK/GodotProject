using Godot;
using System;
using System.Collections.Generic;

public class Planet : StaticBody
{

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    [Export]
    Gradient gradient = null;
   
    public Player PlanetOwner { get; set; } = null;

    public List<Tile> Tiles { get; set; } = new List<Tile>();

    public Random Rand { get; set; } = new Random();

    public string PlanetName { get; set; } = "PlanetName";

    public StarSystem System { get; set; } = null;

    PackedScene TileScene = null;

    MeshInstance Mesh = null;

    [Signal]
    public delegate void OpenPlanetInterface(Planet planet);

    public enum Type
    {
        Balanced,
        Aggro,
        MegaCity,
        Colony,
        Mine,
        Industrial,
        HeavyIndustrial,
        HellIndustrial
    }

    void Generate(){
        Name = PlanetName;
        Size = Rand.Next(5,15);
        Scale *= Size/10;
        Size = Size%2==0 ? Size : ++Size;
        for(int i = 0; i<Size;i++){
            Tile t =  (Tile)TileScene.Instance();
            t.Rand = Rand;
            Tiles.Add(t);
        }
    }

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

    void _on_Planet_input_event(Node camera, InputEvent e,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(e is InputEventMouseButton mouseButton){
            if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Left){
                EmitSignal(nameof(OpenPlanetInterface), this);
            }
        }
    }

    void GetNodes(){
        Mesh = GetNode<MeshInstance>("MeshInstance");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TileScene = (PackedScene)GD.Load("res://Map/Tile.tscn");
        GetNodes();
        GenerateMesh();
        World w = GetNode<World>("/root/Game/World");
        w.ConnectTo_OpenPlanetInterface(this);
        //Generate();
    }
}
