using Godot;
using System;
using System.Collections.Generic;


public partial class Technology : Node, IBuilding
{

	public override void _Ready()
	{
		  
	}

	public int Index { get; set; }

	[Export]
	public int BuildTime { get; set; } = 0;
	public int CurrentTime { get; set; } = 0;
	
    [Export]
     Godot.Collections.Dictionary<int, int> ExportBuildCost { get; set; } = new Godot.Collections.Dictionary<int, int>();

    public System.Collections.Generic.Dictionary<int, int> BuildCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();

    [Export]
     Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> ExportRequirements { get; set; } = new Godot.Collections.Dictionary<int, Godot.Collections.Array<int>>();

     public System.Collections.Generic.Dictionary<int, List<int>> Requirements { get; set; } = new System.Collections.Generic.Dictionary<int, List<int>>();
    string IBuilding.Name { get; set; }

}
