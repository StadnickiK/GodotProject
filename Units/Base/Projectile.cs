using Godot;

public partial class Projectile : Area3D, IProjectile, ISavingNode
{
    public IDamagable Source { get; set; }

    public event IProjectile.DamageEventHandler Damage;
    public event Node3DPool.SaveNodeEventHandler SaveNode;

    public CollisionObject3D CollisionObject3D { get; set; }

    public MeshInstance3D MeshInstance3D { get; set; }

    [Export]
    public int TopVelocity { get; set; } = 10;

    [Export]
    public double Lifetime { get; set; } = 60;

    double time = 0;

    public Node GetAsNode { get { return this; }}

    public Vector3 LinearVelocity { get; private set; }

    public override void _Ready()
    {
        MeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
    }


    public void Shoot(Vector3 from, Vector3 to, Vector3 direction)
    {
        LinearVelocity = (to - from).Normalized() * TopVelocity;
        LookAt(to);
        Show();
    }

    public void UpdateSource(IDamagable damagable)
    {
        Source = damagable;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition += LinearVelocity;
        //MoveAndCollide(LinearVelocity);
        if (time > Lifetime){
            InvokeSaveNode();
        }
        time += delta;
    }

    public void Shoot(Vector3 from, ITargetable to, Vector3 direction)
    {
        LinearVelocity = (to.GlobalPosition - from).Normalized() * TopVelocity;
        LookAt(to.GlobalPosition);
        Show();
    }

    public void InvokeSaveNode()
    {
        time = 0;
        SaveNode?.Invoke(this);
    }

    public void BeforeSave()
    {}

    public void _on_body_entered(Node node)
    {
        if(node != Source && node != CollisionObject3D)
        {
            if (node is IDamagable damagable)
            {
                Damage?.Invoke(Source, damagable);
            }
            Hide();
            time = Lifetime + 1;
            //InvokeSaveNode();
        }
    }

}
