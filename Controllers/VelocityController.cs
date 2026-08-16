using Godot;
using System;
using System.Collections.Generic;

public partial class VelocityController : Node3D, IUpdateStat, IVisionComponentListener
{

    [Export]
    public float Acceleration { get; set; } = 1;
    [Export]
    public Vector3 Forward { get; set; } = new Vector3(0,0,-1);

    [Export]
    public Vector3 Up { get; set; } = new Vector3(0,1,0);

    [Export]
    public Godot.Collections.Array<string> StatNames { get; set; } = new Godot.Collections.Array<string>() { 
        GlobalStatNames.RotationSpeed,
        GlobalStatNames.Speed,
        GlobalStatNames.RotationTolerance,
        GlobalStatNames.Tolerance,
        GlobalStatNames.SeparationRadius,
        GlobalStatNames.SeparationStrength,
        GlobalStatNames.CohesionStrength,
        GlobalStatNames.CohesionRadius,
        GlobalStatNames.AlignmentStrength,
        GlobalStatNames.AlignmentRadius,
        GlobalStatNames.Range
    };

    public StatManager StatManager { get; set; }

    HashSet<IMovable> Neighbors = new HashSet<IMovable>();

    IMovable Body;

    Vector3 ParentSize;

    public Vector3 SeparationForce { get; set; } = Vector3.Zero;
    public Vector3 AligmentForce { get; set; } = Vector3.Zero;
    public Vector3 AvoidanceForce { get; set; } = Vector3.Zero;
    public Dictionary<string, IStat<float>> Stats { get; set; }

    public VelocityController(){}

    public void Initialize(IMovable movable, StatManager statManager, Vector3 Size)
    {
        StatManager = statManager;
        Body = movable;
        ParentSize = Size;
        foreach (var item in StatNames)
            Stats.Add(item, statManager.GetStat<float>(item));
        
    }

    static public Vector3 GetTargetDir(Vector3 currentPosition, Vector3 targetPosition){
        return (targetPosition - currentPosition).Normalized();
    }

    static public Vector2 GetTargetDir(Vector2 currentPosition, Vector2 targetPosition){
        return (targetPosition - currentPosition).Normalized();
    }

    static public float GetAngleYToTarget(Transform3D transform, Vector3 targetPosition){
        Vector3 dir = transform.Basis * -Vector3.Forward;
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float angle = Mathf.Atan2(targetDir.X,targetDir.Z) - Mathf.Atan2(dir.X,dir.Z);
        return angle;
    }

    public float GetAngleXToTarget(Transform3D transform, Vector3 targetPosition)
    {
        Vector3 dir = transform.Basis * -Vector3.Forward;
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float angle = Mathf.Atan2(targetDir.Y,targetDir.Z) - Mathf.Atan2(dir.Y,dir.Z);
        return angle;
    }

    public Vector3 GetAngleToTarget(Transform3D transform, Vector3 targetPosition)
    {
        Vector3 dir = transform.Basis * -Vector3.Forward;
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float y = Mathf.Atan2(targetDir.Y,targetDir.Z) - Mathf.Atan2(dir.Y,dir.Z);
        float x = Mathf.Atan2(targetDir.Y,targetDir.Z) - Mathf.Atan2(dir.Y,dir.Z);
        float z = Mathf.Atan2(targetDir.X, targetDir.Y) - Mathf.Atan2(dir.X, dir.Y);
        return new Vector3(x, y, z);
    }


    static public Vector3 GetQuaternionToTarget(Transform3D transform, Vector3 targetPosition)
    {
        Vector3 dir = (transform.Basis * -Vector3.Forward).Normalized();
        Vector3 targetDir = GetTargetDir(targetPosition, transform.Origin).Normalized();

        // rotation from current forward to target direction
        Quaternion q = new Quaternion(dir, targetDir);

        // convert to Euler angles (in radians)
        Vector3 angles = q.GetEuler();

        return angles;
    }

    static public Vector3 GetQuaternionInDir(Transform3D transform, Vector3 targetDir)
    {
        Vector3 dir = (transform.Basis * -Vector3.Forward).Normalized();
        targetDir = targetDir.Normalized();

        // rotation from current forward to target direction
        Quaternion q = new Quaternion(dir, targetDir);

        // convert to Euler angles (in radians)
        Vector3 angles = q.GetEuler();

        return angles;
    }

    float ang;
    //
    //  Summary:
    //      Returns angular velocity required to partialy rotate on y axis towards position.
    //      The default forward facing direction is on positive z axis Vector3(0,0,1)
    public Vector3 GetAngularVelocity(Transform3D currentTransform, Vector3 targetPosition)
	{
        Vector3 upDir = new Vector3(0, 1, 0);
        // var angle = GetQuaternionToTarget(currentTransform, targetPosition);
        var angle = GetQuaternionToTarget(currentTransform, targetPosition);

        var velocity = GetAngularVelocity(angle.Y, upDir) + GetAngularVelocity(angle.X, new Vector3(1,0,0));
        velocity *= new Vector3(1,1,0);
        return velocity;
	}

    public Vector3 GetAngularVelocity(float angle, Vector3 plane)
	{
        if(angle < Stats[GlobalStatNames.RotationTolerance].CurrentValue && angle > -Stats[GlobalStatNames.RotationTolerance].CurrentValue ){
            GD.Print("Tolerance reached " + angle);
            return Vector3.Zero;
        }

        var rad = -Mathf.DegToRad(Stats[GlobalStatNames.RotationSpeed].CurrentValue);

        if (angle > Math.PI)       { angle -= 2 * (float)Math.PI; }
        else if (angle < -Math.PI) { angle += 2 * (float)Math.PI; }

        if (angle >= 0)  rad *= -1; 

        var speed = angle / 0.6f;
        speed = Mathf.Min(speed, rad);

        // if(((targetPosition-currentTransform.Origin).Length() < 1 && (angle > RotationTolerance || angle < -RotationTolerance )) || Mathf.Abs(rad) > Mathf.Abs(angle))
        //     rad = angle;
        
        return plane * speed;
	}

    private Vector3 CalculateLinearVelocity(OrderQueue.Target Target)
    {
        Vector3 dir = Body.GlobalTransform.Basis * Vector3.Forward;
        Vector3 toTarget = Target.Point - Body.GlobalPosition;

        // Prevent tiny jitter if already at target
        var len = toTarget.Length();
        if(len < 5)
            GD.Print("T pos " + Target.Point + " B pos " + Body.GlobalPosition + " " + len + " " + ang);
        if (len < Stats[GlobalStatNames.Tolerance].CurrentValue)
        {
            return Vector3.Zero;
        }
        var speed = len / 0.6f;
        speed = Mathf.Min(speed, Stats[GlobalStatNames.Speed].CurrentValue);

        return dir.Normalized() * speed;
    }

    public Vector3 GetAvoidanceAngularVelocity(Transform3D currentTransform, Aabb ClosestIntersection, ICollider3D ClosestCollider)
	{
        Vector3 upDir = new Vector3(0, 1, 0);
        
        var offset = ClosestIntersection.Position - ClosestCollider.Position + new Vector3(ParentSize.X,0,0);
        
        var angle = GetQuaternionToTarget(currentTransform, ClosestIntersection.Position + offset)  * new Vector3(1,1,0);

        var velocity = GetAngularVelocity(angle.Y, upDir) + GetAngularVelocity(angle.X, new Vector3(1,0,0));
        
        velocity *= new Vector3(1,1,0);

        return velocity;
	}

    private Vector3 GetSeparationForce()
    {
        Vector3 force = Vector3.Zero;
        int count = 0;

        foreach (var other in Neighbors)
        {
            float dist = GlobalPosition.DistanceTo(other.GlobalPosition);
            var SeparationRadius = Stats[GlobalStatNames.SeparationRadius].CurrentValue;
            
            if (dist > 0 && dist < SeparationRadius)
            {
                force += (GlobalPosition - other.GlobalPosition).Normalized() * (SeparationRadius - dist);
                count++;
            }
        }

        var result = force.Normalized() * Stats[GlobalStatNames.SeparationStrength].CurrentValue;
        return count > 0 ? result : Vector3.Zero;
    }

    private Vector3 CohesionForce()
    {
        if (Neighbors.Count == 0) return Vector3.Zero;

        Vector3 center = Vector3.Zero;
        int count = 0;
        foreach (var other in Neighbors){
            float dist = GlobalPosition.DistanceTo(other.GlobalPosition);
            if(dist < Stats[GlobalStatNames.CohesionRadius].CurrentValue){
                center += other.GlobalPosition;
                count ++;
            }
        }
        if(count == 0) return Vector3.Zero;
        center /= count;

        Vector3 direction = (center - GlobalPosition).Normalized();
        return direction * Stats[GlobalStatNames.CohesionStrength].CurrentValue;
    }

    private Vector3 StabilizationForce()
    {
        var force = Vector3.Zero;
        var z = GlobalTransform.Basis.GetEuler().Z;
        var Tolerance = Stats[GlobalStatNames.RotationTolerance].CurrentValue;
        if( z > Tolerance ||
            z < -Tolerance)
        {
            force = GetAngularVelocity(z, Vector3.Forward);
        }
        return force;
    }

    private Vector3 GetAligmentForce()
    {
        if (Neighbors.Count == 0) return Vector3.Zero;

        Vector3 avgVel = Vector3.Zero;
        foreach (var other in Neighbors)
        {
            float dist = GlobalPosition.DistanceTo(other.GlobalPosition);
            if(dist < Stats[GlobalStatNames.AlignmentRadius].CurrentValue)
                avgVel += other.Velocity;
        }
        
        avgVel /= Neighbors.Count;
        return avgVel.Normalized() * Stats[GlobalStatNames.AlignmentStrength].CurrentValue;
    }

    public Vector3 GetSteering(Vector3 Velocity)
    {
        // SeparationForce = GetSeparationForce();
        // //var cohesion = CohesionForce();
        // AligmentForce = GetAligmentForce();
        // Vector3 steering =
        //     SeparationForce +
        //     //cohesion +
        //     AligmentForce;
        var result = Vector3.Zero;
        // steering = steering.Clamp(-Stats[GlobalStatNames.Speed].CurrentValue, Stats[GlobalStatNames.Speed].CurrentValue);
        // var result = (Velocity + steering).Clamp(-Stats[GlobalStatNames.Speed].CurrentValue, Stats[GlobalStatNames.Speed].CurrentValue);
        return result;
    }

    public Vector3 GetAngularVelocity(Vector3 angularVelocity)
    {
        //var stabilizationForce = StabilizationForce();
        var angular = angularVelocity;// + stabilizationForce;
        return angular;
    }

    public void UpdateStat(IStat stat){}

    public void _on_area_body_Entered(Node node)
    {
        if(node is IMovable collider3D && node != GetParent())
        {
            Neighbors.Add(collider3D);
        }
    }

    public void _on_area_body_Exited(Node node)
    {
        if(node is IMovable collider3D)
        {
            Neighbors.Remove(collider3D);
        }
    }
}