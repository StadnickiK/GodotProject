using Godot;
using System;
using System.Linq;


public class Unit3dModel : Node3DModel, IMapObjectController
{
    public Player Controller { get; set; }

}

public partial class Unit3D : RigidBody3D, IMapObjectController, ITargetable, IMovable, IProjectileFactory
{
    private Player controller;
    private ProjectileFactory projectileFactory;

    public VisibilityConroller VisibilityConroller { get; set; }

    public Player Controller { get => controller; set { controller = value; UpdateController(); } }
    public MeshInstance3D MeshInstance3D { get; set; }
    public CollisionShape3D CollisionShape3D { get; set; }

    public InputController InputController { get; set; }

    public IMovableState MovableState { get; set; } = IMovableState.Movement;

    StateMachine StateMachine { get; set; }
    public OrderQueue OrderQueue { get; set; }
    public Vector3 Velocity { get => LinearVelocity; set => LinearVelocity = value; }

    public SelectionCircleControler Selection { get; set; }

    public StatManager StatManager { get; set; }
    public ProjectileFactory ProjectileFactory { get => projectileFactory; set { projectileFactory = value; UpdateProjectileFactory(); } }


    public override void _Ready()
    {
        GetNodes();
        UpdateController();
        StateMachine.Body = this;
        StateMachine.Enter(new IdleState(this));
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
    }

    public void LoadUnit(IStatManager unit)
    {
        StatManager = unit.StatManager;
        StatManager.ConnectStatListeners(this);
        StatManager.UpdateListeners();
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
                state.MoveSpeed = StatManager.GetStat("Speed").CurrentValue;
                if (target.TargetNode != null)
                    if (target.TargetNode is IMapObjectController controller)
                        if (controller.Controller.PlayerID != Controller.PlayerID)
                        {
                            state.Tolerance = StatManager.GetStat("Range").CurrentValue;
                        }
                    StatManager.Stats[state.StatNames.FirstOrDefault()].StatChanged += state.UpdateStat;
                    StateMachine.Enter(state);
                
                break;
        }
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

    public void UpdateModel(Mesh mesh, Shape3D collisionShape)
    {
        MeshInstance3D.Mesh = mesh;
        CollisionShape3D.Shape = collisionShape;
        var meshSize = MeshInstance3D.Mesh.GetAabb().Size;
        var size = meshSize.Z > meshSize.X ? meshSize.Z : meshSize.X;
        Selection.Size = new Vector2(size, size);
    }
}
