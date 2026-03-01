using Godot;
using System;
using System.Collections.Generic;

public partial class TurretModel : Node, IStatManager
{
    public StatManager StatManager { get; set; }

    [Export]
    public Godot.Collections.Array<string> Projectiles { get; set; } = new Godot.Collections.Array<string>();

    public override void _Ready()
    {
        base._Ready();
        StatManager = GetNode<StatManager>("StatManager");
    }


}
