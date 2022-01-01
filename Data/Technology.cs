using Godot;
using System;


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

    /// <summary>
    /// Godot is unable to Export tuple so string[] is used as replacement meaning that each element should be a pair of strings Type of requirement and its name ex. Building, Building 1
    /// </summary>
    /// <typeparam name="string[]"></typeparam>
    /// <returns>Array<string[]></returns>
    [Export]
    public Godot.Collections.Dictionary<string, string[]> Requirements { get; set; } = new Godot.Collections.Dictionary<string, string[]>();

}
