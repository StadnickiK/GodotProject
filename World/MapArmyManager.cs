using Godot;
using System;
using System.Collections.Generic;

public partial class MapArmyManager : Node
{
    public Node3DPool ArmyPool { get; set; }

    public Random Rand { get; set; }

    public override void _Ready()
    {
        ArmyPool = GetNode<Node3DPool>("ArmyPool");
    }

    public Ship CreateShip(Planet planet, Vector3 position, string name){
		var ship = (Ship)ArmyPool.GetNode3D(planet.System.StarSysObjects, position, name);
		UpdateController(ship, planet.Controller);
		return ship;
	}

    public Ship CreateShip(Planet planet, Unit unit){
		var ship = (Ship)ArmyPool.GetNode3D(planet.System.StarSysObjects, planet.Transform.Origin+ new Vector3(3,0,3), planet.Name +" "+Rand.Next(0,1000));
		AddUnit(ship, unit);
		ship.Units.AddChild(unit);
		//ConnectShip(ship);
        UpdateController(ship, planet.Controller);
		//  planet.AddToOrbit(ship);
		return ship;
	}

    public Ship CreateShip(ShipStruct shipStruct){
		var ship = (Ship)ArmyPool.GetNode3D(shipStruct.Parent, shipStruct.Position, shipStruct.Name);
        ship.Visible = shipStruct.Visible;
		AddUnits(ship, shipStruct.Units);
        UpdateController(ship, shipStruct.Controller);
        UpdateVisibility(ship, shipStruct.PlayerVisibility, shipStruct.Visible);
        UpdateProcessing(ship, ProcessModeEnum.Inherit);
        ship.MoveToTarget(shipStruct.Target);
		return ship;
	}

    public void FreeShip(Ship ship){
        RemoveController(ship);
        RemoveUnits(ship);
        ClearVisibility(ship);
        UpdateProcessing(ship, ProcessModeEnum.Disabled);
    }

    void AddUnits(Ship ship, List<Unit> Units){
            if(Units != null){
            foreach(var unit in Units){
                var parent = unit.GetParent();
                if(parent != null)
                    parent.RemoveChild(unit);
                ship.Units.AddUnit(unit);
            }
        }
    }

    void AddUnit(Ship ship, Unit unit){
            if(unit != null){
                var parent = unit.GetParent();
                if(parent != null)
                    parent.RemoveChild(unit);
                ship.Units.AddUnit(unit);
        }
    }

    void RemoveUnits(Ship ship){
        if(ship.Units.Count > 0){
            ship.Units.RemoveUnit();
        }
    }

    void UpdateController(Ship ship, Player Controller){
        if(Controller != null)
		    Controller.AddMapObject(ship);
    }

    void RemoveController(Ship ship){
        if(ship.Controller != null)
		    ship.Controller.RemoveMapObject(ship);
    }

    void UpdateVisibility(Ship ship, Dictionary<int, VisibilityConroller.VisibilityStruct> visibilityMap, bool visible) {
        if(visibilityMap != null){
            ship.VisibilityConroller.PlayerVisibility = new Dictionary<int, VisibilityConroller.VisibilityStruct>(visibilityMap);
        }
        ship.Visible = visible;
    }

    void ClearVisibility(Ship ship) {
        ship.VisibilityConroller.PlayerVisibility.Clear();
        ship.Visible = false;       
    }

    void UpdateProcessing(Ship ship, ProcessModeEnum processing){
        ship.ProcessMode = processing;
    }

}
