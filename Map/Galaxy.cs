using Godot;
using System;
using System.Collections.Generic;

public class Galaxy : Spatial
{

    [Export]
    int Size = 40;

    public List<StarSystem> StarSystems { get; set; }

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
            starSystem.SystemName = "System " + i;
            var temp = starSystem.Transform;
            temp.origin = pos;
            starSystem.Transform = temp;
            AddChild(starSystem);
            dist += Rand.Next(300, 500);
            angle += Rand.Next(0, 50);
        }
        Rotation = Vector3.Zero;
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StarSystemScene = (PackedScene)GD.Load("res://Map/StarSystem.tscn");
        InitRand();
        Generate();
    }

}
