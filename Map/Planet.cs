using Godot;
using System;
using System.Collections.Generic;

public class Planet : RigidBody
{

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    public List<Tile> Tiles { get; set; } = new List<Tile>();

    public Random Rand { get; set; } = new Random();

    public string PlanetName { get; set; }

    PackedScene TileScene = null;
    
    PlanetInterface PInterface = null;

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
        Size = Rand.Next(5,15);
        Scale *= Size/10;
        Size = Size%2==0 ? Size : ++Size;
        for(int i = 0; i<Size;i++){
            Tile t =  (Tile)TileScene.Instance();
            t.Rand = Rand;
            Tiles.Add(t);
        }
    }

    void _on_Planet_input_event(Node camera, InputEvent e,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(e is InputEventMouseButton mouseButton){
            if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Left){
                PInterface.Visible = true;
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TileScene = (PackedScene)GD.Load("res://Map/Tile.tscn");
        PInterface = GetNode<PlanetInterface>("PlanetInterface");
        //Generate();
    }
}
