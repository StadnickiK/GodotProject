using Godot;
using System;
using System.Collections.Generic;

public partial class HealthBar3D : Node3D, IUpdateStat
{
    public HealthBar Health { get; set; }
    public HealthBar Shield { get; set; }
    public HashSet<string> StatNames { get; set; }

    public SubViewport SubViewport { get; set; }

    [Export]
    public Vector2I Size { get; set; } = new Vector2I(4096, 400);

    public void UpdateStat(IStat stat)
    {
        switch (stat.Name)
        {
            case GlobalStatNames.Health:
                UpdateHealth(stat);
                break;
            case GlobalStatNames.Shield:
                UpdateShield(stat);
                break;
            // default:
        }
    }

    public void UpdateHealth(IStat stat)
    {
        Health.Update(stat);
    }

    public void UpdateShield(IStat stat)
    {
        Shield.Update(stat, stat.MaxValue > 0);
    }

    public void UpdatePosition(Vector3 meshSize)
    {
        Position = new Vector3(0, meshSize.Y, 0);
    }

    public override void _Ready()
    {
        Health = GetNode<HealthBar>("SubViewport/VBoxContainer/Health");
        Shield = GetNode<HealthBar>("SubViewport/VBoxContainer/Shield");
        SubViewport = GetNode<SubViewport>("SubViewport");
        UpdateSize(Size);
    }

    public void UpdateSize(Vector2I size)
    {
        Size = size;
        SubViewport.Size = size;
        var barSize = new Vector2(size.X, size.Y/2);
        Health.Size = barSize;
        Shield.Size = barSize;
    }




}
