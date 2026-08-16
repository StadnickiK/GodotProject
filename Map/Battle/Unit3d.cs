using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


public class Unit3dModel : Node3DModel, IMapObjectController
{
    public Player Controller { get; set; }

}

public interface ICardIndex : IMapObjectController
{
    public int CardIndex { get; set; }
}

public partial class Unit3D : RigidBody3D, IUnit3D, IProjectileFactory, IDamagable, ISavingNode, IStatManager, IInputController
{
    private Player controller;
    private ProjectileFactory projectileFactory;

    public VisibilityConroller VisibilityConroller { get; set; }

    public Player Controller { get => controller; set { controller = value; UpdateController(); } }
    public MeshInstance3D MeshInstance3D { get; set; }
    public CollisionShape3D CollisionShape3D { get; set; }

    public Node GetAsNode { get {return this;} }

    public InputController InputController { get; set; }

    public IMovableState MovableState { get; set; } = IMovableState.Movement;

    StateMachine StateMachine { get; set; }
    public OrderQueue OrderQueue { get; set; }
    public Vector3 Velocity { get => LinearVelocity; set => LinearVelocity = value; }

    public SelectionCircleControler Selection { get; set; }

    public HealthBar3D HealthBar { get; set; }

    public StatManager StatManager { get; set; }

    public ProjectileFactory ProjectileFactory { get => projectileFactory; set { projectileFactory = value; UpdateProjectileFactory(); } }

    public Shield Shield { get; set; }

    public DeathController DeathController { get; set; }

    public SimpleFireControl SimpleFireControl { get; set; }

    TurretControl TurretControl;

    // ObstacleDetectionComponent ObstacleAvoidance;

    public NavAgent3d NavAgent3D { get; set; }

    public event Node3DPool.SaveNodeEventHandler SaveNode;
    public event DrawingLines3DController.SpawnOrderMarkersHandler SpawnMarkers;
    public event DrawingLines3DController.SpawnOrderMarkersHandler ClearMarkers;

    // public event DrawingLines3DController.RedrawOrderMarkersHandler RedrawMarkers;

    public int CardIndex { get; set; }

    public CollisionObject3D GetAsSpecificNode { get {return this;} }

    public VelocityController VelocityController { get; private set; }


    public override void _Ready()
    {
        GetNodes();
        UpdateController();
        StateMachine.Body = this;
        StateMachine.Enter(new IdleState(this));
        TurretControl.Source = this;
        InputController.Initialize(this, this, Shield);
        NavAgent3D.VelocityComputed += OnVelocityComputed;
        //NavAgent3D.PathChanged += OnPathChanged;
        NavAgent3D.NavigationFinished += OnNavigationFinished;
    }

    private void OnVelocityComputed(Vector3 safeVelocity)
    {
        LinearVelocity = safeVelocity;
    }

    private void OnPathChanged()
    {
        // Vector3 nextPathPosition = NavAgent3D.GetNextPathPosition();
        // EnterState(new OrderQueue.Target(nextPathPosition));
    }

    void OnNavigationFinished()
    {
        
    }

    // void _on_navigation_target_reached()
    // {
    //     ClearMarkers?.Invoke(this);
    //     SimpleFireControl.Stop();
    // }

    void UpdateController()
    {
        foreach (var item in GetChildren())
            if (item is IMapObjectController mapObjectController)
                mapObjectController.Controller = Controller;
    }

    void UpdateProjectileFactory()
    {
        foreach (var item in GetChildren())
            if (item is IProjectileFactory mapObjectController)
                mapObjectController.ProjectileFactory = ProjectileFactory;
    }

    private void GetNodes()
    {
        MeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
        CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        VisibilityConroller = GetNode<VisibilityConroller>("VisibilityConroller");
        InputController = GetNode<InputController>("InputController");
        OrderQueue = GetNode<OrderQueue>("TargetManager");
        StateMachine = GetNode<StateMachine>("StateMachine");
        Selection = GetNode<SelectionCircleControler>("Selection");
        HealthBar = GetNode<HealthBar3D>("HealthBar");
        TurretControl = GetNode<TurretControl>("TurretControl");
        Shield = GetNode<Shield>("Shield");
        StatManager = GetNode<StatManager>("StatManager");
        DeathController = GetNode<DeathController>("DeathController");
        SimpleFireControl = GetNode<SimpleFireControl>("SimpleFireControl");
        // ObstacleAvoidance = GetNode<ObstacleDetectionComponent>("ObstacleDetectionComponent");
        VelocityController = GetNode<VelocityController>("VelocityController");
        NavAgent3D = GetNode<NavAgent3d>("NavigationAgent3D");

    }

    public void LoadUnit(IStatManager unit)
    {
        StatManager.CloneStats(unit);
        StatManager.ConnectStatListeners(this);
        StatManager.UpdateListeners();
        if (Shield != null)
            Shield.StatManager = StatManager;
    }

    public void ChangeMovableState(IMovableState movableState)
    {
        MovableState = movableState;
        switch (movableState)
        {
            case IMovableState.Placement:
                InputController.ChangeMode(InputController.InputMode.Drag);
                NavAgent3D.AvoidanceEnabled = false;
                TurretControl.Fire = false;
            break;
            default:
                InputController.ChangeMode(InputController.InputMode.Select);
                NavAgent3D.AvoidanceEnabled = true;
                TurretControl.Fire = true;
            break;
        }
    }

    public Vector3 GetUnitSize()
    {
        return MeshInstance3D.GetAabb().Size;
    }

    public Aabb GetAabb()
    {
        return MeshInstance3D.GetAabb();
    }

    public void ChangeVision(VisibilityConroller.VisibilityStruct visibilityStruct, int playerID)
    {
        throw new NotImplementedException();
    }

    public int ReturnIndex()
    {
        return GetIndex();
    }

    public void SetVisibility(bool visible)
    {
        Visible = visible;
    }

    public bool ReturnVisible()
    {
        return Visible;
    }

    public void MoveToTarget(OrderQueue.Target target)
    {
        OrderQueue.SetTarget(target);
        EnterState(target);
    }

    public void ClearTargets()
    {
        OrderQueue.ClearTargets();
    }

    public void MoveToPosition(Vector3 destination)
    {
        OrderQueue.SetTarget(new OrderQueue.Target(destination));
        EnterState(new OrderQueue.Target(destination));
    }

    void EnterState(OrderQueue.Target target)
    {
        State<IMovable> state;
        switch (MovableState)
        {
            case IMovableState.Placement:
                SetPosition(target.Point);
                break;
            default:
                NavAgent3D.TargetPosition = target.Point;
                // if(target.TargetNode is IMovable movable)
                //     state = new PhysicsPursuitState(movable, ObstacleAvoidance, VelocityController);
                // else
                //var pos = NavAgent3D.GetNextPathPosition();
                // state = new PhysicsMoveState(new OrderQueue.Target(pos), NavAgent3D, VelocityController);
                state = new PhysicsMoveState(target, NavAgent3D, VelocityController);
                // state.MoveStateExited += OnExitPhysicsMoveState;
                SimpleFireControl.Start();
                StateMachine.Enter(state);
                SpawnMarkers?.Invoke(this);
            break;
        }
    }


    void OnExitPhysicsMoveState(OrderQueue.Target target)
    {
        ClearMarkers?.Invoke(this);
        SimpleFireControl.Stop();
    }
    double time = 0;

    public override void _PhysicsProcess(double delta)
    {
        // // Do not query when the map has never synchronized and is empty.
        // if (NavigationServer3D.MapGetIterationId(NavAgent3D.GetNavigationMap()) == 0)
        // {
        //     return;
        // }

        // if (NavAgent3D.IsNavigationFinished())
        // {
        //     LinearVelocity = Vector3.Zero;
        //     return;
        // }
        // LinearVelocity = VelocityController.GetSteering(LinearVelocity);
        // AngularVelocity = VelocityController.GetAngularVelocity(AngularVelocity);
        // Vector3 nextPathPosition = NavAgent3D.GetNextPathPosition();
        // EnterState(new OrderQueue.Target(nextPathPosition));
        //Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * VelocityController.Stats[GlobalStatNames.Speed].CurrentValue;
        if (NavAgent3D.AvoidanceEnabled)
        {
            NavAgent3D.Velocity = LinearVelocity;
            // LinearVelocity = newVelocity;
            // NavAgent3D.Velocity = newVelocity;
        }
        else
        {
            OnVelocityComputed(LinearVelocity);
        }

        if(Selection.Visible && time > 0.2 && MovableState == IMovableState.Movement)
        {
            SpawnMarkers?.Invoke(this);
            time = 0;
        } 
        
        // NavAgent3D.Velocity = LinearVelocity;
        //MoveAndCollide(LinearVelocity);
        // NavAgent3D.Velocity = LinearVelocity * (float)delta;
        // MoveAndCollide(LinearVelocity * (float)delta);
        time += delta; 
    }

    // void _on_navigation_path_changed()
    // {
    //     Vector3 destination = NavAgent3D.GetNextPathPosition();
    //     GD.Print("Nav path changed Dest " + destination + " B pos " + Position);
    //     EnterState(new OrderQueue.Target(destination));
    //     if(Selection.Visible) SpawnMarkers?.Invoke(this);
    // }



    public void UpdateVelocity(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        LinearVelocity = linearVelocity;
        AngularVelocity = angularVelocity;
    }

    new public void SetPosition(Vector3 Vector3)
    {
        var g = GlobalTransform;
        g.Origin = Vector3;
        GlobalTransform = g;
    }

    public void UpdateModel(Unit unit)
    {
        //MeshInstance3D.Mesh?.Free();
        MeshInstance3D.Mesh = (Mesh)unit.ModelData.MeshInstance3D.Mesh.Duplicate();
        //CollisionShape3D.Shape?.Free();
        CollisionShape3D.Shape = (Shape3D)unit.ModelData.CollisionShape3D.Shape.Duplicate();
        var meshSize = MeshInstance3D.Mesh.GetAabb().Size;
        var size = meshSize.Z > meshSize.X ? meshSize.Z : meshSize.X;
        Selection.Size = new Vector2(size, size);
        HealthBar.UpdatePosition(meshSize);
        Shield.UpdateRadius(size);
        SimpleFireControl.UpdateEmitters(unit.ModelData.FireEmitters, 2f);
        if (StatManager.HasStat("FireLifetime"))
            {
                SimpleFireControl.UpdateEmitters(unit.ModelData.FireEmitters, StatManager.GetStatCurrentValue("FireLifetime"));
            }else
                SimpleFireControl.UpdateEmitters(unit.ModelData.FireEmitters);
        TurretControl.InitTurrets(unit);
    }

    public void Damage()
    {
        throw new System.NotImplementedException();
    }

    public void InvokeSaveNode()
    {
        SaveNode?.Invoke(this);
    }

    public void BeforeSave()
    {
        OrderQueue?.ClearTargets();
        StateMachine?.Enter(new IdleState(this));
    }

    public void Initialize(UnitFactory unitFactory, Node parent, Vector3 position, Player controller, Unit unit)
    {
        unitFactory.ConnectSignals(this);
        LoadUnit(unit);
        ProjectileFactory = unitFactory.ProjectileFactory;
        UpdateModel(unit);
        // ObstacleAvoidance.Initialize(
        //     this, 
        //     GetUnitSize(), 
        //     VelocityController.Stats[GlobalStatNames.Speed].CurrentValue, 
        //     VelocityController.Stats[GlobalStatNames.RotationSpeed].CurrentValue);
        NavAgent3D.Initialize(this, GetUnitSize(), StatManager);
        InputController.Drag -= unitFactory.UnitDragHandler.OnDrag;
        InputController.Drag += unitFactory.UnitDragHandler.OnDrag;
        InputController.Drag -= unitFactory.WorldCursorControl.OnDrag;
        InputController.Drag += unitFactory.WorldCursorControl.OnDrag;
    }

    public void Initialize(Node parent, Vector3 position, Player controller,Unit unit)
    {
        LoadUnit(unit);
        UpdateModel(unit);
        // ObstacleAvoidance.Initialize(
        //     this, 
        //     GetUnitSize(), 
        //     VelocityController.Stats[GlobalStatNames.Speed].CurrentValue, 
        //     VelocityController.Stats[GlobalStatNames.RotationSpeed].CurrentValue);
        NavAgent3D.Initialize(this, GetUnitSize(), StatManager);
    }

    public void Select()
    {
        Selection?.Show();
        InputController.Selected = true;
        if(MovableState == IMovableState.Movement)
            SpawnMarkers?.Invoke(this);
    }

    public void Deselect()
    {
        Selection?.Hide();
        InputController.Selected = false;
        ClearMarkers?.Invoke(this);
    }

    public void UpdatePosition(Vector3 position, float yRotation = 0)
    {
        GlobalTransform = new Transform3D(GlobalBasis, position);
        GlobalRotation = new Vector3(0, yRotation, 0);
    }

}
