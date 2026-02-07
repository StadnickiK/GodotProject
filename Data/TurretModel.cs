using Godot;
using System;
using System.Collections.Generic;

public partial class TurretModel : Node, IStatManager
{
    public StatManager StatManager { get; set; }

    public List<string> Projectiles { get; set; } = new List<string>();

    public override void _Ready()
    {
        base._Ready();
        StatManager = GetNode<StatManager>("StatManager");
    }


}
