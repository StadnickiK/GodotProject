using Godot;
using System;
using System.Collections.Generic;


	public class ShipModel : Node3DModel, IMapObjectController
	{

        public Player Controller { get; set; }

        public Dictionary<int, VisibilityConroller.VisibilityStruct> PlayerVisibility { get; private set; }

        public List<Unit> Units { get; set; }

		public OrderQueue.Target Target { get; set; }

		
	}

public partial class Ship : CharacterBody3D, IMapObjectController, IVision, IEnterMapObject, IEnterCombat, IMovable, ISelectMapObject, IPlanetInterface
{
    public delegate void EnterCombatEventHandler(IEnterCombat attacker, IEnterCombat enemy, Node parent);

    public event IEnterCombat.EnterCombatEventHandler EnterCombat;

    [Signal]
    public delegate void SignalExitMapObjectEventHandler(Node node, Vector3 aproachVec, PhysicsDirectBodyState3D state); 

    [Signal]
    public delegate void OpenUnitTransferPanelEventHandler(Ship left, Ship right);

    public delegate void PlayerDataChanged();

    public event PlayerDataChanged ArmiesChanged;

    public event World.FreeShipEventHandler FreeShip;

    public event World.TransferUnitsEventHandler OpenTransferPanel;

    public InputController InputController { get; set; }

    public IMovableState MovableState { get; set; } = IMovableState.Movement;

    public VisibilityConroller VisibilityConroller { get; set; }

    public ModelLoader ModelLoader { get; set; }

    public int CurrentModelVoluume { get; set; } = 0;

    [Export]
    public int effectiveRange = 10;

    [Export]
    public bool IsLocal { get; set; } = false;

    public SelectionCircleControler Selection { get; set; }

    private Player _controller;
    public Player Controller { get => _controller; set { _controller = value; ControllerComponent.Controller = value; } }

    //public IEnterMapObject MapObject { get; set; } = null;

    public UnitController UnitController { get; set; }

    public ControllerComponent ControllerComponent { get; set; }

    public SimpleFireControl SimpleFireControl { get; set; }

    //public Vector3 PlanetPos { get; set; } = Vector3.Zero;

    public MeshInstance3D Mesh { get; set; } = null;  

    public float Power { get; set; }

    [Export]
    public int VisionRange { get; set; } = 4;

    public VisionComponent _area { get; set; }

    public RecruitmentComponent RecruitmentComponent { get; set; }

    public StateMachine StateMach { get; set; }

    public OrderQueue OrderQueue { get; set; } = new OrderQueue();

    //public CmdPanel.CmdPanelOption Task { get; set; } = CmdPanel.CmdPanelOption.None;

    public enum ArmyStance
    {
        Idle,
        Moving,
        Rectruiting
    }

    public ArmyStance Stance { get; set; } = ArmyStance.Idle;

    public bool CanMove { get; set; } = true;
    public Vector3 AngularVelocity { get; set; }
    public BuildingManager BuildingManager { get; set; }
    public RecruitmentManager RecruitmentManager { get; set; }

    // protected void UpdateLinearVelocity(PhysicsDirectBodyState3D state){
    //     // was GlobalTransform.Basis.XForm (new Vector3(0, 0, 1)
    //         state.LinearVelocity = _velocityController.GetAcceleratedVelocity(GlobalTransform.Basis * (new Vector3(0, 0, 1)),GlobalTransform.Origin,OrderQueue.currentTarget.Point);
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
    //     if(OrderQueue.HasTarget){
    //         return (OrderQueue.currentTarget.Point-Transform.Origin).Normalized();
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

    void _on_UpdateRecruitment(RecruitmentComponent recruitmentComponent)
    {
        CanMove = recruitmentComponent.CurrentlyRecruitedUnits.Count == 0;
        StateMach.Enter(new IdleState(this));
        Stance = ArmyStance.Rectruiting;
        //LookAt(AngularVelocity);
    }

    public void _on_UpdateUnitsToTransfer(List<Unit> UnitsToTransfer){
        CanMove = UnitsToTransfer.Count == 0;
        if (!CanMove)
        {
            StateMach.Enter(new IdleState(this));
            Stance = ArmyStance.Idle;
        }
    }

    void Split(OrderQueue.Target target)
    {
        UnitController.Split(target, Controller);
    }
    
    void _on_ExitState(OrderQueue.Target target)
    {
        SimpleFireControl.Stop();
    }

    void Merge(OrderQueue.Target target){
        if (target.TargetNode is IEnterCombat ship && target.TargetNode != this)
        {
            if (ship.Controller.PlayerID == Controller.PlayerID)
            {
                if (UnitController.Count + ship.UnitController.Count < ship.UnitController.MaxUnits)
                {
                    UnitController.TransferUnit(ship.UnitController);
                    if (Selection.Visible) ship.InputController.SelectUnitInvoke();
                    FreeShipInvoke();
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
        SimpleFireControl.Stop();
    }

    public void SelectMapObject()
    {
        InputController.SelectUnitInvoke();
    }

    public void Merge(Ship ship){
        foreach(Unit unit in ship.UnitController.UnitsList){
            unit.GetParent().RemoveChild(unit);
            UnitController.AddUnit(unit);
        }
        ship.Controller.RemoveMapObject(ship);
        ArmiesChanged?.Invoke();
        FreeShipInvoke();
        //ship.QueueFree();
    }

    public void ClearTargets()
    {
        OrderQueue.ClearTargets();
    }

    public void MoveToTarget(OrderQueue.Target target)
    {
        // Sleeping = false;
        if (target.TargetNode != this)
            if (CanMove)
            {
                OrderQueue.SetTarget(target);
                var moveState = new MoveState(target);
                StateMach.Enter(moveState);
                moveState.MoveStateExited += Merge;
                Stance = ArmyStance.Moving;
                SimpleFireControl.Start();
            }
            else
            {
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
        //     OrderQueue.SetTarget(new OrderQueue.Target(planet.Transform.Origin, planet));    // set temp target to leave orbit
        //     OrderQueue.AddTarget(new OrderQueue.Target(planet.Transform.Origin, planet));
        //     return;
        // }
        // if(target.GetParent() != starSysObj){   // check if target is in the same map object (f.e. star system)
        //     if(starSysObj.GetParent() is StarSystem system){

        //         // leak
        //         var tempTarget = GetTempWaypoint((target.Transform.Origin-system.Transform.Origin).Normalized()*(((float)system.Radius)*1.2f));
        //         OrderQueue.SetTarget(tempTarget);    // set temp target to leave star system
        //         // if(target.GetParent().GetParent() is StarSystem targetSystem){
        //         //     OrderQueue.AddTarget(targetSystem);
        //         // }
        //         OrderQueue.AddTarget(target);
        //         return;
        //     }else{
        //         if(target.GetParent().GetParent() is StarSystem targetSystem){
        //             OrderQueue.SetTarget(targetSystem);
        //         }
        //         OrderQueue.AddTarget(target);
        //         return;
        //     }
        // }
        //OrderQueue.SetTarget(target); // if target is in the same map object (f.e. star system) set as target
    }

    public void MoveToPosition(Vector3 destination){
        // Sleeping = false;
        if(CanMove){
            OrderQueue.SetTarget(new OrderQueue.Target(destination));
            var moveState = new MoveState(destination);
            StateMach.Enter(moveState);
            SimpleFireControl.Start();
            moveState.MoveStateExited += _on_ExitState;
        }else{
            Split(new OrderQueue.Target(destination));
        }
    }

    // public void NextTarget(){
    //     if(OrderQueue.currentTarget.TargetNode is StaticBody3D body){
    //         if(body.Name == "tempTarget"){
    //             body.QueueFree();
    //             OrderQueue.NextTarget();
    //             return;
    //         }
    //     }
    //     OrderQueue.NextTarget();
    // }

    // void UpdateShipVelocities(PhysicsDirectBodyState3D state , Vector3 targetPos){
    //     if(OrderQueue.HasTarget){
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
    //     if(OrderQueue.HasTarget){
    //         Vector3 targetPos = Vector3.Zero;
    //         if(OrderQueue.currentTarget.TargetNode is PhysicsBody3D){
    //             targetPos = OrderQueue.currentTarget.Point;
    //         }else{
    //             targetPos = OrderQueue.currentTarget.Point;
    //         }


    //         if(OrderQueue.currentTarget.TargetNode is IEnterMapObject targetObject && (targetPos - GlobalTransform.Origin).Length()<2){
    //             if(MapObject != targetObject){
    //                 if(targetObject is Node someNode)
    //                     EmitSignal(nameof(SignalEnterMapObjectEventHandler), this, someNode, DirToCurrentTarget(), state);
    //                 MapObject = targetObject;
    //             }
    //             NextTarget();
    //         }
    //         if(MapObject != null){
    //             if(MapObject != OrderQueue.currentTarget.TargetNode){
    //                 if(MapObject is Node someNode)
    //                     EmitSignal(nameof(SignalExitMapObjectEventHandler), this, someNode, DirToCurrentTarget(), state);
    //             }
    //         }
    //         if(OrderQueue.currentTarget.TargetNode is Ship ship && (targetPos - GlobalTransform.Origin).Length()<2){
    //             if(ship.Controller != Controller){
    //                 EmitSignal(nameof(EnterCombatEventHandler), (PhysicsBody3D)this, (PhysicsBody3D)ship, GetParent());
    //             }else{
    //                 EmitSignal(nameof(OpenUnitTransferPanelEventHandler), this, ship);
    //             }
    //             OrderQueue.ClearTargets();
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

    void UpdateMesh_onAddUnit(Unit unit) {
        if (unit.ModelVolume > CurrentModelVoluume)
        {
            CurrentModelVoluume = unit.ModelVolume;
            Mesh.Mesh = ModelLoader.GetMiniMeshInstance3D(unit.ModelName).Mesh;
            SimpleFireControl.UpdateEmitters(ModelLoader.GetMiniFireEmmiters(unit.ModelName));

        }
    }

    void UpdateMesh_onRemoveUnit(List<Unit> units) {
        foreach (var unit in units)
        {
            UpdateMesh_onAddUnit(unit);
        }
    }

    public void ChangeVision(VisibilityConroller.VisibilityStruct visibilityStruct, int playerID){
        VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct(){Visibility = VisibilityConroller.VisibilityState.Visible, Visible = true}, playerID);
    }

    void GetNodes(){
        _area = GetNode<VisionComponent>("Area3D");
        VisibilityConroller = GetNode<VisibilityConroller>("VisibilityConroller");
        Mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        StateMach = GetNode<StateMachine>("StateMachine");
        //_velocityController = GetNode<VelocityController>("VelocityController");
        UnitController = GetNode<UnitController>("UnitController");
        RecruitmentComponent = GetNode<RecruitmentComponent>("RecruitmentComponent");
        Selection = GetNode<SelectionCircleControler>("Selection");
        InputController = GetNode<InputController>("InputController");
        ControllerComponent = GetNode<ControllerComponent>("ControllerComponent");
        SimpleFireControl = GetNode<SimpleFireControl>("SimpleFireControl");
        AddChild(OrderQueue);
    }

    public void ConnectToEnterCombat(Node node, string methodName){
         Connect("EnterCombatEventHandler", new Callable(node, methodName));
    }

    public override void _Ready()
    {
        GetNodes();
        StateMach.Body = this;
        StateMach.Enter(new IdleState(this));
        RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent += _on_UpdateRecruitment;
        //_velocityController.Mass = 10;
        _area.UpdateVisionRange(VisionRange);
        UnitController.NoUnits += FreeShipInvoke;
        UnitController.UnitAdded += UpdateMesh_onAddUnit;
        UnitController.UnitsRemoved += UpdateMesh_onRemoveUnit;
        UnitController.UnitsToTransferChanged += _on_UpdateUnitsToTransfer;
        RecruitmentComponent.UnitController = UnitController;
    }

    public void UpdatePower(){
        Power = 0;
        foreach(var unit in UnitController.UnitsList){
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

    void FreeShipInvoke()
    {
        UnitController.Clear();
        SimpleFireControl.ClearEmitters();
        FreeShip?.Invoke(this);
    }


    // public bool IsVisible()
    // {
    //     return Visible;
    // }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
     public override void _Process(double delta)
     {
        MoveAndSlide();
     }

    public void UpdateVelocity(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Velocity = linearVelocity;
    }
}
