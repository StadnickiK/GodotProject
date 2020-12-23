using Godot;
using System;

public class Resource : Node
{

    public int Quantity { get; set; } = 0;

    public int QuantityCap { get; set; } = -1;

    public int Value { get; set; } = 1;

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

    void Generate(){
        Quantity = Rand.Next(1,100)*1000+Rand.Next(1,1000);
        QuantityCap = Quantity;
        _type = (Type)Rand.Next(0,1);
    }

    public Resource(){
        Generate();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
