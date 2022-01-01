using Godot;
using System;

public class Resource : Node
{

    //public int Quantity { get; set; } = 0;
    public int Quantity = 0;

    public int? QuantityCap { get; set; } = null;

    public int Value { get; set; } = 0;

    private int _baseValue = 1;
    public int BaseValue { get => _baseValue; }
    [Export]
    public Type ResourceType { get; set; } = Type.Ore;
    [Export]
    public int Rarity { get; set; } = 0;
    [Export]
    public bool IsStarter { get; set; } = false;

    [Export]
    public Localisation Location { get; set; } = Localisation.Global;

    public Resource(){}

    public Resource(string name){
        Name = name;
    }

    public Resource(string name, int quantity){
        Name = name;
        Quantity = quantity;
    }

    public Resource(string name, int quantity, int value){
        Name = name;
        Quantity = quantity;
        Value = value;
    }

    public enum Type
    {
        Ore,
        Gas,
        Liquid,
        Product,
        Other
    }

    public enum Localisation
    {
        Global,
        Local
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
