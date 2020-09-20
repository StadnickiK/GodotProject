using Godot;
using System;

public class VelocityController : Node
{
    public float Mass { get; set; } = 1;
    public float MaxSpeed { get; set; } = 10;
    public float Speed { get; set; } = 0;

    public float RotationSpeed { get; set; } = 1;

    public float Acceleration { get; set; } = 1;

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
        Velocity.y = 0;
        return Velocity;
    }

    public Vector3 GetAcceleratedVelocity(
            Vector3 currentPosition,
            Vector3 targetPos)
            {
        if(Speed <= MaxSpeed){
            Speed += Acceleration;
        }
        Vector3 Velocity = (targetPos - currentPosition).Normalized() * Speed;
        Velocity.y = 0;
        return Velocity;
    }

    public Vector3 GetTargetDir(Vector3 currentPosition, Vector3 targetPosition){
        return (targetPosition - currentPosition).Normalized();
    }

    static public Vector3 GetTransformedPos(Transform transform, Vector3 vector){
        return transform.basis.Xform(vector);
    }

    public float GetAngleToTarget(Transform transform, Vector3 targetPosition){
        Vector3 dir = transform.basis.Xform(new Vector3(0, 0, -1));
        var targetDir = GetTargetDir(targetPosition, transform.origin);
        float angle = (Mathf.Atan2(targetDir.x,targetDir.z) - Mathf.Atan2(dir.x,dir.z));
        return angle;
    }

    public float GetAngleToTarget(Transform transform, Vector3 targetPosition, Vector3 front, Vector3 atan2Yaxis){
        Vector3 dir = transform.basis.Xform(front);
        var targetDir = GetTargetDir(targetPosition, transform.origin);
        float angle = (Mathf.Atan2((targetDir*atan2Yaxis).Length(),targetDir.z) - Mathf.Atan2((dir*atan2Yaxis).Length(),dir.z));
        return angle;
    }

    public float GetAngleToTargetOnXAxis(Transform transform, Vector3 targetPosition){
        Vector3 dir = transform.basis.Xform(new Vector3(0, 0, -1));
        var targetDir = GetTargetDir(targetPosition, transform.origin);
        float angle = (Mathf.Atan2(targetDir.y,targetDir.z) - Mathf.Atan2(dir.y,dir.z));
        
        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }
        //float angle = (Mathf.Atan2(targetDir.x,targetDir.z) - Mathf.Atan2(dir.x,dir.z));
        return angle;
    }

    //
    //  Summary:
    //      Returns angular velocity required to partialy rotate on y axis towards position.
    //      The default forward facing direction is on positive z axis Vector3(0,0,1)
    public Vector3 GetAngularVelocity(Transform currentTransform, Vector3 targetPosition)
	{
        Vector3 upDir = new Vector3(0, 1, 0);
        float angle = GetAngleToTarget(currentTransform, targetPosition);

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        return upDir*(angle) * RotationSpeed;
	}

    public Vector3 GetAngularVelocity(Transform currentTransform, Vector3 targetPosition, Vector3 upDir)
	{
        float angle = GetAngleToTargetOnXAxis(currentTransform, targetPosition);

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        return upDir*(angle) * RotationSpeed;
	}

    public void ResetSpeed(){
        Speed = 0;
    }

}