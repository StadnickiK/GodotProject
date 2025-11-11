using Godot;

public partial class Projectile : RigidBody3D, IProjectile
{
    public IDamagable Source { get; set; }

    public event IProjectile.DamageEventHandler Damage;

    public CollisionShape3D CollisionShape3D { get; set; }

    public MeshInstance3D MeshInstance3D { get; set; }

    [Export]
    public int TopVelocity { get; set; } = 10;

    public override void _Ready()
    {
        CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        MeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
    }


    public void Shoot(Vector3 from, Vector3 to, Vector3 direction)
    {
        LinearVelocity = (to - from).Normalized() * TopVelocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        MoveAndCollide(LinearVelocity);
    }

}
