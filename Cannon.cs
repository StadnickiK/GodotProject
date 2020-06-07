using Godot;
using System;

public class Cannon : RigidBody
{
	private void LookFollow(PhysicsDirectBodyState state, Transform currentTransform, Vector3 targetPosition)
	{
		var upDir = new Vector3(0, 1, 0);
		var curDir = currentTransform.basis.Xform(new Vector3(0, 0, 1));
		var targetDir = (targetPosition - currentTransform.origin).Normalized();
		var rotationAngle = Mathf.Acos(curDir.x) - Mathf.Acos(targetDir.x);

		state.AngularVelocity = upDir * (rotationAngle / state.Step);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		var targetPosition = GetNode<RigidBody>("/root/World/Target").GlobalTransform.origin;
		//GD.Print(state.AngularVelocity);
		LookFollow(state, GlobalTransform, targetPosition);
	}
}
