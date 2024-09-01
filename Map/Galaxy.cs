using Godot;
using System;
using System.Collections.Generic;

public partial class Galaxy : Node3D
{

    [Export]
    public int StarSystemNumber { get; set; } = 3;

    private int _radius;
    public int Radius
    {
        get { return _radius; }
        set { _radius = value; }
    }

    
    StarSystem _currentSystem = null;

    private List<StarSystem> _starSystems = new List<StarSystem>();
    public List<StarSystem> StarSystems
    {
        get { return _starSystems; }
    }

    public RigidBody3D Ground { get; set; } = null;

    [Signal]
    public delegate void CameraLookAtEventHandler(Vector3 position);

    [Signal]
    public delegate void LookAtStarSystemEventHandler(StarSystem system);

    public enum Type
    {
        Elliptical,
        Spiral,
        Irregular
    }

    int GetBiggestStarSystemRadius(){
        int max = 0;
        foreach(StarSystem system in StarSystems){
            if(system.Radius > max) max = system.Radius;
        }
        return max;
    }

    public void ViewStarSystem(StarSystem system){
        _currentSystem = system;
        foreach(Node3D spatial in GetChildren()){
            if(spatial is StarSystem starSystem){
                if(starSystem.SystemID != system.GetIndex()){
                    starSystem.Visible = false;
                }else{
                    EmitSignal(nameof(CameraLookAtEventHandler), starSystem.Transform.Origin);
                }
            }else{
                spatial.Visible = false;
            }
        }
    }

    void _on_ViewStarSystem(StarSystem system){
        ViewStarSystem(system);
    }

    void _on_ViewGalaxy(StarSystem system){
        EmitSignal(nameof(LookAtStarSystemEventHandler), system);
        foreach(Node3D node in GetChildren()){
            node.Visible = true;
        }
    }


    public void ViewGalaxy(){
        // if(_currentSystem != null){
        //     _currentSystem.CloseSystem();
        //     _currentSystem = null;
        // }
        foreach(Node3D spatial in GetChildren()){
            spatial.Visible = true;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetPhysicsProcess(false);
        SetProcess(false);
    }

    void HideNodes(params Node3D[] Nodes){
        foreach(Node3D spatial in Nodes){
            spatial.Visible = false;
        }
    }
}
