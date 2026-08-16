using Godot;
using System;

public partial class Explosion : GpuParticles3D, ISavingNode
{

    [Export]
    public float SizeMultiplier { get; set; } = 1;

    public event Node3DPool.SaveNodeEventHandler SaveNode;

    ParticleProcessMaterial particleProcessMaterial;
    SphereMesh sphereMesh;

    public override void _Ready()
    {
        base._Ready();
        particleProcessMaterial = (ParticleProcessMaterial)ProcessMaterial;
        sphereMesh = (SphereMesh)DrawPass1;
        // OneShot = true;
        // Finished += _on_finished;
        // Restart();

        // Emitting = true;

    }

    public void UpdateSize(float sizeMultiplier)
    {
        particleProcessMaterial.EmissionSphereRadius *= sizeMultiplier;
        ProcessMaterial = particleProcessMaterial;
        sphereMesh.Radius *= sizeMultiplier;
        sphereMesh.Height *= sizeMultiplier;
        DrawPass1 = sphereMesh;
    }

    public void BeforeSave()
    {
        
    }

    public void InvokeSaveNode()
    {
        SaveNode?.Invoke(this);
    }


    public void _on_finished()
    {
        QueueFree();
    }
}
