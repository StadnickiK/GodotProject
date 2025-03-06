using Godot;
using System;
using System.Collections.Generic;

public partial class UpkeepComponent : Node
{

    public Dictionary<int, int> ProdCost { get; set; } = new Dictionary<int, int>();

    public bool ProdCostChanged { get; set; } = false;

    public Dictionary<int, int> Production { get; set; } = new Dictionary<int, int>();

    public bool ProductionChanged { get; set; } = false;



}
