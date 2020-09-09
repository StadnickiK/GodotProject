using Godot;
using System;
using System.Collections.Generic;

public class StarSystem : Spatial
{

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    public String SystemName { get; set; }

    public Text3D SystemName3D { get; set; }

    PackedScene SunScene = null;
    PackedScene PlanetScene = null;

    List<Planet> Planets = new List<Planet>();

    public Random Rand { get; set; } = new Random();

    void LoadScenes(){
        SunScene = (PackedScene)GD.Load("res://Map/Star.tscn");
        PlanetScene = (PackedScene)GD.Load("res://Map/Planet.tscn");
    }

    void Generate(){
        var sun = SunScene.Instance();
        AddChild(sun);

        SystemName3D.UpdateText(SystemName);

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
            AddChild(planet);
            dist += Rand.Next(4, 10);
            angle += Rand.Next(0,50);
        }
        Rotation = Vector3.Zero;
    }
    public enum Type
    {
        Capitol,
        Core,
        Strategic,
        Colony
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadScenes();
        SystemName3D = (Text3D)GetNode("Text3D");
        Generate();
    }

}
