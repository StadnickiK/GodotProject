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

    PackedScene TileScene = null;

    [Signal]
    public delegate void SelectTarget(RigidBody target);
   
    public Player PlanetOwner { get; set; } = null;

    public List<Tile> Tiles { get; set; } = new List<Tile>();

    public Random Rand { get; set; } = new Random();

    public string PlanetName { get; set; } = "PlanetName";

    public StarSystem System { get; set; } = null;

    private Spatial _orbit = null;
    public Spatial Orbit
    {
        get { return _orbit; }
    }

    private Timer _timer = null;
    public Timer PlanetTimer
    {
        get { return _timer; }
    }

    public Building Construction { get; set; } = null;
    

    MeshInstance Mesh = null;

    private List<Resource> _resources = new List<Resource>();
    public List<Resource> Resources
    {
        get { return _resources; }
    }

    private List<Resource> _naturalResources = new List<Resource>();
    public List<Resource> NaturalResources
    {
        get { return _resources; }
    }

    private List<Building> _buildings = new List<Building>();
    public List<Building> Buildings
    {
        get { return _buildings; }
    }

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

    void _on_Planet_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(inputEvent is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(OpenPlanetInterface), this);
            break;
          case ButtonList.Right:
            EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
            break;
        }
      } 
    }

    void _on_Timer_timeout(){
        if(Construction != null){
            Construction.CurrentTime++;
            if(Construction.CurrentTime > Construction.BuildTime){
                Construction = null;
            }else{
                _timer.Start(1);
            }
        }
    }

    void GetNodes(){
        Mesh = GetNode<MeshInstance>("MeshInstance");
        _orbit = GetNode<Spatial>("Orbit");
        _timer = GetNode<Timer>("Timer");
    }

    public void ConstructBuilding(Building building){
        GD.Print("Start C");
        Construction = building;
        _timer.Start(1);
    }

    public override void _Ready()
    {
        TileScene = (PackedScene)GD.Load("res://Map/Tile.tscn");
        GetNodes();
        GenerateMesh();
        World w = GetNode<World>("/root/Game/World");
        w.ConnectTo_OpenPlanetInterface(this);
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
        WCC.ConnectToSelectTarget(this);    
        Name = PlanetName;

        for(int i =0; i<5; i++){
            var building = new Building();
            building.Name = "Building "+i;
            Buildings.Add(building);
        }
        //Generate();
    }
}
