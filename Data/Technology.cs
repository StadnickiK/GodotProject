using Godot;
using System;

public class Technology : Node, ITechReq, IBuilding
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
    public Godot.Collections.Array<string> TechReq { get; set; } = new Godot.Collections.Array<string>();

}
