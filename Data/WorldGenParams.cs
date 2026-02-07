using Godot;
using System;
using System.Collections.Generic;

public partial class WorldGenParams
{
    public int RandomSeed { get; set; }
    public Godot.Collections.Dictionary<string, int> WorldGenParameters;

    public Godot.Collections.Dictionary<int, ValueSlider> ResourceSliders;

    public HashSet<Combatant> Combatants { get; set; }

    public BattlefieldSettings BattlefieldSettings { get; set; }

    // public List<IEnterCombatBase> Attackers { get; set; }

    // public List<IEnterCombatBase> Defenders { get; set; }
}
