using Godot;
using System;
using System.Collections.Generic;

public partial class Ship : CharacterBody3D, ISelectMapObject, IMapObjectController, IVision, IMapObject, IGetTotalUpkeep//, IResourceManager
{
    [Signal]
    public delegate void SelectUnitEventHandler(RigidBody3D unit);

    [Signal]
    public delegate void SelectTargetEventHandler(RigidBody3D target);

    [Signal]
    public delegate void EnterCombatEventHandler(PhysicsBody3D ship, PhysicsBody3D enemy, Node parent);

    [Signal]
    public delegate void SignalEnterMapObjectEventHandler(Node node, Vector3 aproachVec, PhysicsDirectBodyState3D state);

    [Signal]
    public delegate void SignalExitMapObjectEventHandler(Node node, Vector3 aproachVec, PhysicsDirectBodyState3D state); 

    [Signal]
    public delegate void OpenUnitTransferPanelEventHandler(Ship left, Ship right);

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; }

    [Export]
    public bool IsLocal { get; set; } = false;
    public Player Controller { get; set; } = null;

    public IEnterMapObject MapObject { get; set; } = null;

    public List<BaseStat> Stats { get; set; } = new List<BaseStat>(); // leak

    //public List<Unit> Units { get; set; } = new List<Unit>(); // probably leak

    public Node Units { get; set; } = new Node();

    public Vector3 PlanetPos { get; set; } = Vector3.Zero;

    public MeshInstance3D Mesh { get; set; } = null;  

    public int Power { get; set; }

    [Export]
    public int VisionRange { get; set; } = 4;

    public VisionArea _area { get; set; }

    //protected VelocityController _velocityController = null;

    public StateMachine StateMach { get; set; }

    public TargetManager<Node3D> targetManager { get; set; } = new TargetManager<Node3D>();

    public CmdPanel.CmdPanelOption Task { get; set; } = CmdPanel.CmdPanelOption.None;

    SimpleFireControl _control = null;

    // protected void UpdateLinearVelocity(PhysicsDirectBodyState3D state){
    //     // was GlobalTransform.Basis.XForm (new Vector3(0, 0, 1)
    //         state.LinearVelocity = _velocityController.GetAcceleratedVelocity(GlobalTransform.Basis * (new Vector3(0, 0, 1)),GlobalTransform.Origin,targetManager.currentTarget.Point);
    // }

    // protected void ResetVelocity(){
    //     _velocityController.ResetSpeed();
    //     // LinearVelocity = Vector3.Zero;
    //     // AngularVelocity = Vector3.Zero;
    //     // Sleeping = true;
    // }

    // protected void ResetVelocity(PhysicsDirectBodyState3D state){
    //     _velocityController.ResetSpeed();
    //     state.LinearVelocity = Vector3.Zero;
    //     state.AngularVelocity = Vector3.Zero;
    //     state.Sleeping = true;
    // }

    // protected Vector3 PosToTargetWithEffectiveRange(Vector3 targetPos){
    //     Vector3 effectivePos = ((targetPos) - ((targetPos - GlobalTransform.Origin).Normalized()*effectiveRange));
    //     return effectivePos;
    // }

    // protected Vector3 DirToCurrentTarget(){
    //     if(targetManager.HasTarget){
    //         return (targetManager.currentTarget.Point-Transform.Origin).Normalized();
    //     }
    //     return Vector3.Zero;
    // }

    // public Node3D GetTempWaypoint(Vector3 position){
    //     var tempTarget = new Node3D(); // used as point to exit current system
    //     var tempTrans = tempTarget.Transform;
    //     tempTrans.Origin = position;
    //     tempTarget.Transform = tempTrans;
    //     tempTarget.Name = "tempTarget";
    //     return tempTarget;
    // }

    public void MoveToTarget(Node3D target){
        
        // Sleeping = false;
        var t = new TargetManager<Node3D>.Target(target.Position, target);
        targetManager.SetTarget(t); 
        StateMach.Enter(new MoveState(t));
        // Node starSysObj = null;
        // if((GetParent() is Orbit orbit)){ 
        //     starSysObj = orbit.GetParent().GetParent();
        // }else{
        //     starSysObj = GetParent();
        // }
        // if(target == starSysObj.GetParent() && GetParent().GetParent() is Planet planet){
        //     //var tempTarget = GetTempWaypoint(planet.Transform.Origin);
        //     targetManager.SetTarget(new TargetManager<Node3D>.Target(planet.Transform.Origin, planet));    // set temp target to leave orbit
        //     targetManager.AddTarget(new TargetManager<Node3D>.Target(planet.Transform.Origin, planet));
        //     return;
        // }
        // if(target.GetParent() != starSysObj){   // check if target is in the same map object (f.e. star system)
        //     if(starSysObj.GetParent() is StarSystem system){
                
        //         // leak
        //         var tempTarget = GetTempWaypoint((target.Transform.Origin-system.Transform.Origin).Normalized()*(((float)system.Radius)*1.2f));
        //         targetManager.SetTarget(tempTarget);    // set temp target to leave star system
        //         // if(target.GetParent().GetParent() is StarSystem targetSystem){
        //         //     targetManager.AddTarget(targetSystem);
        //         // }
        //         targetManager.AddTarget(target);
        //         return;
        //     }else{
        //         if(target.GetParent().GetParent() is StarSystem targetSystem){
        //             targetManager.SetTarget(targetSystem);
        //         }
        //         targetManager.AddTarget(target);
        //         return;
        //     }
        // }
        //targetManager.SetTarget(target); // if target is in the same map object (f.e. star system) set as target
    }

    public void MoveToPos(Vector3 destination){
        // Sleeping = false;
        targetManager.SetTarget(new TargetManager<Node3D>.Target(destination)); 
        StateMach.Enter(new MoveState(destination));
    }

    // public void NextTarget(){
    //     if(targetManager.currentTarget.TargetNode is StaticBody3D body){
    //         if(body.Name == "tempTarget"){
    //             body.QueueFree();
    //             targetManager.NextTarget();
    //             return;
    //         }
    //     }
    //     targetManager.NextTarget();
    // }

    // void UpdateShipVelocities(PhysicsDirectBodyState3D state , Vector3 targetPos){
    //     if(targetManager.HasTarget){
    //         float angle = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
    //         if(angle > 0.01f || angle < -0.01f ){
    //             state.AngularVelocity = _velocityController.GetAngularVelocity(GlobalTransform,targetPos);
    //             UpdateLinearVelocity(state);
    //         }else{
    //             // AngularVelocity = Vector3.Zero;
    //             UpdateLinearVelocity(state);
    //         }
    //         if(targetPos != Vector3.Zero){
    //             Vector3 posDiff = (targetPos.Abs() - GlobalTransform.Origin.Abs());
    //             Vector3 MultyScale = Scale;
    //             if(posDiff.X < MultyScale.X && posDiff.Z < MultyScale.Z &&
    //             posDiff.X > -MultyScale.X && posDiff.Z > -MultyScale.Z){
    //                 NextTarget();
    //             }  
    //         }
    //     }
    // }

    public void Merge(Ship ship){
        foreach(Unit unit in ship.Units.GetChildren()){
            unit.GetParent().RemoveChild(unit);
            Units.AddChild(unit);
        }
        ship.Controller.RemoveMapObject(ship);
        ship.Controller.MapObjectsChanged = true;
        ship.QueueFree();
    }

    // public void _IntegrateForces(PhysicsDirectBodyState3D state){
    //     if(targetManager.HasTarget){
    //         Vector3 targetPos = Vector3.Zero;
    //         if(targetManager.currentTarget.TargetNode is PhysicsBody3D){
    //             targetPos = targetManager.currentTarget.Point;
    //         }else{
    //             targetPos = targetManager.currentTarget.Point;
    //         }


    //         if(targetManager.currentTarget.TargetNode is IEnterMapObject targetObject && (targetPos - GlobalTransform.Origin).Length()<2){
    //             if(MapObject != targetObject){
    //                 if(targetObject is Node someNode)
    //                     EmitSignal(nameof(SignalEnterMapObjectEventHandler), this, someNode, DirToCurrentTarget(), state);
    //                 MapObject = targetObject;
    //             }
    //             NextTarget();
    //         }
    //         if(MapObject != null){
    //             if(MapObject != targetManager.currentTarget.TargetNode){
    //                 if(MapObject is Node someNode)
    //                     EmitSignal(nameof(SignalExitMapObjectEventHandler), this, someNode, DirToCurrentTarget(), state);
    //             }
    //         }
    //         if(targetManager.currentTarget.TargetNode is Ship ship && (targetPos - GlobalTransform.Origin).Length()<2){
    //             if(ship.Controller != Controller){
    //                 EmitSignal(nameof(EnterCombatEventHandler), (PhysicsBody3D)this, (PhysicsBody3D)ship, GetParent());
    //             }else{
    //                 EmitSignal(nameof(OpenUnitTransferPanelEventHandler), this, ship);
    //             }
    //             targetManager.ClearTargets();
    //             ResetVelocity(state);
    //         }


    //         UpdateShipVelocities(state, targetPos);
    //         _control.Start();
    //     }else{
    //         state.AngularVelocity = Vector3.Zero;
    //         state.LinearVelocity = Vector3.Zero;
    //         // Sleeping = true;
    //         state.Sleeping = true;
    //         _control.Stop();
    //     }
    // }

    public void SelectMapObject(){
        EmitSignal(nameof(SignalName.SelectUnit), (PhysicsBody3D)this);
    }

    public void GetTotalUpkeep(Dictionary<int, int> costs){
        foreach(var node in Units.GetChildren()){
            if(node is IUpkeep upkeep){
                foreach(var resource in upkeep.Upkeep.Keys){
                    if(costs.ContainsKey(resource)){
                        costs[resource] += upkeep.Upkeep[resource];
                    }else{
                        costs.Add(resource, upkeep.Upkeep[resource]);
                    }
                }
            }
        }
    }

    void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch(eventMouseButton.ButtonIndex){
          case MouseButton.Left:
            SelectMapObject();
            break;
          case MouseButton.Right:
            EmitSignal(SignalName.SelectTarget, (PhysicsBody3D)this);
            break;
        }
      } 
    }

    public void ChangeVision(){
        if(Visible){
            Visible = false;
        }else{
            Visible = true;
        }
    }

    void _on_Area_body_entered(Node body){
        // to do test in galaxy
        if(body is IVisible visionObject){
            if(visionObject.Controller != Controller && visionObject.ReturnVisible() == false && body.GetParent() == GetParent()){
                visionObject.ChangeVision();
            }
            if(Controller != null)
                if(body is Planet planet){ 
                        if(Controller.IsLocal && planet.Orbit.GetChildren().Count > 0){
                            planet.IcoOrbit.Visible = true;
                            if(planet.Orbit.HasEnemy(this))
                                planet.IcoOrbit.SetRed();
                        }
                }
        }
    }

    void _on_Area_body_exited(Node body){
        if(body is IVisible visionObject){
            if(Controller != null){
                if(visionObject.Controller != Controller && visionObject.ReturnVisible() == true && body.GetParent() == GetParent() && Controller.IsLocal){
                    visionObject.ChangeVision();
                }
                if(body is Planet planet){
                    if(Controller.IsLocal && visionObject.Controller != Controller){
                        planet.IcoOrbit.Visible = false;
                    }
                }
            }
        }
    }

    void _on_Ship_body_entered(Node node){
        if(node is Ship ship){
            if(ship.MapObject == MapObject){
                EmitSignal(nameof(EnterCombatEventHandler), (PhysicsBody3D)this, (PhysicsBody3D)node, GetParent());
                //ResetVelocity();
            }
        }
    }

    void GetNodes(){
        _area = GetNode<VisionArea>("Area3D");
        Mesh = GetNode<MeshInstance3D>("ship model/Cube");
        _control = GetNode<SimpleFireControl>("FireControl");
        StateMach = GetNode<StateMachine>("StateMachine");
        //_velocityController = GetNode<VelocityController>("VelocityController");
        AddChild(Units);
        AddChild(targetManager);
    }

    public void ConnectToEnterCombat(Node node, string methodName){
         Connect("EnterCombatEventHandler", new Callable(node, methodName));
    }

    public override void _Ready()
    {
        GetNodes();
        StateMach.Body = this;
        StateMach.Enter(new IdleState());
        //_velocityController.Mass = 10;
        _area.UpdateVisionRange(VisionRange);
    }

    public void UpdatePower(){
        Power = 0;
        foreach(Node node in Units.GetChildren()){
            if(node is Unit unit)
                Power += unit.Stats.GetNode<BaseStat>("HitPoints").CurrentValue;
        }
    }

    public void SetVisibility(bool visible)
    {
        Visible = visible;
    }

    public int ReturnIndex()
    {
        return GetIndex();
    }

    public bool ReturnVisible()
    {
        return Visible;
    }

    // public bool IsVisible()
    // {
    //     return Visible;
    // }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
         
    //  }


}
