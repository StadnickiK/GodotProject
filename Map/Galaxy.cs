using Godot;
using System;
using System.Collections.Generic;

public class Galaxy : Spatial
{

    [Export]
    public int StarSystemNumber { get; set; } = 3;

    private int _radius;
    public int Radius
    {
        get { return _radius; }
    }
    
    StarSystem _currentSystem = null;

    private List<StarSystem> _starSystems = new List<StarSystem>();
    public List<StarSystem> StarSystems
    {
        get { return _starSystems; }
    }

    public RigidBody Ground { get; set; } = null;

    [Signal]
    public delegate void CameraLookAt(Vector3 position);

    PackedScene StarSystemScene = null;

    public Random Rand { get; set; }

    public enum Type
    {
        Elliptical,
        Spiral,
        Irregular
    }


    void Generate(){

        int dist = Rand.Next(10, 20);
        float angle = Rand.Next(0, 70);
        for(int i = 0;i < StarSystemNumber; i++){
            RotateY(angle);
            var pos = Transform.basis.Xform(new Vector3(0, 0, dist));
            var starSystem = (StarSystem)StarSystemScene.Instance();
            starSystem.Rand = Rand;
            starSystem.SystemID = i;
            starSystem.SystemName = "System " + i;
            starSystem.Connect("ViewStarSystem", this, nameof(_on_ViewStarSystem));
            starSystem.Connect("ViewGalaxy", this, nameof(_on_ViewGalaxy));
            var temp = starSystem.Transform;
            temp.origin = pos;
            starSystem.Transform = temp;
            AddChild(starSystem);
            _starSystems.Add(starSystem);
            dist += Rand.Next(3, 8);
            angle += Rand.Next(0, 60);
        }
        _radius = (int)(1.2f*dist);
        var biggestRadius = GetBiggestStarSystemRAdius();
        if(_radius < biggestRadius){
            _radius = (int)(biggestRadius*1.5f);
        }
        Rotation = Vector3.Zero;
    }

    int GetBiggestStarSystemRAdius(){
        int max = 0;
        foreach(StarSystem system in StarSystems){
            if(system.Radius > max) max = system.Radius;
        }
        return max;
    }

    public void ViewStarSystem(StarSystem system){
        _currentSystem = system;
        foreach(Spatial spatial in GetChildren()){
            if(spatial is StarSystem starSystem){
                if(starSystem.SystemID != system.GetIndex()){
                    starSystem.Visible = false;
                }else{
                    EmitSignal(nameof(CameraLookAt), starSystem.Transform.origin);
                }
            }else{
                spatial.Visible = false;
            }
        }
    }

    void _on_ViewStarSystem(StarSystem system){
        ViewStarSystem(system);
    }

    void _on_ViewGalaxy(){
        foreach(Spatial node in GetChildren()){
            node.Visible = true;
        }
    }

    public void ViewGalaxy(){
        if(_currentSystem != null){
            _currentSystem.CloseSystem();
            _currentSystem = null;
        }
        foreach(Spatial spatial in GetChildren()){
            spatial.Visible = true;
        }
    }

    void GetNodes(){
        //Ground = (RigidBody)GetNode("Ground");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StarSystemScene = (PackedScene)GD.Load("res://Map/StarSystem.tscn");
        GetNodes();
        Generate();
    }

    void HideNodes(params Spatial[] Nodes){
        foreach(Spatial spatial in Nodes){
            spatial.Visible = false;
        }
    }
}
