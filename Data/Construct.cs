using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Construct : Node, IConstruct
{


    [Export]
    public Godot.Collections.Dictionary<string, int> ExportBuildCost { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public System.Collections.Generic.Dictionary<int, int> BuildCost { get; set; } = new System.Collections.Generic.Dictionary<int, int>();
    
    [Export]
    public Godot.Collections.Dictionary<string, Array<string>> ExportRequirements { get; set; } = new Godot.Collections.Dictionary<string, Array<string>>();

    public System.Collections.Generic.Dictionary<int, List<int>> Requirements { get; set; } = new System.Collections.Generic.Dictionary<int, List<int>>();

    [Export]
    public string ConstructName { get; set; }

    [Export]
    public int BuildTime { get ; set ; }
    
    public int CurrentTime { get ; set ; }

    public int Index { get; set; }
}
