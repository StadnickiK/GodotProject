using Godot;
using System;

public class RCube : RigidBody
{

[Export]
public float TurretSpeed = 1;

    private void LookFollow(PhysicsDirectBodyState state, Transform currentTransform, Vector3 targetPosition)
    {
        Vector3 upDir = new Vector3(0, 1, 0);
        targetPosition.y = GlobalTransform.origin.y;
        //Vector3 curDir = new Vector3(0,0,1).Rotated(upDir,GlobalTransform.basis.GetEuler().y);
        var curDir = currentTransform.basis.Xform(new Vector3(0, 0, 1));
        //float rotationAngle = (float)(Math.Atan2(targetDir.z, targetDir.x)-Math.Atan2(curDir.z, curDir.x));
        var targetDir = (targetPosition - currentTransform.origin).Normalized();
        float angle = Mathf.Atan2(targetDir.x,targetDir.z) - Mathf.Atan2(curDir.x,curDir.z); //Mathf.Atan2(curDir.x,curDir.z) - ;
        //state.AngularVelocity = curDir.LinearInterpolate(targetDir, TurretSpeed*state.Step);    
        //GD.Print(angle);

        if (angle > Math.PI)        { angle -= 2 * (float)Math.PI; }
        else if (angle <= -Math.PI) { angle += 2 * (float)Math.PI; }

        state.AngularVelocity = upDir*(angle) * 10;//(upDir * (rotationAngle / state.Step));
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        RigidBody r = (RigidBody)GetParent().GetNode("Target");
        var targetPosition = r.GlobalTransform.origin;//GetNode<RigidBody>("Target").GlobalTransform.origin;
        //var targetPosition = new Vector3(-5,1,-15);
        //GD.Print(Transform.origin.AngleTo(targetPosition));
        //state.AngularVelocity = new Vector3(0,2,0);
        //state.AngularVelocity = new Vector3(0,0,0);
        LookFollow(state, GlobalTransform, targetPosition);
    }
}
