using Godot;
using System;
using System.Collections.Generic;

public class StarSystem : StaticBody
{

    [Export]
    int Size = 10;


    [Export]
    int Wealth = 5;

    [Signal]
    public delegate void ViewStarSystem(int id);

    [Signal]
    public delegate void ViewGalaxy();

    [Signal]
    public delegate void SelectTarget(StarSystem target);

    private int _diameter;
    public int Diameter
    {
        get { return _diameter; }
    }

    MeshInstance _size = null;

    public String SystemName { get; set; }

    public Text3D SystemName3D { get; set; }

    public int SystemID { get; set; }
    // Node storing objects in the star system
    Spatial StarSysObjects = null;
    CollisionShape Placeholder = null;
    Button XButton = null;

    PackedScene SunScene = null;
    PackedScene PlanetScene = null;

    private List<Planet> _planets = new List<Planet>();
    public List<Planet> Planets
    {
        get { return _planets; }
    }

    public Random Rand { get; set; } = new Random();

    void LoadScenes(){
        SunScene = (PackedScene)GD.Load("res://Map/Star.tscn");
        PlanetScene = (PackedScene)GD.Load("res://Map/Planet.tscn");
    }

    void GetNodes(){
        StarSysObjects = GetNode<Spatial>("StarSysObjects");
        SystemName3D = GetNode<Text3D>("Text3D");
        Placeholder = GetNode<CollisionShape>("Placeholder");
        XButton = GetNode<Button>("XButton");
        _size = GetNode<MeshInstance>("StarSysObjects/Diameter");
    }

    void Generate(){
        var sun = SunScene.Instance();
        StarSysObjects.AddChild(sun);

        SystemName3D.UpdateText(SystemName);
        int dist = Rand.Next(5, 15);
        float angle = Rand.Next(0, 70);
        for(int i = 0;i < Size; i++){
            RotateY(angle);
            var pos = Transform.basis.Xform(new Vector3(0, 0, dist));
            var planet = (Planet)PlanetScene.Instance();
            planet.PlanetName = SystemName +" "+ i;
            planet.Rand = Rand;
            planet.System = this;
            var temp = planet.Transform;
            temp.origin = pos;
            planet.Transform = temp;
            StarSysObjects.AddChild(planet);
            _planets.Add(planet);
            dist += Rand.Next(4, 10);
            angle += Rand.Next(0,50);
        }
        _diameter = (int)(dist*1.2f);
        StarSysObjects.Visible = false;
        Rotation = Vector3.Zero;
    }
    public enum Type
    {
        Capitol,
        Core,
        Strategic,
        Colony
    }

    public void OpenStarSystem(){
        StarSysObjects.Visible = true;
        XButton.Visible = true;
        Placeholder.Visible = false;
        EmitSignal(nameof(ViewStarSystem), SystemID);
    }

    void _on_StarSystem_input_event(Node camera, InputEvent e,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(e is InputEventMouseButton mouseButton){
            if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Left){
                OpenStarSystem();
            }else if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Right){
                EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
            }
        }
    }

    void _on_XButton_button_up(){
        StarSysObjects.Visible = false;
        XButton.Visible = false;
        Placeholder.Visible = true;
        EmitSignal(nameof(ViewGalaxy));
    }

    protected void _ConnectSignal(){
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
        WCC.ConnectToSelectTarget(this);
        //control.Connect("_SelectTarget", this, nameof(SelectTarget));
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetPhysicsProcess(false);
        LoadScenes();
        GetNodes();
        Generate();
        _size.Scale = new Vector3(_diameter,1,_diameter);
        _ConnectSignal();
    }

}
