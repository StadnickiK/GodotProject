using Godot;
using System;
using System.Collections.Generic;


public class Technology : Node, IBuilding
{

    public override void _Ready()
    {
          
    }

    [Export]
    public int BuildTime { get; set; } = 0;
    public int CurrentTime { get; set; } = 0;
    
    [Export]
    public Godot.Collections.Dictionary<string, int> BuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    [Export]
    public Dictionary<string, List<string>> Requirements { get; set; } = new Dictionary<string, List<string>>();

}
