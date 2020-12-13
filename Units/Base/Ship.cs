using Godot;
using System;
using System.Collections.Generic;

public class Ship : RigidBody
{
    [Signal]
    public delegate void SelectUnit(RigidBody unit);

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    [Signal]
    public delegate void LeaveSystem(int id);

    [Signal]
    public delegate void EnterSystem(Ship shipID, StarSystem system, Vector3 aproachVec, PhysicsDirectBodyState state);

    [Signal]
    public delegate void EnterCombat(PhysicsBody ship, PhysicsBody enemy, Node parent);

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; } = 0;

    public StarSystem System { get; set; } = null;

    public StatManager StatManager { get; set; } = new StatManager();

    Vector3 targetPos = Vector3.Zero;

    [Export]
    public int Range { get; set; } = 4;

    Spatial VisionRange = null;

    protected VelocityController _velocityController = new VelocityController();
    public TargetManager<PhysicsBody> targetManager { get; set; } = new TargetManager<PhysicsBody>();

    protected void UpdateLinearVelocity(PhysicsDirectBodyState state){
            state.LinearVelocity = _velocityController.GetAcceleratedVelocity(GlobalTransform.basis.Xform(new Vector3(0, 0, 1)),GlobalTransform.origin,targetPos);
    }

    protected void ResetVelocity(){
        _velocityController.ResetSpeed();
        LinearVelocity = Vector3.Zero;
        targetPos = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Sleeping = true;
    }

    protected Vector3 PosToTargetWithEffectiveRange(Vector3 targetPos){
        Vector3 effectivePos = ((targetPos) - ((targetPos - GlobalTransform.origin).Normalized()*effectiveRange));
        return effectivePos;
    }

    protected Vector3 DirToCurrentTarget(){
        if(targetManager.HasTarget){
            return (targetManager.currentTarget.GlobalTransform.origin-GlobalTransform.origin).Normalized();
        }
        return Vector3.Zero;
    }

    public void MoveToTarget(PhysicsBody target){
        Sleeping = false;
        targetPos = target.GlobalTransform.origin;//PosToTargetWithEffectiveRange(target.GlobalTransform.origin);
    }

    public void MoveToPos(Vector3 destination){
        Sleeping = false;
        targetPos = destination;
    }

    void UpdateShipVelocities(PhysicsDirectBodyState state){
        float angle = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
            if(angle > 0.01f || angle < -0.01f ){
                state.AngularVelocity = _velocityController.GetAngularVelocity(GlobalTransform,targetPos);
            }else{
                AngularVelocity = Vector3.Zero;
                UpdateLinearVelocity(state);
            }
            if(targetPos != Vector3.Zero){
                Vector3 posDiff = (targetPos.Abs() - GlobalTransform.origin.Abs());
                Vector3 MultyScale = Scale;
                if(posDiff.x < MultyScale.x && posDiff.z < MultyScale.z &&
                posDiff.x > -MultyScale.x && posDiff.z > -MultyScale.z){
                    ResetVelocity();
                }  
            }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state){
        if(targetManager.HasTarget ||(targetPos != Vector3.Zero && targetPos != null)){
            UpdateShipVelocities(state);
            if((targetManager.currentTarget is StarSystem) && (targetPos - GlobalTransform.origin).Length()<2){
                GD.Print("Enter system");
                EmitSignal(nameof(EnterSystem), this, (StarSystem)targetManager.currentTarget, DirToCurrentTarget(), state);
            }
            if(System != null){
                if(Transform.origin.Length()>System.Diameter){
                    GD.Print("Leave system");
                    EmitSignal(nameof(LeaveSystem), this, DirToCurrentTarget(), state);
                }
            }

        }else{
            Sleeping = true;
            state.Sleeping = true;
        }
    }

    void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(SelectUnit), (PhysicsBody)this);
            GD.Print("left");
            break;
          case ButtonList.Right:
            EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
            break;
        }
      } 
    }

    void _on_Area_body_entered(Node body){
        if(body is Ship){
            var s = (Ship)body;
            if(s.ID_Owner != ID_Owner && s.Visible == false){
                s.Visible = true;
            }
        }
    }

    void _on_Area_body_exited(Node body){
        if(body is Ship){
            var s = (Ship)body;
            if(s.ID_Owner != ID_Owner && s.Visible == true){
                s.Visible = false;
            }
        }
    }

    void _on_Ship_body_entered(Node node){
        if(node is Ship){
            EmitSignal(nameof(EnterCombat), (PhysicsBody)this, (PhysicsBody)node, GetParent());
        }
    }

    protected void _ConnectSignal(){
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
        WCC.ConnectToSelectUnit(this);
        WCC.ConnectToSelectTarget(this);
        Map map = GetNode<Map>("/root/Game/World/Map/");
        map.ConnectToEnterSystem(this);
        map.ConnectToLeaveSystem(this);
        map.ConnectToEnterCombat(this);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //VisionRange = GetNode<Spatial>("VisionRange");
        //VisionRange.Scale = new Vector3(Range,0,Range);

        _velocityController.Mass = 10;
        _ConnectSignal();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
