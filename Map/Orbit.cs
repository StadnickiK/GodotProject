using Godot;
using System;

public class Orbit : Spatial
{

    public delegate void StartSiege(Node node);

    public bool OrbitChanged { get; set; } = false;
    
    public override void _Ready()
    {
        
    }

    public void AddNode(Node node){
        AddChild(node);
    }

    public void ConnectToStartSiege(Node node, string methodName){
        Connect(nameof(StartSiege), node, methodName);
    }

    public void Colonize(Node node){
        if(node is Ship ship){
            if(GetParent() is Planet planet){
                switch(planet.PlanetStatus){
                    case Planet.Status.None:
                    planet.PlanetOwner = ship.ShipOwner;
                        break;
                    default:
                        return;
                }
            }
        }
    }

    public void Conquer(Node node){
        if(node is Ship ship){
            if(GetParent() is Planet planet){
                switch(planet.PlanetStatus){
                    case Planet.Status.Occupied:
                    planet.PlanetOwner = ship.ShipOwner;
                        break;
                    default:
                        return;
                }
            }
        }
    }

    public bool CanSiege(Node node){
        if(node is Ship ship){

        }
        return false;
    }

    public Ship GetLocal(){
        foreach(Node node in GetChildren()){
            if(node is Ship ship){
                if(ship.IsLocal)
                    return ship;
            }
        }
        return null;
    }

    public bool HasLocal(){
        foreach(Node node in GetChildren()){
            if(node is Ship ship){
                if(ship.IsLocal)
                    return true;
            }
            if(node is SpaceBattle battle)
                if(battle.IsLocal)
                    return true;
        }
        return false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}