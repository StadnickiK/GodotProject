using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleFireControl : Node
{

    List<GpuParticles3D> GpuParticles3Ds = new List<GpuParticles3D>();

    [Export]
    public string ItemScenePath { get; set; } = ScenePaths.Instance.FlameScene;

    PackedScene packedScene;

    public override void _Ready()
    {
        packedScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        Stop();
    }

    public void Start(){
        foreach(var node in GpuParticles3Ds)
            node.Emitting = true;
    }

    public void Stop()
    {
        foreach (var node in GpuParticles3Ds)
            node.Emitting = false;
    }

    void LoadEmitters(List<MeshInstance3D> meshes, float Lifetime = 0.6f)
    {
        foreach (var mesh in meshes)
        {
            var node = (FireEmitter)packedScene.Instantiate();
            node.UpdateMesh(mesh, Lifetime);
            AddChild(node);
            GpuParticles3Ds.Add(node);
        }
    }

    public void UpdateEmitters(List<MeshInstance3D> meshes, float Lifetime = 0.6f)
    {
        ClearEmitters();
        LoadEmitters(meshes, Lifetime);
    }

    public void ClearEmitters()
    {
        foreach (var item in GpuParticles3Ds)
            item.QueueFree();
        GpuParticles3Ds.Clear();
    }
    
}
