using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleFireControl : Node
{

    List<GpuParticles3D> GpuParticles3Ds = new List<GpuParticles3D>();

    [Export]
    public string ItemScenePath { get; set; } = "res://Effects/FireEmitter.tscn";

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

    public void LoadEmitters(List<MeshInstance3D> meshes)
    {
        foreach (var mesh in meshes)
        {
            var node = (FireEmitter)packedScene.Instantiate();
            node.UpdateMesh(mesh);
            AddChild(node);
            GpuParticles3Ds.Add(node);
        }
    }

    public void UpdateEmitters(List<MeshInstance3D> meshes)
    {
        ClearEmitters();
        LoadEmitters(meshes);
    }

    public void ClearEmitters()
    {
        foreach (var item in GpuParticles3Ds)
            item.QueueFree();
        GpuParticles3Ds.Clear();
    }
    
}
