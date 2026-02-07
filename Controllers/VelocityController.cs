using Godot;
using System;
using System.Collections.Generic;

public partial class VelocityController : Node, IUpdateStat
{
    [Export]
    public float Mass { get; set; } = 1;
    [Export]
    public float MaxSpeed { get; set; } = 10;
    [Export]
    public float Speed { get; set; } = 0;
    [Export]
    public float RotationSpeed { get; set; } = 3;
    [Export]
    public float RotationTolerance { get; set; } = 0.01f;

    [Export]
    public float Acceleration { get; set; } = 1;
    [Export]
    public Vector3 Forward { get; set; } = new Vector3(0,0,-1);

    [Export]
    public Vector3 Up { get; set; } = new Vector3(0,1,0);
    public HashSet<string> StatNames { get; set; } = new HashSet<string>() { GlobalStatNames.RotationSpeed };

    public VelocityController(){}
    public VelocityController(float maxSpeed, float acceleration, float mass){
        Mass = mass;
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
    }

    public Vector3 GetAcceleratedVelocity(
            Vector3 currentDirection, 
            Vector3 currentPosition,
            Vector3 targetPos)
            {
        if(Speed <= MaxSpeed){
            Speed += Acceleration;
        }
        Vector3 Velocity = currentDirection * Speed;
        Velocity.Y = 0;
        return Velocity;
    }

    public Vector3 GetAcceleratedVelocity(Vector3 currentPosition, Vector3 targetPos){
        if(Speed <= MaxSpeed){
            Speed += Acceleration;
        }
        Vector3 Velocity = GetTargetDir(currentPosition, targetPos) * Speed;
        Velocity.Y = 0;
        return Velocity;
    }

    public Vector3 GetTargetDir(Vector3 currentPosition, Vector3 targetPosition){
        return (targetPosition - currentPosition).Normalized();
    }

    static public Vector3 GetTransformedPos(Transform3D transform, Vector3 vector){
        return transform.Basis*(vector);
    }

    public float GetAngleToTarget(Transform3D transform, Vector3 targetPosition, Vector3 front, Vector3 atan2Yaxis){
        Vector3 dir = transform.Basis*(front);
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float angle = (Mathf.Atan2((targetDir*atan2Yaxis).Length(),targetDir.Z) - Mathf.Atan2((dir*atan2Yaxis).Length(),dir.Z));
        return angle;
    }

    public float GetAngleToTargetOnXAxis(Transform3D transform, Vector3 targetPosition){
        Vector3 dir = transform.Basis * Forward;
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float angle = Mathf.Atan2(targetDir.Y,targetDir.Z) - Mathf.Atan2(dir.Y,dir.Z);
        
        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }
        //float angle = (Mathf.Atan2(targetDir.X,targetdir.Z) - Mathf.Atan2(dir.x,dir.Z));
        return angle;
    }

    public float GetAngleToTarget(Transform3D transform, Vector3 targetPosition){
        return GetAngleToTarget(transform, targetPosition, Forward);
    }

    public float GetAngleToTarget(Transform3D transform, Vector3 targetPosition, Vector3 front){
        Vector3 dir = transform.Basis * front;
        var targetDir = GetTargetDir(targetPosition, transform.Origin);
        float angle = Mathf.Atan2(targetDir.X,targetDir.Z) - Mathf.Atan2(dir.X,dir.Z);
        return angle;
    }

    //
    //  Summary:
    //      Returns angular velocity required to partialy rotate on y axis towards position.
    //      The default forward facing direction is on positive z axis Vector3(0,0,1)
    public Vector3 GetAngularVelocity(Transform3D currentTransform, Vector3 targetPosition)
	{
        int rotationFix = 1;

        float angle = GetAngleToTarget(currentTransform, targetPosition);

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        if((targetPosition-currentTransform.Origin).Length() < 8){
            rotationFix *= 4;
        }

        return Up*(angle) * RotationSpeed * rotationFix;
	}

    public Vector3 GetAngularVelocity(Transform3D currentTransform, Vector3 targetPosition, Vector3 front)
	{
        int rotationFix = 1;

        float angle = GetAngleToTarget(currentTransform, targetPosition, front);

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        if((targetPosition-currentTransform.Origin).Length() < 8){
            rotationFix *= 4;
        }

        return Up*(angle) * RotationSpeed * rotationFix;
	}

    // public Vector3 GetAngularVelocity(Transform3D currentTransform, Vector3 targetPosition, Vector3 upDir)
	// {
    //     float angle = GetAngleToTargetOnXAxis(currentTransform, targetPosition);

    //     if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
    //     else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

    //     return upDir*(angle) * RotationSpeed;
	// }

    public void ResetSpeed(){
        Speed = 0;
    }

    public void UpdateStat(IStat stat)
    {
        switch (stat.Name)
        {
            case GlobalStatNames.RotationSpeed:
                RotationSpeed = stat.CurrentValue;
                break;
            case GlobalStatNames.Mass:
                Mass = stat.CurrentValue;
                break;
            case GlobalStatNames.RotationTolerance:
                RotationTolerance = stat.CurrentValue;
                break;
        }
    }
}