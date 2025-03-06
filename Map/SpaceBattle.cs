using Godot;
using System;
using System.Collections.Generic;

public partial class SpaceBattle : StaticBody3D, ISelectMapObject
{

    // public List<PhysicsBody> Comabatants { get; set; } = new List<PhysicsBody>();

    public Node Participants = null;

    // public Ship Attacker { get; set; } = null;

    public List<Ship> Attackers { get; set; } = new List<Ship>();

    public int AttackPower { get; set; }

    // public Ship Defender { get; set; } = null;

    public List<Ship> Defenders { get; set; } = new List<Ship>();

    public int DefPower { get; set; }

    public bool IsLocal { get; set; } = false;

    bool ZeroOne = false;

    bool _endCombat = false;

    public bool PowerChanged { get; set; } = false;

    [Signal]
    public delegate void OpenBattlePanelEventHandler(SpaceBattle battle);

    MeshInstance3D _placeholder = null;

    Node3D _mesh = null;

    double _time = 0;

    public int TimeStep { get; set; } = 1;

    public new void SetPosition(Vector3 pos){
        var trans = Transform;
        trans.Origin = pos;
        Transform = trans;
    }

    public void AddCombatants(params PhysicsBody3D[] body){
        foreach(PhysicsBody3D b in body){
            if(b is Ship ship){
                if(ship.IsLocal)
                    IsLocal = true;
                if(!ZeroOne){
                    Attackers.Add(ship);
                    ZeroOne = true;
                }else{
                    Defenders.Add(ship);
                    ZeroOne = false;
                }
            }
        }
    }

    public void AddAttackers(params PhysicsBody3D[] body){
        foreach(PhysicsBody3D b in body){
            if(b is Ship ship){
                Attackers.Add(ship);
            }
        }
    }

    public void AddADefenders(params PhysicsBody3D[] body){
        foreach(PhysicsBody3D b in body){
            if(b is Ship ship){
                Defenders.Add(ship);
            }
        }
    }

    public void GenerateMesh(){
        if(Attackers.Count > 0 && Defenders.Count > 0){
                _placeholder.QueueFree();
                MeshInstance3D mesh = (MeshInstance3D)Attackers[0].Mesh.Duplicate();
                var transform = mesh.Transform;
                transform.Origin = new Vector3(2,0,0);
                mesh.Transform = transform;
                _mesh.AddChild(mesh);
                mesh = (MeshInstance3D)Defenders[0].Mesh.Duplicate();
                transform = mesh.Transform;
                transform.Origin = new Vector3(-2,0,0);
                mesh.Transform = transform;
                mesh.RotateY(135);
                _mesh.AddChild(mesh);
                _mesh.Scale = new Vector3(0.5f,0.5f,0.5f);
        }
    }

    void GetNodes(){
        Participants = GetNode("Participants");
        _placeholder = GetNode<MeshInstance3D>("Placeholder");
        _mesh = GetNode<Node3D>("Node3D");
    }

    public override void _Ready()
    {
        GetNodes();
        GenerateMesh();
        InitAttackers();
        InitDefenders();
    }

    void InitAttackers(){
        foreach(var attacker in Attackers){
            attacker.GetParent().RemoveChild(attacker);
            Participants.AddChild(attacker);
            AttackPower += attacker.Power;
        }
    }

    void UpdateAttackPower(){
        foreach(var attacker in Attackers){
            AttackPower += attacker.Power;
        }
    }

    void InitDefenders(){
        foreach(var defender in Defenders){
            defender.GetParent().RemoveChild(defender);
            Participants.AddChild(defender);
            DefPower += defender.Power;
        }
    }

    void UpdateDefPower(){
        foreach(var defender in Defenders){
            DefPower += defender.Power;
        }
    }

    void UpdatePower(){
        UpdateAttackPower();
        UpdateDefPower();
    }

    public void ConnectToOpenBattlePanel(Node node, string method){
        Connect(nameof(OpenBattlePanel), new Callable(node, method));
    }

    void Duel(Ship Attacker, Ship Defender){
        var defenderCount = Defender.Units.GetChildren().Count - 1;
        for(var i = Attacker.Units.GetChildren().Count-1;i>=0; i--){
            var node = Attacker.Units.GetChildren()[i];
            if(node is Unit unit){
                if(defenderCount>=0){
                    node = Defender.Units.GetChildren()[defenderCount];
                    if(node is Unit defender){
                        unit.CalculateDamage(defender);
                        if(!defender.HasHitpoints){
                            defender.QueueFree();
                            // Defender.Units.RemoveChild(defender);
                            defenderCount -= 1;
                        }
                    }
                }
                if(!unit.HasHitpoints){
                    unit.QueueFree();
                }
            }
        }
        Attacker.UpdatePower();
        if(Defender.Units.GetChildren().Count == 0){
            Defender.QueueFree();
            Defenders.Remove(Defender);
        }else{
            Defender.UpdatePower();
        }
        PowerChanged = true;
    }

    void Combat(){
        var defendersCount = Defenders.Count - 1;
        for(int i = Attackers.Count-1; i>=0; i--){
            Duel(Attackers[i], Defenders[defendersCount]);
            if(Attackers[i].Units.GetChildren().Count == 0){
                Attackers[i].QueueFree();
                Attackers.RemoveAt(i);
            }
            if(Attackers.Count == 0 || Defenders.Count == 0){
                EndCombat();
            }
        }
        UpdatePower();
    }

    void EndCombat(){
        foreach(Node node in Participants.GetChildren()){
            if(node is Ship ship){
                if(ship.Units.GetChildren().Count == 0){
                    if(ship.Controller != null){
                        ship.Controller.MapObjects.Remove(ship);
                        ship.Controller.MapObjectsChanged = true;
                    }
                    ship.QueueFree();
                }else{
                    Participants.RemoveChild(ship);
                    // if(GetParent().GetParent() is Planet planet){
                    //     planet.EnterMapObject(ship, Vector3.Zero, null);
                    //     if(ship.Task == CmdPanel.CmdPanelOption.Conquer)
                    //         planet.ChangeController(ship.Controller);
                    // }
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
        EmitSignal(nameof(OpenBattlePanelEventHandler), (PhysicsBody3D)this);
    }

    public void _on_SpaceBattle_input_event(Camera3D camera, InputEvent input, Vector3 clickPosition, Vector3 clickNormal, int index){
        if(input is InputEventMouseButton eventMouseButton){
        switch(eventMouseButton.ButtonIndex){
          case MouseButton.Left:
            SelectMapObject();
            break;
          case MouseButton.Right:
            //EmitSignal(nameof(Ship.SelectTarget), (PhysicsBody)this);
            break;
        }
      }
    }

    public override void _Process(double delta){
        if(!_endCombat){
            if(Attackers.Count > 0 && Defenders.Count > 0){
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
