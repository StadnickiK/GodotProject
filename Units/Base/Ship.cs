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
    public delegate void EnterSystem(int shipID, int systemID, Vector3 aproachVec, PhysicsDirectBodyState state);

    [Signal]
    public delegate void EnterCombat(int shipID, int enemyID);

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; }

    public StarSystem System { get; set; } = null;

    public StatManager StatManager { get; set; } = new StatManager();

    Vector3 targetPos = Vector3.Zero;

    protected Ship self = null;

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
        return (targetManager.currentTarget.GlobalTransform.origin-GlobalTransform.origin).Normalized();
    }

    public void MoveToTarget(PhysicsBody target){
        Sleeping = false;
        targetPos = PosToTargetWithEffectiveRange(target.GlobalTransform.origin);
    }

    public void MoveToPos(Vector3 destination){
        Sleeping = false;
        targetPos = destination;
    }

    public void MoveToSystem(StarSystem system){
        Sleeping = false;
        targetPos = system.GlobalTransform.origin;
        System = system;
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
                EmitSignal(nameof(EnterSystem), self.GetIndex(), targetManager.currentTarget.GetIndex(), DirToCurrentTarget(), state);
            }
        }else{
            Sleeping = true;
            state.Sleeping = true;
        }
    }

    public void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(SelectUnit), (PhysicsBody)self);
            break;
          case ButtonList.Right:
            EmitSignal(nameof(SelectTarget), (PhysicsBody)self);
            break;
        }
      } 
    }

    void _on_Ship_body_entered(Node node){
        if(node is Ship){
            EmitSignal(nameof(EnterCombat), self.GetIndex(), node.GetIndex());
            GD.Print("colliision");
        }
    }

    protected void _ConnectSignal(){
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/World/WorldCursorControl");
        WCC.ConnectToSelectUnit(self);
        WCC.ConnectToSelectTarget(self);
        Galaxy galaxy = GetNode<Galaxy>("/root/World/Galaxy");
        galaxy.ConnectToEnterSystem(self);
        galaxy.ConnectToEnterCombat(self);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        self = (Ship)GetParent().GetChild(GetIndex());
        _velocityController.Mass = 10;
        _ConnectSignal();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
