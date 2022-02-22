using Godot;
using System;
using System.Linq;

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
                    var battle = combat.CreateBattle(ship, enemy, parent);
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

    public void ConnectToExitMapObject(Node node){
        node.Connect("SignalExitMapObject", this ,nameof(_on_Exit_MapObject));
    }

    void _on_Exit_MapObject(Node mapObject, Node parentMapObject, Vector3 exitVec = default(Vector3), PhysicsDirectBodyState state = null){
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
                    if(system.Transform.origin.DistanceTo(target.Transform.origin) > system.Transform.origin.DistanceTo(starSystem.Transform.origin))
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
