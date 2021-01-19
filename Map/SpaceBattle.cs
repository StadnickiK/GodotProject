using Godot;
using System;
using System.Collections.Generic;

public class SpaceBattle : StaticBody
{

    public List<PhysicsBody> Comabatants { get; set; } = new List<PhysicsBody>();

    public Node Participants = null;

    public List<PhysicsBody> Debris { get; set; } = new List<PhysicsBody>();

    public Ship Attacker { get; set; } = null;

    public Ship Defender { get; set; } = null;

    bool ZeroOne = false;

    [Signal]
    public delegate void OpenBattlePanel(SpaceBattle battle);

    MeshInstance _placeholder = null;

    Spatial _mesh = null;

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
        if(Comabatants.Count >=2){
            if(Comabatants[0] is Ship ship && Comabatants[1] is Ship ship2){
                _placeholder.QueueFree();
                MeshInstance mesh = (MeshInstance)ship.Mesh.Duplicate();
                var transform = mesh.Transform;
                transform.origin = new Vector3(2,0,0);
                mesh.Transform = transform;
                _mesh.AddChild(mesh);
                mesh = (MeshInstance)ship2.Mesh.Duplicate();
                transform = mesh.Transform;
                transform.origin = new Vector3(-2,0,0);
                mesh.Transform = transform;
                _mesh.AddChild(mesh);
            }
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
    }

    public void ConnectToOpenBattlePanel(Node node, string method){
        Connect(nameof(OpenBattlePanel),node, method);
    }

    void Combat(){
        var defenderCount = Defender.Units.Count - 1;
        for(var i = Attacker.Units.Count-1;i>=0; i--){
            var unit = Attacker.Units[i];
            unit.CalculateDamage(Defender.Units[defenderCount]);
            Defender.Units[defenderCount].CalculateDamage(unit);
            if(!Defender.Units[defenderCount].HasHitpoints){
                Defender.Units.RemoveAt(defenderCount);
                defenderCount -= 1;
            }
            if(!unit.HasHitpoints){
                Attacker.Units.Remove(unit);
            }
            if(Attacker.Units.Count == 0 || Defender.Units.Count == 0){
                EndCombat();
            }
        }
    }

    void EndCombat(){
        QueueFree();
    }

    public void _on_SpaceBattle_input_event(Camera camera, InputEvent input, Vector3 clickPosition, Vector3 clickNormal, int index){
        if(input is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(OpenBattlePanel), (PhysicsBody)this);
            break;
          case ButtonList.Right:
            EmitSignal(nameof(Ship.SelectTarget), (PhysicsBody)this);
            break;
        }
      }
    }

  public override void _Process(float delta){
      if(Attacker != null&& Defender != null){
        Combat();
      }
  }

}
