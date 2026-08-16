using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ObstacleDetectionComponent : Area3D//, IVisionComponentListener
{

	public CollisionShape3D CollisionShape3D { get; set; }

	HashSet<ICollider3D> Colliders = new HashSet<ICollider3D>();

	public ICollider3D Parent { get; set; }

	ICollider3D ClosestCollider;

	public Aabb ClosestIntersection;

	public Vector3 AvoidanceVelocity { get; private set; } = Vector3.Zero;

	[Export]
	float CollisionRange { get; set; } = 20;

	float CollisionRadius { get; set; } = 10;

	[Export]
	float MaxAvoidDistance { get; set; } = 5;

	public float MaxSpeed { get; set; } = 10;

	public float RotationSpeed { get; set; } = 20;

	public delegate void ClosestColliderChangedEventHandler(ICollider3D closest, Aabb closestCollider);

	public event ClosestColliderChangedEventHandler ClosestColliderChanged;

	public delegate void DesiredVelocityChangedEventHandler(Vector3 velocity);

	public event DesiredVelocityChangedEventHandler DesiredVelocityChanged;

	public override void _Ready()
	{
		CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
		UpdateVisionRange();
	}

	public void Initialize(ICollider3D parent, Vector3 collisionSize, float maxSpeed, float rotationSpeed)
	{
		Parent = parent;
		MaxSpeed = maxSpeed;
		RotationSpeed = rotationSpeed;
		UpdateSize(collisionSize.X > collisionSize.Y ? collisionSize.X : collisionSize.Y, collisionSize.Z + CollisionRange);
	}

	public void UpdateVisionRange()
    {
        UpdateVisionRange(CollisionRange);
    }

    public void UpdateVisionRange(float range)
    {
        CollisionRange = range;
        UpdateSize(CollisionRadius, CollisionRange);
    }

	public void UpdateSize(float radius, float range)
	{
		CollisionRadius = radius;
		CollisionRange = range;
        var s = new CylinderShape3D();
        s.Height = CollisionRange;
		s.Radius = CollisionRadius;
        CollisionShape3D.Shape = s;
		CollisionShape3D.Position = new Vector3(0,0, CollisionRadius - CollisionRange);
	}

	void _on_body_entered(Node3D node3D)
	{
		if(node3D is ICollider3D collisionObject3D && node3D != Parent && Parent != null)
		{
			// HandleSingleCollider(collisionObject3D);
			HandleCollider(collisionObject3D);
		}	
	}

	void HandleSingleCollider(ICollider3D collisionObject3D)
	{
		if(ClosestCollider == null)
		{
			ClosestIntersection = Intersection(collisionObject3D);
			ClosestCollider = collisionObject3D;
		}
		else
		{
			CompareDistances(collisionObject3D, ClosestCollider);
		}
		ClosestColliderChanged?.Invoke(ClosestCollider, ClosestIntersection);
		Colliders.Add(collisionObject3D);
	}

	void HandleCollider(ICollider3D collisionObject3D)
	{
		ClosestIntersection = Intersection(collisionObject3D);
		ClosestCollider = collisionObject3D;
		var offset = ClosestIntersection.Position - ClosestCollider.Position + new Vector3(Parent.GetUnitSize().X,0,0);
		var targetPos = ClosestIntersection.Position + offset;
		var dir = (targetPos - Position);
		AvoidanceVelocity = (AvoidanceVelocity + dir);
		DesiredVelocityChanged?.Invoke(AvoidanceVelocity);
		Colliders.Add(collisionObject3D);
	}

	void RemoveCollider(ICollider3D collisionObject3D)
	{
		if(Colliders.Count > 1)
		{
			ClosestIntersection = Intersection(collisionObject3D);
			ClosestCollider = collisionObject3D;
			var offset = ClosestIntersection.Position - ClosestCollider.Position + new Vector3(Parent.GetUnitSize().X,0,0);
			var targetPos = ClosestIntersection.Position + offset;
			var dir = (targetPos - Position);
			AvoidanceVelocity = (AvoidanceVelocity - dir);	
		}else
			AvoidanceVelocity = Vector3.Zero;
		DesiredVelocityChanged?.Invoke(AvoidanceVelocity);
		Colliders.Remove(collisionObject3D);
	}

	void _on_body_exited(Node3D node3D)
	{
		if(node3D is ICollider3D collisionObject3D && Parent != null)
		{
			RemoveCollider(collisionObject3D);
			// Colliders.Remove(collisionObject3D);
			// if(ClosestCollider == collisionObject3D){
			// 	FindClosestCollider();
			// 	ClosestColliderChanged?.Invoke(ClosestCollider, ClosestIntersection);
			// }
		}	
	}

	Aabb GetAabb()
	{
		return new Aabb(Position, new Vector3(CollisionRadius * 2, CollisionRadius * 2, CollisionRange));
	}

	Aabb Intersection(ICollider3D collider3D)
	{
		var bodyRect = GetAabb();
		return bodyRect.Intersection(collider3D.GetAabb());
	}

	void FindClosestCollider()
	{
		if(Colliders.Count > 0)
		{
			ICollider3D closest = Colliders.FirstOrDefault();
			foreach (var item in Colliders)
			{
				CompareDistances(closest, item);
			}
		}
		else
		{
			ClosestCollider = null;
		}
	}

	void CompareDistances(ICollider3D colA, ICollider3D colB)
	{
		var body = GetAabb();
		var aInt = Intersection(colA);
		var bInt = Intersection(colB);
		var a = (aInt.Position - Parent.Position).Length();
		var b = (bInt.Position - Parent.Position).Length();
		if(a < b)
		{
			ClosestIntersection = aInt;
			ClosestCollider = colA;
		}
		else
		{
			ClosestIntersection = bInt;
			ClosestCollider = colB;
		}
			
	}

}
