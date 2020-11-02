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
    
    protected StarSystem self = null;

    public String SystemName { get; set; }

    public Text3D SystemName3D { get; set; }

    public int SystemID { get; set; }
    // Node storing objects in the star system
    Spatial StarSysObjects = null;
    CollisionShape Placeholder = null;
    Button XButton = null;

    PackedScene SunScene = null;
    PackedScene PlanetScene = null;

    List<Planet> Planets = new List<Planet>();


    public Random Rand { get; set; } = new Random();

    void LoadScenes(){
        SunScene = (PackedScene)GD.Load("res://Map/Star.tscn");
        PlanetScene = (PackedScene)GD.Load("res://Map/Planet.tscn");
    }

    void LoadNodes(){
        StarSysObjects = GetNode<Spatial>("StarSysObjects");
        SystemName3D = GetNode<Text3D>("Text3D");
        Placeholder = GetNode<CollisionShape>("Placeholder");
        XButton = GetNode<Button>("XButton");
    }

    void Generate(){
        var sun = SunScene.Instance();
        StarSysObjects.AddChild(sun);

        SystemName3D.UpdateText(SystemName);
        GD.Print(SystemName);
        int dist = Rand.Next(5, 15);
        float angle = Rand.Next(0, 70);
        for(int i = 0;i < Size; i++){
            RotateY(angle);
            var pos = Transform.basis.Xform(new Vector3(0, 0, dist));
            var planet = (Planet)PlanetScene.Instance();
            planet.PlanetName = SystemName + i;
            planet.Rand = Rand;
            var temp = planet.Transform;
            temp.origin = pos;
            planet.Transform = temp;
            StarSysObjects.AddChild(planet);
            dist += Rand.Next(4, 10);
            angle += Rand.Next(0,50);
        }
        _diameter = dist*2-Rand.Next(20,40);
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

    void _on_StarSystem_input_event(Node camera, InputEvent e,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(e is InputEventMouseButton mouseButton){
            if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Left){
                StarSysObjects.Visible = true;
                XButton.Visible = true;
                Placeholder.Visible = false;
                EmitSignal(nameof(ViewStarSystem), SystemID);
            }else if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Right){
                EmitSignal(nameof(SelectTarget), (PhysicsBody)self);
            }
        }
    }

    void _on_XButton_button_up(){
        StarSysObjects.Visible = false;
        XButton.Visible = false;
        Placeholder.Visible = true;
        EmitSignal(nameof(ViewGalaxy));
        GD.Print(SystemName);
    }

    protected void _ConnectSignal(){
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/World/WorldCursorControl");
        WCC.ConnectToSelectTarget(self);
        //control.Connect("_SelectTarget", this, nameof(SelectTarget));
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        self = (StarSystem)GetParent().GetChild(GetIndex());
        SetPhysicsProcess(false);
        LoadScenes();
        LoadNodes();
        Generate();
        _ConnectSignal();
    }

}
