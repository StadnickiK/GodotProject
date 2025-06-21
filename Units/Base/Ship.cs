using Godot;
using System;
using System.Collections.Generic;


	public struct ShipStruct : IMapObjectController
	{
		public Node Parent { get; set; }
		public Vector3 Position { get; set; }

        public Player Controller { get; set; }

        public Dictionary<int, VisibilityConroller.VisibilityStruct> PlayerVisibility { get; private set; }

        public List<Unit> Units { get; set; }

		public TargetManager<Node3D>.Target Target { get; set; }

		public string Name { get; set; }

        public bool Visible { get; set; }
	}

public partial class Ship : CharacterBody3D, ISelectMapObject, IExtendedMapObjectController, IVision, IGetTotalUpkeep, IEnterMapObject
// IMapObject,
{
    [Signal]
    public delegate void SelectUnitEventHandler(RigidBody3D unit);

    [Signal]
    public delegate void SelectTargetEventHandler(RigidBody3D target);

    public delegate void EnterCombatEventHandler(Ship ship, Ship enemy, Node parent);

    public event EnterCombatEventHandler EnterCombat;

    [Signal]
    public delegate void SignalExitMapObjectEventHandler(Node node, Vector3 aproachVec, PhysicsDirectBodyState3D state); 

    [Signal]
    public delegate void OpenUnitTransferPanelEventHandler(Ship left, Ship right);

    public delegate void PlayerDataChanged();

    public event PlayerDataChanged ArmiesChanged;

    public event World.SplitShipEventHandler SplitShip;

    public event World.FreeShipEventHandler FreeShip;

    public event World.TransferUnitsEventHandler OpenTransferPanel;

    public VisibilityConroller VisibilityConroller { get; set; }

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; }

    [Export]
    public bool IsLocal { get; set; } = false;

    private Player myVar;
    public Player Controller
    {
        get { return myVar; }
        set { myVar = value; ControllerChanged?.Invoke(Controller);}
    }

    //public IEnterMapObject MapObject { get; set; } = null;

    public UnitController Units { get; set; }

    //public Vector3 PlanetPos { get; set; } = Vector3.Zero;

    public Planet ClosePlanet { get; set; }

    public MeshInstance3D Mesh { get; set; } = null;  

    public float Power { get; set; }

    [Export]
    public int VisionRange { get; set; } = 4;

    public VisionComponent _area { get; set; }

    public RecruitmentComponent RecruitmentComponent { get; set; }

    public StateMachine StateMach { get; set; }

    public TargetManager<Node3D> targetManager { get; set; } = new TargetManager<Node3D>();

    //public CmdPanel.CmdPanelOption Task { get; set; } = CmdPanel.CmdPanelOption.None;

    SimpleFireControl _control = null;

    public event IExtendedMapObjectController.ControllerChangedEventHandler ControllerChanged;

    public List<Unit> UnitsToTransfer = new List<Unit>();

    public enum ArmyStance
    {
        Idle,
        Moving,
        Rectruiting
    }

    public ArmyStance Stance { get; set; } = ArmyStance.Idle;

    void FreeThisShip()
    {
            FreeShip?.Invoke(this);
    }

    public bool CanMove { get; set; } = true;

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

    void _on_UpdateRecruitment(RecruitmentComponent recruitmentComponent){
        CanMove = recruitmentComponent.CurrentlyRecruitedUnits.Count == 0;
        StateMach.Enter(new IdleState());
        Stance = ArmyStance.Rectruiting;
    }

    public void _on_UpdateUnitsToTransfer(List<Unit> unitsToTransfer){
        CanMove = unitsToTransfer.Count == 0;
        UnitsToTransfer = unitsToTransfer;
        StateMach.Enter(new IdleState());
        Stance = ArmyStance.Idle;
    } 

    void Split(TargetManager<Node3D>.Target target){
        if(UnitsToTransfer.Count > 0 && UnitsToTransfer.Count < Units.UnitsList.Count){
                Units.RemoveUnits(UnitsToTransfer);
                SplitShip?.Invoke(new ShipStruct(){
                    Target = target,
                    Name = this.Name,
                    Parent = this.GetParent(),
                    Controller = this.Controller,
                    Position = this.Position,
                    Units = UnitsToTransfer,
                    Visible = this.Visible,
                });
                UnitsToTransfer.Clear();
            }
    }

    void Merge(TargetManager<Node3D>.Target target){
        if(target.TargetNode is Ship ship && target.TargetNode != this){
            if (ship.Controller == this.Controller)
            {
                if (Units.Count + ship.Units.Count < ship.Units.MaxUnits)
                {
                    Units.TransferUnit(ship.Units);
                    FreeShip?.Invoke(this);
                }
                else
                {
                    OpenTransferPanel?.Invoke(this, ship);
                }
            }
            else
            {
                EnterCombat?.Invoke(this, ship, GetParent());
            }
        }
    }

    public void TransferUnits(Ship target, List<Unit> targetUnits){
        Units.RemoveUnits(UnitsToTransfer);
        target.Units.AddUnit(UnitsToTransfer);
        target.Units.RemoveUnits(targetUnits);
        Units.AddUnit(targetUnits);
    }

    public void Merge(Ship ship){
        foreach(Unit unit in ship.Units.UnitsList){
            unit.GetParent().RemoveChild(unit);
            Units.AddUnit(unit);
        }
        ship.Controller.RemoveMapObject(ship);
        ArmiesChanged?.Invoke();
        FreeShip?.Invoke(ship);
        //ship.QueueFree();
    }

    public void MoveToTarget(TargetManager<Node3D>.Target target){
        // Sleeping = false;
        if(target.TargetNode != this)
            if(CanMove){
                targetManager.SetTarget(target); 
                var moveState = new MoveState(target);
                StateMach.Enter(moveState);
                moveState.MoveStateExited += Merge;
                Stance = ArmyStance.Moving;
            }else{
                Split(target);
            }
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
        if(CanMove){
            targetManager.SetTarget(new TargetManager<Node3D>.Target(destination)); 
            StateMach.Enter(new MoveState(destination));
        }else{
            Split(new TargetManager<Node3D>.Target(destination));
        }
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

    public void ChangeVision(VisibilityConroller.VisibilityStruct visibilityStruct, int playerID){
        VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct(){Visibility = VisibilityConroller.VisibilityState.Visible, Visible = true}, playerID);
    }


    void GetNodes(){
        _area = GetNode<VisionComponent>("Area3D");
        VisibilityConroller = GetNode<VisibilityConroller>("VisibilityConroller");
        Mesh = GetNode<MeshInstance3D>("ship model/Cube");
        _control = GetNode<SimpleFireControl>("FireControl");
        StateMach = GetNode<StateMachine>("StateMachine");
        //_velocityController = GetNode<VelocityController>("VelocityController");
        Units = GetNode<UnitController>("UnitController");
        RecruitmentComponent = GetNode<RecruitmentComponent>("RecruitmentComponent");
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
        RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent += _on_UpdateRecruitment;
        //_velocityController.Mass = 10;
        _area.UpdateVisionRange(VisionRange);
        Units.NoUnits += FreeThisShip;

    }

    public void UpdatePower(){
        Power = 0;
        foreach(var unit in Units.UnitsList){
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

    public void EnterMapObject(Node node)
    {
        if(node is Ship ship){
            Merge(ship);
        }
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
