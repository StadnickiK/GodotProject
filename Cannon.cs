using Godot;
using System;

public partial class Cannon : RigidBody3D
{
	private void LookFollow(PhysicsDirectBodyState3D state, Transform3D currentTransform, Vector3 targetPosition)
	{
		var upDir = new Vector3(0, 1, 0);
		var curDir = currentTransform.Basis*(new Vector3(0, 0, 1));
		var targetDir = (targetPosition - currentTransform.Origin).Normalized();
		var rotationAngle = Mathf.Acos(curDir.X) - Mathf.Acos(targetDir.X);

		state.AngularVelocity = upDir * (rotationAngle / state.Step);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		var targetPosition = GetNode<RigidBody3D>("/root/World/Target").GlobalTransform.Origin;
		//GD.Print(state.AngularVelocity);
		LookFollow(state, GlobalTransform, targetPosition);
	}
}
