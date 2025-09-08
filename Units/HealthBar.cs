using Godot;
using System;
using System.Collections.Generic;

public partial class HealthBar : Node3D, IUpdateStat
{
    public ProgressBar Health { get; set; }

    public ProgressBar Shield { get; set; }
    public HashSet<string> StatNames { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void UpdateStat(IStat stat)
    {
        switch (stat.Name)
        {
            case "Health":
                UpdateHealth(stat);
                break;
            case "Shield":
                UpdateShield(stat);
                break;
            // default:
        }
    }

    public void UpdateHealth(IStat stat)
    {
        Health.MinValue = stat.MinValue;
        Health.MaxValue = stat.MaxValue;
        Health.Value = stat.CurrentValue;
        //Health._Draw();
    }

    public void UpdateShield(IStat stat)
    {
        Shield.Visible = true;
        Shield.MinValue = stat.MinValue;
        Shield.MaxValue = stat.MaxValue;
        Shield.Value = stat.CurrentValue;
        //Shield._Draw();
    }

    public void UpdatePosition(Vector3 meshSize)
    {
        Position = new Vector3(0, meshSize.Y, 0);
    }

    public override void _Ready()
    {
        Health = GetNode<ProgressBar>("SubViewport/VBoxContainer/Health");
        Shield = GetNode<ProgressBar>("SubViewport/VBoxContainer/Shield");
    }

}
