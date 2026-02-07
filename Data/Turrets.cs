using Godot;
using System;
using System.Collections.Generic;

public partial class Turrets : Node
{
    public Dictionary<string, TurretModel> TurretModels { get; set; } = new Dictionary<string, TurretModel>();

    public override void _Ready()
    {
        base._Ready();
        foreach (var item in GetChildren())
        {
            if(item is TurretModel turretModel)
                TurretModels.Add(item.Name, turretModel);
        }
    }

}
