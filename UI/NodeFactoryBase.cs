using Godot;
using System;

public partial class NodeFactoryBase<T> : Node where T : Node
{
    [Export]
    public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";

    protected PackedScene scene;

    public override void _Ready()
    {
        scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
    }

    public T GetInstance()
    {
        return scene.Instantiate<T>();
    }

    public T GetInstance(Node parent)
    {
        T instance = GetInstance();
        parent.AddChild(instance);
        return instance;
    }

}
