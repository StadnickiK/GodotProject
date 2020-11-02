using Godot;
using System;
using System.Collections.Generic;

public class Galaxy : Spatial
{

    [Export]
    int Size = 40;

    public List<StarSystem> StarSystems { get; set; } = new List<StarSystem>();

    public RigidBody Ground { get; set; } = null;

    [Signal]
    public delegate void CameraLookAt(Vector3 position);

    PackedScene StarSystemScene = null;

    public Random Rand { get; set; }

    int Seed = 0;

    public enum Type
    {
        Elliptical,
        Spiral,
        Irregular
    }

    void InitRand(){
        Seed = Guid.NewGuid().GetHashCode();
        Rand = new Random(Seed);
;    }
    void Generate(){

        int dist = Rand.Next(10, 20);
        float angle = Rand.Next(0, 70);
        for(int i = 0;i < Size; i++){
            RotateY(angle);
            var pos = Transform.basis.Xform(new Vector3(0, 0, dist));
            var starSystem = (StarSystem)StarSystemScene.Instance();
            starSystem.Rand = Rand;
            starSystem.SystemID = i;
            starSystem.SystemName = "System " + i;
            starSystem.Connect("ViewStarSystem", this, nameof(_on_ViewStarSystem));
            starSystem.Connect("ViewGalaxy", this, nameof(_on_ViewGalaxy));
            var temp = starSystem.Transform;
            temp.origin = pos;
            starSystem.Transform = temp;
            AddChild(starSystem);
            StarSystems.Add(starSystem);
            dist += Rand.Next(3, 8);
            angle += Rand.Next(0, 60);
        }
        Rotation = Vector3.Zero;
    }

    void _on_ViewStarSystem(int id){
        foreach(StarSystem s in StarSystems){
            if(s.SystemID != id){
                s.Visible = false;
            }else{
                EmitSignal(nameof(CameraLookAt), s.Transform.origin);
            }
        }
    }

    void _on_ViewGalaxy(){
        foreach(StarSystem s in StarSystems){
            s.Visible = true;
        }
    }
    
    void _on_Ship_EnterSystem(int shipId, int SystemID, Vector3 aproachVec, PhysicsDirectBodyState state){
        MoveShipToSystem(shipId,SystemID, aproachVec, state);
    }

    void LoadNodes(){
        //Ground = (RigidBody)GetNode("Ground");
    }

    void MoveShipToSystem(int shipID, int systemID, Vector3 aproachVec, PhysicsDirectBodyState state){
        var system = GetChild<StarSystem>(systemID);
        var ship = GetChild<Ship>(shipID);
        if(system != null && ship != null){
            RemoveChild(ship);
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
        StarSystemScene = (PackedScene)GD.Load("res://Map/StarSystem.tscn");
        LoadNodes();
        InitRand();
        Generate();
    }

    public void ConnectToEnterSystem(Node node){
        node.Connect("EnterSystem", this, nameof(_on_Ship_EnterSystem));
    }

    List<int> Combatants { get; set; } = new List<int>();
    PackedScene _battleScene = (PackedScene)ResourceLoader.Load("res://Map/SpaceBattle.tscn");

    public SpaceBattle CreateBattle(PhysicsBody ship, PhysicsBody enemy){
        var battle = (SpaceBattle)_battleScene.Instance();
        var trans = battle.Transform;
        trans.origin =  ship.Transform.origin;
        battle.Transform = trans;
        battle.AddCombatants(ship, enemy);
        HideNodes(ship, enemy);
        return battle;
    }

    void HideNodes(params Spatial[] Nodes){
        foreach(Spatial n in Nodes){
            n.Visible = false;
        }
    }

    void _on_EnterCombat(int shipID, int enemyID){
        if(!Combatants.Contains(shipID) && !Combatants.Contains(enemyID)){
            var ship = GetChild<PhysicsBody>(shipID);
            var enemy = GetChild<PhysicsBody>(enemyID);
            Combatants.Add(shipID);
            Combatants.Add(enemyID);
           AddChild(CreateBattle(ship, enemy));
        }
    }

    public void ConnectToEnterCombat(Node node){
         node.Connect("EnterCombat", this, nameof(_on_EnterCombat));
    }
}
