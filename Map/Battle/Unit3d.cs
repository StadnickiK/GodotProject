using Godot;
using System;
using System.Linq;


public class Unit3dModel : Node3DModel, IMapObjectController
{
    public Player Controller { get; set; }

}

public interface ICardIndex
{
    public Player Controller { get; set; }
    public int CardIndex { get; set; }
}

public partial class Unit3D : RigidBody3D, IMapObjectController, ITargetable, IMovable, IProjectileFactory, IDamagable, ISavingNode, ICardIndex, IStatManager, IInputController
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

    public event Node3DPool.SaveNodeEventHandler SaveNode;

    public int CardIndex { get; set; }

    public CollisionObject3D GetAsSpecificNode { get {return this;} }


    public override void _Ready()
    {
        GetNodes();
        UpdateController();
        StateMachine.Body = this;
        StateMachine.Enter(new IdleState(this));
        TurretControl.Source = this;
    }

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
    }

    public Vector3 GetUnitSize()
    {
        return MeshInstance3D.GetAabb().Size;
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
        switch (MovableState)
        {
            case IMovableState.Placement:
                SetPosition(target.Point);
                break;
            default:
                var state = new PhysicsMoveState(target);
                state.MoveStateExited += OnExitPhysicsMoveState;
                SimpleFireControl.Start();
                StateMachine.Enter(state);

                break;
        }
    }


    void OnExitPhysicsMoveState(OrderQueue.Target target)
    {
        SimpleFireControl.Stop();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndCollide(LinearVelocity * (float)delta);
    }


    public void UpdateVelocity(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        throw new NotImplementedException();
    }

    new public void SetPosition(Vector3 Vector3)
    {
        var g = GlobalTransform;
        g.Origin = Vector3;
        GlobalTransform = g;
    }

    public void UpdateModel(Unit unit)
    {
        MeshInstance3D.Mesh = unit.ModelData.MeshInstance3D.Mesh;
        CollisionShape3D.Shape = unit.ModelData.CollisionShape3D.Shape;
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
}
