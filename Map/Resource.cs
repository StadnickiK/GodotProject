using Godot;
using System;

public class Resource : Node
{

    //public int Quantity { get; set; } = 0;

    public int Quantity = 5;

    public int? QuantityCap { get; set; } = null;

    public int Value { get; set; } = 3;

    public Random Rand { get; set; } = new Random();

    private int _baseValue = 1;
    public int BaseValue { get => _baseValue; }

    private Type _type = Type.Other;
    public Type ResourceType
    {
        get { return _type; }
    }

    public enum Type
    {
        Ore,
        Gas,
        Liquid,
        Other
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
