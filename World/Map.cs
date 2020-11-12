using Godot;
using System;

public class Map : Node
{
    Galaxy galaxy = null;

    MapObjects mapObj = null;
    Combat combat;

    void GetNodes(){
        galaxy = GetNode<Galaxy>("Galaxy");
        mapObj = GetNode<MapObjects>("MapObjects");
        combat = GetNode<Combat>("Combat");
    }

    void _on_Ship_EnterSystem(int shipId, int SystemID, Vector3 aproachVec, PhysicsDirectBodyState state){
        MoveShipToSystem(shipId,SystemID, aproachVec, state);
    }

    void MoveShipToSystem(int shipID, int systemID, Vector3 aproachVec, PhysicsDirectBodyState state){
        var system = galaxy.GetChild<StarSystem>(systemID);
        var ship = galaxy.GetChild<Ship>(shipID);
        if(system != null && ship != null){
            galaxy.RemoveChild(ship);
            system.GetNode("StarSysObjects").AddChild(ship);
            ship.targetManager.NextTarget();
            var trans = state.Transform;
            trans.origin = system.Diameter*0.9f*(-aproachVec)+system.GlobalTransform.origin;
            state.Transform = trans;
            ship.System = system;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
    }

    public void ConnectToEnterSystem(Node node){
        node.Connect("EnterSystem", this, nameof(_on_Ship_EnterSystem));
    }

    void _on_EnterCombat(PhysicsBody ship, PhysicsBody enemy, Node parent){
        if(ship != null && enemy != null){
            if(!combat.Combatants.Contains(ship) && !combat.Combatants.Contains(enemy)){
                combat.Combatants.Add(ship);
                combat.Combatants.Add(enemy);
                combat.CreateBattle(ship, enemy,parent);
            }
        }
    }

    public void ConnectToEnterCombat(Node node){
         node.Connect("EnterCombat", this, nameof(_on_EnterCombat));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
