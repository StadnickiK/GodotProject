using Godot;
using System;
using System.Collections.Generic;

public class StarSystem : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    PackedScene SunScene = null;
    PackedScene PlanetScene = null;

    List<Planet> Planets = new List<Planet>();

    void LoadScenes(){
        SunScene = (PackedScene)GD.Load("res://Map/Star.tscn");
        PlanetScene = (PackedScene)GD.Load("res://Map/Planet.tscn");
    }

    void Generate(){
        var sun = SunScene.Instance();
        AddChild(sun);

        int dist = 5;
        float angle = 45f;
        for(int i = 0;i < Size; i++){
            RotateY(angle);
            var pos = Transform.basis.Xform(new Vector3(0, 0, dist));
            GD.Print(pos);
            var planet = (Planet)PlanetScene.Instance();
            var temp = planet.GlobalTransform;
            temp.origin = pos;
            planet.GlobalTransform = temp;
            AddChild(planet);
            dist += 4;
            angle+= 23f;
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
        Generate();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
