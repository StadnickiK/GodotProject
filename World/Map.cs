using Godot;
using System;
using System.Linq;

public partial class Map : Node3D
{
    public Galaxy galaxy = null;

    MapObjects mapObj = null;

    //PackedScene _ShipScene = (PackedScene)ResourceLoader.Load("res://Units/Base/Ship.tscn");

    void GetNodes(){
        mapObj = GetNode<MapObjects>("MapObjects");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
    }

    // public void ConnectToShowBattlePanel(Node node, string method){
    //     Connect(nameof(SignalName.ShowBattlePanel), new Callable(node, method));
    // }

    // public void _on_OpenBattlePanel(SpaceBattle battle){
    //     EmitSignal(nameof(SignalName.ShowBattlePanel), battle);
    // }

    public void ConnectToEnterMapObject(Node node){
        node.Connect("SignalEnterMapObject", new Callable(this, nameof(_on_Enter_MapObject)));
    }

    void _on_Enter_MapObject(Node mapObject, Node targetMapObject, Vector3 aproachVec = default(Vector3), PhysicsDirectBodyState3D state = null){
        MoveToMapObject(mapObject, targetMapObject, aproachVec, state);
    }

    void _on_Enter_MapObject(Node mapObject, Node targetMapObject){
        MoveToMapObject(mapObject, targetMapObject);
    }

    void MoveToMapObject(Node mapObject, Node targetMapObject, Vector3 aproachVec = default(Vector3), PhysicsDirectBodyState3D state = null){
        if(targetMapObject is IEnterMapObject enterMapObject){
            enterMapObject.EnterMapObject(mapObject);
        }
    }

    public void ConnectToExitMapObject(Node node){
        if(!node.IsConnected("SignalExitMapObject", new Callable(this, nameof(_on_Exit_MapObject))))
            node.Connect("SignalExitMapObject", new Callable(this, nameof(_on_Exit_MapObject)));
    }

    void _on_Exit_MapObject(Node mapObject, Node parentMapObject, Vector3 exitVec = default(Vector3), PhysicsDirectBodyState3D state = null){
        if(parentMapObject is IExitMapObject exitMapObject){
            exitMapObject.ExitMapObject(mapObject, exitVec, state);
        }
    }
    public void _on_UInfo_ChangeStance(Node node, string stance){
        if(node is Ship ship){

        }
    }

    public StarSystem GetClosestStarSystem(StarSystem system){
        var children = galaxy.GetChildren();
        
        StarSystem target = null;
        foreach(Node node in children){
            if(node is StarSystem starSystem){
                if(target == null){
                    target = starSystem;
                }else{
                    if(system.Transform.Origin.DistanceTo(target.Transform.Origin) > system.Transform.Origin.DistanceTo(starSystem.Transform.Origin))
                        target = starSystem;
                }
            }
        }
        return target;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
