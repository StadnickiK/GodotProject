using Godot;
using System;


public class Unit3dModel : Node3DModel, IMapObjectController
{
    public Player Controller { get; set; }

}

public partial class Unit3D : RigidBody3D, IMapObjectController, IVisible, IMovable
{

    public VisibilityConroller VisibilityConroller { get; set; }

    public Player Controller { get; set; }
    public MeshInstance3D MeshInstance3D { get; set; }
    public CollisionShape3D CollisionShape3D { get; set; }

    public InputController InputController { get; set; }

    public IMovableState MovableState { get; set; } = IMovableState.Movement;

    StateMachine StateMachine { get; set; }
    public OrderQueue OrderQueue { get; set; }
    public Vector3 Velocity { get => LinearVelocity; set => LinearVelocity = value; }

    public override void _Ready()
    {
        GetNodes();
        StateMachine.Body = this;
        StateMachine.Enter(new IdleState(this));
    }

    private void GetNodes()
    {
        MeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
        CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        VisibilityConroller = GetNode<VisibilityConroller>("VisibilityConroller");
        InputController = GetNode<InputController>("InputController");
        OrderQueue = GetNode<OrderQueue>("TargetManager");
        StateMachine = GetNode<StateMachine>("StateMachine");
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
                StateMachine.Enter(new PhysicsMoveState(target));
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
}
