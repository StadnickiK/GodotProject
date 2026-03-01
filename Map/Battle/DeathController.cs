using Godot;
using System;
using System.Collections.Generic;

public partial class DeathController : Node3D, IUpdateStat
{
    public HashSet<string> StatNames { get; set; }

    public delegate void OnDeathEventHandler(object sender);

    public event OnDeathEventHandler OnDeath;

    ISavingNode savingNode;

    public void UpdateStat(IStat stat)
    {
        switch (stat.Name)
        {

            case GlobalStatNames.Health:
                if (stat.CurrentValue <= 0)
                    savingNode.InvokeSaveNode();
                break;
        }
    }

    public override void _Ready()
    {
        if (GetParent() is ISavingNode node)
            savingNode = node;
    }


}
