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
        set { _radius = value; }
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

    [Signal]
    public delegate void LookAtStarSystem(StarSystem system);

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

    void _on_ViewGalaxy(StarSystem system){
        EmitSignal(nameof(LookAtStarSystem), system);
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetPhysicsProcess(false);
        SetProcess(false);
    }

    void HideNodes(params Spatial[] Nodes){
        foreach(Spatial spatial in Nodes){
            spatial.Visible = false;
        }
    }
}
