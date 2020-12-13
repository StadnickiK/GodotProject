using Godot;
using System;
using System.Collections.Generic;

public class SpaceBattle : StaticBody
{

    public List<PhysicsBody> Comabatants { get; set; } = new List<PhysicsBody>();

    public Node Participants = null;

    public List<PhysicsBody> Debris { get; set; } = new List<PhysicsBody>();

    [Signal]
    public delegate void OpenBattlePanel(SpaceBattle battle);

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
            Comabatants.Add(b);
            b.GetParent().RemoveChild(b);
            AddChild(b);
        }
    }
   
    public void RemoveCombatant(PhysicsBody body){
        Comabatants.Remove(body);
    }

    void GetNodes(){
        Participants = GetNode("Participants");
        if(Participants == null){
            GD.Print("null");
        }
    }

    public override void _Ready()
    {
        GetNodes();
    }

    public void ConnectToOpenBattlePanel(Node node, string method){
        Connect(nameof(OpenBattlePanel),node, method);
    }

    public void _on_SpaceBattle_input_event(Camera camera, InputEvent input, Vector3 clickPosition, Vector3 clickNormal, int index){
        if(input is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(OpenBattlePanel), (PhysicsBody)this);
            GD.Print("left");
            break;
          case ButtonList.Right:
            EmitSignal(nameof(Ship.SelectTarget), (PhysicsBody)this);
            break;
        }
      }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
