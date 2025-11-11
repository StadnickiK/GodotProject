using Godot;
using System;

public partial class FireEmitter : GpuParticles3D
{
    MeshInstance3D material;

    [Export]
    public string ItemScenePath { get; set; } = "res://Effects/fire_emitter_material_mesh.tscn";

    PackedScene packedScene;

    public override void _Ready()
    {
        packedScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        material = (MeshInstance3D)packedScene.Instantiate();
        UpdateMaterial();
    }

    public void UpdateMesh(MeshInstance3D meshInstance)
    {
        DrawPass1 = meshInstance.Mesh;
        Transform = meshInstance.Transform;
        UpdateMaterial();
    }
    
    void UpdateMaterial()
    {
        if(material != null)
        {
            var surfaceMaterial = material.GetSurfaceOverrideMaterial(0);
            DrawPass1.SurfaceSetMaterial(0, surfaceMaterial);
        }  
    }
}
