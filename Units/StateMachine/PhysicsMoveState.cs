using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class PhysicsMoveState : State<IMovable>, IUpdateStat
{
    private OrderQueue.Target _target;

    public delegate void MoveStateExitedEventHandler(OrderQueue.Target target);

    public event MoveStateExitedEventHandler MoveStateExited;


    [Export]
    public HashSet<string> StatNames { get; set; } = new HashSet<string>() { "Speed" };
    [Export]
    private float _tolerance = 2f;
    [Export]
    private float _rotationTolerance = 0.01f;
    [Export]
    public float MoveSpeed = 5f;         // Units per second
    [Export]
    public float RotationSpeed { get; set; } = 3;
    public OrderQueue.Target Target { get => _target; set => _target = value; }
    public float Tolerance { get => _tolerance; set => _tolerance = value; }

    
    public float RotationTolerance
    {
        get { return _rotationTolerance; }
        set { _rotationTolerance = value; }
    }
    

    /// <summary>
    /// Creates a new state instance with the given target position.
    /// </summary>

    public PhysicsMoveState(){}

    public PhysicsMoveState(OrderQueue.Target target)
    {
        Target = target;
    }

    public PhysicsMoveState(OrderQueue.Target target, float tolerance)
    {
        Target = target;
        Tolerance = tolerance;
    }

    public PhysicsMoveState(Vector3 target)
    {
        Target = new OrderQueue.Target(target);
    }

    public override void Enter(IMovable body)
    {
        base.Enter(body);
        if(body.StatManager != null)
        {
            if(body.StatManager.HasStat("Speed")) MoveSpeed = body.StatManager.GetStatCurrentValue("Speed");
            if(body.StatManager.HasStat("RotationSpeed")) RotationSpeed = body.StatManager.GetStatCurrentValue("RotationSpeed");
            if(body.StatManager.HasStat("Tolerance")) Tolerance = body.StatManager.GetStatCurrentValue("Tolerance");
            if(body.StatManager.HasStat("RotationTolerance")) RotationTolerance = body.StatManager.GetStatCurrentValue("RotationTolerance");
        }
        if (Target.TargetNode != null)
            if (Target.TargetNode is IMapObjectController controller)
                if (controller.Controller.PlayerID != body.Controller.PlayerID)
                {
                    if(body.StatManager.HasStat("Range")) Tolerance = body.StatManager.GetStatCurrentValue("Range");
                }
        body.StatManager.Stats[StatNames.FirstOrDefault()].StatChanged += UpdateStat;
        // Optionally: play a walking or moving animation.
    }

    public override void Exit()
    {
        base.Exit();
        MoveStateExited.Invoke(Target);
    }

    public override State<IMovable> ProcessState(double delta)
    {
        CalculateAngularVelocity();
        return CalculateLinearVelocity();
    }

    private State<IMovable> CalculateLinearVelocity()
    {
        Vector3 toTarget = Target.Point - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        var len = toTarget.Length();
        if (len < Tolerance)
        {
            Body.Velocity = Vector3.Zero;
            Body.OrderQueue.NextTarget();
            if (!Body.OrderQueue.HasTarget)
                return new IdleState(Body);
            return new PhysicsMoveState(Body.OrderQueue.currentTarget);
        }

        Body.Velocity = toTarget.Normalized() * MoveSpeed;
        return this;
    }

    public Vector3 GetTargetDir(Vector3 currentPosition, Vector3 targetPosition){
        return (targetPosition - currentPosition).Normalized();
    }

    public float GetAngleToTarget(Transform3D transform, Vector3 targetPosition){
        Vector3 dir = transform.Basis*new Vector3(0, 0, 1);
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float angle = (Mathf.Atan2(targetDir.X,targetDir.Z) - Mathf.Atan2(dir.X,dir.Z));
        return angle;
    }

    //
    //  Summary:
    //      Returns angular velocity required to partialy rotate on y axis towards position.
    //      The default forward facing direction is on positive z axis Vector3(0,0,1)
    public Vector3 GetAngularVelocity(Transform3D currentTransform, Vector3 targetPosition)
	{
        int rotationFix = 1;

        Vector3 upDir = new Vector3(0, 1, 0);
        float angle = GetAngleToTarget(currentTransform, targetPosition);

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        if((targetPosition-currentTransform.Origin).Length() < 8){
            rotationFix *= 4;
        }

        return upDir*(angle) * RotationSpeed * rotationFix;
	}

    private void CalculateAngularVelocity()
    {
        // 1. Current facing direction (assuming -Z is forward)
        Vector3 forward = -Body.GlobalBasis.Z.Normalized();

        // 2. Desired direction toward target
        Vector3 toTarget = (Target.Point - Body.GlobalPosition).Normalized();

        // 3. If vectors are nearly aligned, don't rotate
        float dot = forward.Dot(toTarget);
        if (dot > 1 - RotationTolerance)
        {
            Body.AngularVelocity = Vector3.Zero;
            return;
        }

        // 4. Compute rotation axis
        Vector3 rotationAxis = forward.Cross(toTarget).Normalized();

        // 5. Angle between current direction and target direction
        float angle = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));

        // 6. Angular velocity: how fast to rotate per second
        //Body.AngularVelocity = (rotationAxis * angle) / Mathf.Max(TimeToRotate, 0.001f);
        Body.AngularVelocity = GetAngularVelocity(Body.GlobalTransform, Target.Point);
    }

    public void UpdateStat(IStat stat)
    {
        MoveSpeed = stat.CurrentValue;
    }
}
