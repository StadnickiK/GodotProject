using Godot;
using System;




public partial class Laser : RayCast3D, IProjectile, ISavingNode
{
    [Export]
    private Color _color = new Color("ff0000b4");
    [Export]
    private float circleRadius = 0.03f;
    [Export]
    float height = 0.01f;

    [Export]
    float range = 100f;

    [Export]
    public double Duration { get; set; } = 0.3;

    [Export]
    public int MaxDmgInstances { get; set; } = 1;

    int DmgInstances = 0;

    public IDamagable Source { get; set; }

    public delegate void DamageEventHandler(IDamagable source, IDamagable to);

    public event IProjectile.DamageEventHandler Damage;

    double time = 0;

    public Color CurrentColor
    {
        get { return _color; }
        set { _color = value; UpdateColor(value); }
    }

    [Export]
    public string ColorParameterName { get; set; } = "Color";

    public float Height { get => height; set { height = value; UpdateHeight(value); } }

    public float CircleRadius { get => circleRadius; set { circleRadius = value; UpdateRadius(value); } }

    public float Range { get => range; set { range = value; UpdateRange(value); } }

    ShaderMaterial shaderMaterial;

    CylinderMesh cylinderMesh;

    GpuParticles3D gpuParticles3D;

    MeshInstance3D meshInstance3D;

    public event Node3DPool.SaveNodeEventHandler SaveNode;

    public override void _Ready()
    {
        gpuParticles3D = GetNodeOrNull<GpuParticles3D>("EndLaserParticles");
        meshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");

        cylinderMesh = (CylinderMesh)meshInstance3D.Mesh;
        shaderMaterial = cylinderMesh.Material as ShaderMaterial;
        UpdateRadius(circleRadius);
        UpdateRange(Range);
    }

    void UpdateColor(Color value)
    {
        shaderMaterial.SetShaderParameter(ColorParameterName, value);
    }

    void UpdateHeight(float height)
    {
        cylinderMesh.Height = MathF.Abs(height);
    }

    void UpdateRange(float range)
    {
        TargetPosition = new Vector3(0, 0, -range);
    }

    void UpdateRadius(float radius)
    {
        cylinderMesh.TopRadius = radius;
        cylinderMesh.BottomRadius = radius;
    }

    public void Shoot(Vector3 from, Vector3 to)
    {
        time = 0;
        DmgInstances = 0;
        GlobalPosition = from;
        LookAt(to);
        //TargetPosition = ToLocal(to);
        //meshInstance3D.LookAt(to, new Vector3(1,0,0));
        Show();
    }

    public void Shoot(Vector3 to)
    {
        time = 0;
        DmgInstances = 0;
        TargetPosition = ToLocal(to);
        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (time < Duration)
        {
            ForceRaycastUpdate();
            if (IsColliding())
            {
                UpdateProjectile();
            }
            else
            {
                meshInstance3D.Position = new Vector3(0, 0, -Range / 2);
                UpdateHeight(Range);
            }
            time += delta;
        }
        else
        {
            if (IsColliding())
                if (GetCollider() is IDamagable damagable && DmgInstances < MaxDmgInstances)
                {
                    Damage?.Invoke(Source, damagable);
                    DmgInstances++;
                }
            
            InvokeSaveNode();
        }
    }

    void UpdateProjectile()
    {
        var castPoint = ToLocal(GetCollisionPoint());
        UpdateHeight(castPoint.Z);
        meshInstance3D.Position = castPoint / 2;
        gpuParticles3D.Position = new Vector3(gpuParticles3D.Position.X, gpuParticles3D.Position.Y, castPoint.Z);
    }

    public void InvokeSaveNode()
    {
        SaveNode?.Invoke(this);
    }

    public void BeforeSave()
    {}

}
