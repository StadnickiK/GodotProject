using Godot;
using System;

public class Ship : RigidBody
{
    [Signal]
    public delegate void SelectUnit(RigidBody unit);

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    int effectiveRange = 5;

    [Export]
    Vector3 dir = new Vector3();

    [Export]
    public int ID_Owner { get; set; }

    Vector3 targetPos = Vector3.Zero;

    VelocityController _velocityController = new VelocityController();

    Ship self = null;

    public TargetManager<RigidBody> targetManager { get; set; } = new TargetManager<RigidBody>();


    void UpdateLinearVelocity(){
            LinearVelocity = _velocityController.GetAcceleratedVelocity(LinearVelocity,GlobalTransform.origin,targetPos);
    }

    void ResetVelocity(){
        _velocityController.ResetSpeed();
        LinearVelocity = Vector3.Zero;
        targetPos = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Sleeping = true;
        GD.Print("sleep");
    }

    Vector3 PosToTargetWithEffectiveRange(Vector3 targetPos){
        Vector3 effectivePos = ((targetPos) - ((targetPos - GlobalTransform.origin).Normalized()*effectiveRange));
        GD.Print(effectivePos + " "+ targetPos);
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
            state.AngularVelocity = _velocityController.GetAngularVelocity(GlobalTransform,targetPos);
            UpdateLinearVelocity();
            if(LinearVelocity != Vector3.Zero){
                if(LinearVelocity < new Vector3(0.01f,0.01f,0.01f) && 
                LinearVelocity > new Vector3(-0.01f,-0.01f,-0.01f)){
                    GD.Print(targetPos + " " + GlobalTransform.origin);
                    ResetVelocity();
                }  
                //GD.Print(AngularVelocity);
                //GD.Print(targetPos + " " + GlobalTransform.origin);
                //GD.Print(LinearVelocity);
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      self = (Ship)GetParent().GetChild(GetIndex());
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
