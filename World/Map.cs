using Godot;
using System;
using System.Collections.Generic;
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

    public void InitMap(IWorld world, IGameScene scene)
    {
        InitGalaxy(world, scene);
        InitResistance();
        ConnectPlanets();
    }

    void InitGalaxy(IWorld world, IGameScene scene)
	{
		var generator = new Generator();
		generator.InitGenerator(scene, world.Rand, world.WorldGenParameters.WorldGenParameters);
		galaxy = generator.GenerateGalaxy();
		generator.QueueFree();
		AddChild(galaxy);
		galaxy.Connect("CameraLookAt", new Callable(this, nameof(world.Camera3D.LookAt)));
		//galaxy.Connect("LookAtStarSystem", new Callable(this, nameof(world._on_LookAtStarSystem)));
	}

    void InitResistance()
	{
		foreach (StarSystem system in galaxy.StarSystems)
		{
			foreach (Node node in system.StarSysObjects.GetChildren())
			{
				if (node is Planet planet)
				{
					if (planet.Controller == null)
					{
						// int amount = 2;//Rand.Next(10, 20);
						// var unitFileName = _data.GetNode<Unit>("Units/Unit 1").SceneFilePath;
						//var ship = CreateShip((Unit)((PackedScene)GD.Load(unitFileName)).Instantiate());
						// for(int i = 0;i<amount;i++){
						// 	var unit = ((PackedScene)GD.Load(unitFileName)).Instantiate();
						// 	var stat = unit.GetNode<BaseStat>("Stats/Attack");
						// 	ship.Units.AddChild(unit);
						// }
						// planet.AddToOrbit(ship);
						//var transform = ship.Transform;
					}
				}
			}
		}
	}

    void ConnectPlanets()
	{
		foreach (StarSystem system in galaxy.StarSystems)
		{
			foreach (Planet planet in system.Planets)
			{
				//planet.Connect("GameAlert", new Callable(this, nameof(_on_Alert)));
			}
		}
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
