using Godot;
using System;

public partial class FireEmitter : GpuParticles3D
{
    MeshInstance3D mesh;

    Material material;

    ParticleProcessMaterial particleProcessMaterial;

    [Export]
    public string ItemScenePath { get; set; } = "res://Effects/fire_emitter_material_mesh.tscn";

    PackedScene packedScene;

    public override void _Ready()
    {
        packedScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        mesh = (MeshInstance3D)packedScene.Instantiate();
        material = mesh.GetSurfaceOverrideMaterial(0);
        particleProcessMaterial = (ParticleProcessMaterial)ProcessMaterial;
        UpdateMaterial();
    }

    public void UpdateMesh(MeshInstance3D meshInstance, float lifetime = 0.6f)
    {
        //Lifetime = 0.2f;
        Lifetime = lifetime;
        //ProcessMaterial = particleProcessMaterial;
        DrawPass1 = meshInstance.Mesh;
        Transform = meshInstance.Transform;
        UpdateMaterial();
    }
    
    void UpdateMaterial()
    {
        if(mesh != null)
        {
            DrawPass1.SurfaceSetMaterial(0, material);
        }  
    }
}
