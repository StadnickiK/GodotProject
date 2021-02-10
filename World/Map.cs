using Godot;
using System;

public class Map : Spatial
{
    public Galaxy galaxy = null;

    MapObjects mapObj = null;
    Combat combat;

    [Signal]
    public delegate void ShowBattlePanel(SpaceBattle battle);

    PackedScene _ShipScene = (PackedScene)ResourceLoader.Load("res://Units/Base/Ship.tscn");

    void GetNodes(){
        mapObj = GetNode<MapObjects>("MapObjects");
        combat = GetNode<Combat>("Combat");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
    }

    public void ConnectToEnterCombat(Node node){
         node.Connect("EnterCombat", this, nameof(_on_EnterCombat));
    }

    void _on_EnterCombat(PhysicsBody ship, PhysicsBody enemy, Node parent){
        if(ship != null && enemy != null){
            if(ship != enemy){
                if(!combat.Combatants.Contains(ship) && !combat.Combatants.Contains(enemy)){
                    combat.Combatants.Add(ship);
                    combat.Combatants.Add(enemy);
                    var battle = combat.CreateBattle(ship, enemy,parent);
                    battle.ConnectToOpenBattlePanel(this, nameof(_on_OpenBattlePanel));
                }
            }
        }
    }

    public void ConnectToShowBattlePanel(Node node, string method){
        Connect(nameof(ShowBattlePanel), node, method);
    }

    public void _on_OpenBattlePanel(SpaceBattle battle){
        EmitSignal(nameof(ShowBattlePanel), battle);
    }

    public void ConnectToEnterMapObject(Node node){
        node.Connect("SignalEnterMapObject", this, nameof(_on_Enter_MapObject));
    }

    void _on_Enter_MapObject(Node mapObject, Node targetMapObject, Vector3 aproachVec = default(Vector3), PhysicsDirectBodyState state = null){
        MoveToMapObject(mapObject, targetMapObject, aproachVec, state);
    }

    void MoveToMapObject(Node mapObject, Node targetMapObject, Vector3 aproachVec = default(Vector3), PhysicsDirectBodyState state = null){
        if(targetMapObject is IEnterMapObject enterMapObject){
            enterMapObject.EnterMapObject(mapObject, aproachVec, state);
        }
    }

    public void ConnectToLeaveSystem(Node node){
         node.Connect("LeaveSystem", this, nameof(_on_Ship_LeaveSystem));
    }

    void _on_Ship_LeaveSystem(Ship ship, Vector3 currentVec, PhysicsDirectBodyState state){
        ShipLeaveSystem(ship, currentVec, state);
    }

    void ShipLeaveSystem(Ship ship, Vector3 currentVec, PhysicsDirectBodyState state){
        if(ship.System != null && ship != null){
            ship.GetParent().RemoveChild(ship);
            galaxy.AddChild(ship);
            var trans = state.Transform;
            trans.origin = ship.System.Transform.origin;
            state.Transform = trans;
            ship.NextTarget();
            ship.System = null;
            ship.Visible = false;
        }
    }

    public void ConnectToLeavePlanet(Node node){
        node.Connect("LeavePlanet", this, nameof(_on_Ship_LeavePlanet));
    }


    void _on_Ship_LeavePlanet(Ship ship){
        if(ship != null){
            ship._Planet.RemoveFromOrbit(ship);
            ship._Planet = null;
            ship.Visible = true;
        }
    }

    public void _on_UInfo_ChangeStance(Node node, string stance){
        if(node is Ship ship){
            if(ship._Planet != null){
                if(ship._Planet.PlanetOwner == null){
                    //ship._Planet.ChangePlanetOwner(ship.ShipOwner);
                }
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
