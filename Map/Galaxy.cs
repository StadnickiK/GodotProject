using Godot;
using System;
using System.Collections.Generic;

public class Galaxy : Spatial
{

    [Export]
    int Size = 40;

    public List<StarSystem> StarSystems { get; set; } = new List<StarSystem>();

    [Signal]
    public delegate void CameraLookAt(Vector3 position);

    PackedScene StarSystemScene = null;

    public Random Rand { get; set; }

    int Seed = 0;

    public enum Type
    {
        Elliptical,
        Spiral,
        Irregular
    }

    void InitRand(){
        Seed = Guid.NewGuid().GetHashCode();
        Rand = new Random(Seed);
;    }
    void Generate(){

        int dist = Rand.Next(10, 20);
        float angle = Rand.Next(0, 70);
        for(int i = 0;i < Size; i++){
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
            StarSystems.Add(starSystem);
            dist += Rand.Next(6, 10);
            angle += Rand.Next(0, 50);
        }
        Rotation = Vector3.Zero;
    }

    void _on_ViewStarSystem(int id){
        foreach(StarSystem s in StarSystems){
            if(s.SystemID != id){
                s.Visible = false;
            }else{
                EmitSignal(nameof(CameraLookAt), s.Transform.origin);
            }
        }
    }

    void _on_ViewGalaxy(){
        foreach(StarSystem s in StarSystems){
            s.Visible = true;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StarSystemScene = (PackedScene)GD.Load("res://Map/StarSystem.tscn");
        InitRand();
        Generate();
    }

}
