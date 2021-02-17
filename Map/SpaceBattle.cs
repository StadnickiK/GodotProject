using Godot;
using System;
using System.Collections.Generic;

public class SpaceBattle : StaticBody, ISelectMapObject
{

    public List<PhysicsBody> Comabatants { get; set; } = new List<PhysicsBody>();

    public Node Participants = null;

    public List<PhysicsBody> Debris { get; set; } = new List<PhysicsBody>();

    public Ship Attacker { get; set; } = null;

    public Ship Defender { get; set; } = null;

    public bool IsLocal { get; set; } = false;

    bool ZeroOne = false;

    bool _endCombat = false;

    public bool PowerChanged { get; set; } = false;

    [Signal]
    public delegate void OpenBattlePanel(SpaceBattle battle);

    MeshInstance _placeholder = null;

    Spatial _mesh = null;

    float _time = 0;

    public int TimeStep { get; set; } = 1;

    public void SetPosition(Vector3 pos){
        var trans = Transform;
        trans.origin = pos;
        Transform = trans;
    }

    public void AddCombatant(PhysicsBody body){
        Comabatants.Add(body);
    }

    public void AddCombatants(params PhysicsBody[] body){
        foreach(PhysicsBody b in body){
            if(b is Ship ship){
                if(ship.IsLocal)
                    IsLocal = true;
                if(!ZeroOne){
                    Attacker = ship;
                    ZeroOne = true;
                }else{
                    Defender = ship;
                    ZeroOne = false;
                }
            }
        }
    }

    public void GenerateMesh(){
        if(Attacker != null && Defender != null){
                _placeholder.QueueFree();
                MeshInstance mesh = (MeshInstance)Attacker.Mesh.Duplicate();
                var transform = mesh.Transform;
                transform.origin = new Vector3(2,0,0);
                mesh.Transform = transform;
                _mesh.AddChild(mesh);
                mesh = (MeshInstance)Defender.Mesh.Duplicate();
                transform = mesh.Transform;
                transform.origin = new Vector3(-2,0,0);
                mesh.Transform = transform;
                mesh.RotateY(135);
                _mesh.AddChild(mesh);
                _mesh.Scale = new Vector3(0.5f,0.5f,0.5f);
        }
    }
   
    public void RemoveCombatant(PhysicsBody body){
        Comabatants.Remove(body);
    }

    void GetNodes(){
        Participants = GetNode("Participants");
        _placeholder = GetNode<MeshInstance>("Placeholder");
        _mesh = GetNode<Spatial>("Spatial");
    }

    public override void _Ready()
    {
        GetNodes();
        GenerateMesh();
        foreach(PhysicsBody b in Comabatants){
            b.GetParent().RemoveChild(b);
            Participants.AddChild(b);
        }
        Attacker.GetParent().RemoveChild(Attacker);
        Participants.AddChild(Attacker);
        Defender.GetParent().RemoveChild(Defender);
        Participants.AddChild(Defender);
        Comabatants.Add(Attacker);
        Comabatants.Add(Defender);
    }

    public void ConnectToOpenBattlePanel(Node node, string method){
        Connect(nameof(OpenBattlePanel),node, method);
    }

    void Combat(){
        var defenderCount = Defender.Units.Count - 1;
        for(var i = Attacker.Units.Count-1;i>=0; i--){
            var unit = Attacker.Units[i];
            if(defenderCount>=0){
                unit.CalculateDamage(Defender.Units[defenderCount]);
                if(!Defender.Units[defenderCount].HasHitpoints){
                    Defender.Units[defenderCount].QueueFree();
                    Defender.Units.RemoveAt(defenderCount);
                    defenderCount -= 1;
                }
            }
            if(!unit.HasHitpoints){
                Attacker.Units[i].QueueFree();
                Attacker.Units.Remove(unit);
            }
            if(Attacker.Units.Count == 0 || Defender.Units.Count == 0){
                EndCombat();
            }
        }
        Attacker.UpdatePower();
        Defender.UpdatePower();
        PowerChanged = true;
    }

    void EndCombat(){
        foreach(Node node in Comabatants){
            if(node is Ship ship){
                if(ship.Units.Count == 0){
                    if(ship.IsLocal){
                        ship.ShipOwner.MapObjects.Remove(ship);
                        ship.ShipOwner.MapObjectsChanged = true;
                    }
                    ship.QueueFree();
                }else{
                    Participants.RemoveChild(ship);
                    GetParent().AddChild(ship);
                    if(GetParent() is Orbit orbit){
                        var planet = (Planet)orbit.GetParent();
                        planet.ChangePlanetOwner(ship.ShipOwner);
                    }
                    if(ship.IsLocal){
                        ship.Visible = true;
                    }
                }
            }
        }
        QueueFree();
        _endCombat = true;
    }

    public void SelectMapObject(){
        EmitSignal(nameof(OpenBattlePanel), (PhysicsBody)this);
    }

    public void _on_SpaceBattle_input_event(Camera camera, InputEvent input, Vector3 clickPosition, Vector3 clickNormal, int index){
        if(input is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            SelectMapObject();
            break;
          case ButtonList.Right:
            //EmitSignal(nameof(Ship.SelectTarget), (PhysicsBody)this);
            break;
        }
      }
    }

    public override void _Process(float delta){
        if(!_endCombat){
            if(Attacker != null&& Defender != null){
                if(_time >= TimeStep){  
                    Combat();
                    _time = 0;
                }
            }else{
                // to do create debris
                QueueFree();
            }
            _time += delta;
        }
    }
}
