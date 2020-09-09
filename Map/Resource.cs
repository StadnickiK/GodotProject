using Godot;
using System;

public class Resource : Node
{
    String RName = "ResourceName";

    public int Amount { get; set; } = 0;

    public int AmountCap { get; set; } = -1;

    enum Type
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
