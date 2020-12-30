using Godot;
using System;

public class Map : Node
{
    public Galaxy galaxy = null;

    MapObjects mapObj = null;
    Combat combat;

    [Signal]
    public delegate void ShowBattlePanel(SpaceBattle battle);

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
            if(!combat.Combatants.Contains(ship) && !combat.Combatants.Contains(enemy)){
                combat.Combatants.Add(ship);
                combat.Combatants.Add(enemy);
                var battle = combat.CreateBattle(ship, enemy,parent);
                battle.ConnectToOpenBattlePanel(this, nameof(_on_OpenBattlePanel));
            }
        }
    }

    public void ConnectToShowBattlePanel(Node node, string method){
        Connect(nameof(ShowBattlePanel), node, method);
    }

    public void _on_OpenBattlePanel(SpaceBattle battle){
        EmitSignal(nameof(ShowBattlePanel), battle);
    }

    public void ConnectToEnterSystem(Node node){
        node.Connect("EnterSystem", this, nameof(_on_Ship_EnterSystem));
    }

    void _on_Ship_EnterSystem(Ship ship, StarSystem System, Vector3 aproachVec, PhysicsDirectBodyState state){
        MoveShipToSystem(ship,System, aproachVec, state);
    }

    void MoveShipToSystem(Ship ship, StarSystem system, Vector3 aproachVec, PhysicsDirectBodyState state){
        if(system != null && ship != null){
            galaxy.RemoveChild(ship);
            system.GetNode("StarSysObjects").AddChild(ship);
            ship.targetManager.NextTarget();
            var trans = state.Transform;
            trans.origin = system.Radius*0.9f*(-aproachVec)+system.GlobalTransform.origin;
            state.Transform = trans;
            ship.System = system;
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
            ship.targetManager.NextTarget();
            ship.System = null;
            ship.Visible = false;
        }
    }

    public void ConnectToEnterPlanet(Node node){
        node.Connect("EnterPlanet", this, nameof(_on_Ship_EnterPlanet));
    }

    public void ConnectToLeavePlanet(Node node){
        node.Connect("LeavePlanet", this, nameof(_on_Ship_LeavePlanet));
    }

    void _on_Ship_EnterPlanet(Ship ship, Planet planet){
        if(!planet.Orbit.GetChildren().Contains(ship)){
            ship.GetParent().RemoveChild(ship);
            planet.Orbit.AddNode(ship);
            //ship.Visible = false;
            ship._Planet = planet;
            ship.PlanetPos = (planet.Transform.origin - ship.GlobalTransform.origin);
            // if(!ship.IsConnected("LeavePlanet", this, nameof(_on_Ship_LeavePlanet))){
            //     ship.ConnectToLeavePlanet(this, nameof(_on_Ship_LeavePlanet));
            // }
        }
    }

    void _on_Ship_LeavePlanet(Ship ship){
        ship._Planet.Orbit.RemoveChild(ship);
        ship.System.AddMapObject(ship);
        ship._Planet = null;
        ship.Visible = true;
    }

    public void _on_UInfo_ChangeStance(Node node, string stance){
        if(node is Ship ship){
            if(ship._Planet != null){
                if(ship._Planet.PlanetOwner == null){
                    ship._Planet.PlanetOwner = ship.ShipOwner;
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
