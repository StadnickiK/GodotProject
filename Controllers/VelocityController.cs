using Godot;
using System;

public class VelocityController
{
    public float Mass { get; set; } = 1;
    public float MaxSpeed { get; set; } = 10;
    public float Speed { get; set; } = 0;

    public float Acceleration { get; set; } = 1;

    public VelocityController(){}
    public VelocityController(float maxSpeed, float acceleration, float mass){
        Mass = mass;
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
    }

    public Vector3 GetAcceleratedVelocity(
            Vector3 currentVelocity, 
            Vector3 currentPosition,
            Vector3 targetPos)
            {
        if(Speed <= MaxSpeed){
            Speed += Acceleration;
        }
        //Vector3 direction = dir * MaxSpeed;
        //GD.Print("dir "+direction);
        Vector3 Velocity = (targetPos - currentPosition).Normalized() * Speed;
        //Vector3 v = (direction - currentVelocity) ;// ShipMass; 
        //v += currentVelocity;
        Velocity.y = 0;
        return Velocity;
    }

    //
    //  Summary:
    //      Returns angular velocity required to partialy rotate on y axis towards position.
    //      The default facing direction is on positive z axis Vector3(0,0,1)
    public Vector3 GetAngularVelocity(Transform currentTransform, Vector3 targetPosition)
	{
        Vector3 upDir = new Vector3(0, 1, 0);
        targetPosition.y = currentTransform.origin.y;
        Vector3 dir = currentTransform.basis.Xform(new Vector3(0, 0, 1));
        var targetDir = (targetPosition - currentTransform.origin).Normalized();
        float angle = (Mathf.Atan2(targetDir.x,targetDir.z) - Mathf.Atan2(dir.x,dir.z))/Mass;

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        return upDir*(angle) * 10;
	}

    public void ResetSpeed(){
        Speed = 0;
    }

}