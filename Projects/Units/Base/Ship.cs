using Godot;
using System;
using System.Collections.Generic;

public class Ship : RigidBody
{
    [Signal]
    public delegate void SelectUnit(RigidBody unit);

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; }

    List<int> _turretIDs = new List<int>();

    Vector3 targetPos = Vector3.Zero;

    protected Ship self = null;

    protected VelocityController _velocityController = new VelocityController();
    public TargetManager<RigidBody> targetManager { get; set; } = new TargetManager<RigidBody>();


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

    public void MoveToTarget(RigidBody target){
        Sleeping = false;
        targetPos = PosToTargetWithEffectiveRange(target.GlobalTransform.origin);
    }

    public void MoveToPos(Vector3 destination){
        Sleeping = false;
        targetPos = destination;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state){
        
        if(targetPos != Vector3.Zero && targetPos != null){
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
        }else{
            Sleeping = true;
        }
    }

    public void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(SelectUnit), (RigidBody)self);
            break;
          case ButtonList.Right:
            EmitSignal(nameof(SelectTarget), (RigidBody)self);
            break;
        }
      } 
    }

    protected void _ConnectSignal(){
        GetNode<WorldCursorControl>("/root/World/WorldCursorControl").ConnectToSelectUnit(self);
        GetNode<WorldCursorControl>("/root/World/WorldCursorControl").ConnectToSelectTarget(self);
        //control.Connect("_SelectTarget", this, nameof(SelectTarget));
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
