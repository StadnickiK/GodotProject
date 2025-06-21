using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public partial class StatManager : Node
{

    public Dictionary<int, BaseStat> Stats { get; set; } = new Dictionary<int, BaseStat>();

    public void AddStat(BaseStat stat)
    {
        Stats.Add(stat.Index, stat);
    }

    public void RemoveStat(int Index)
    {
        Stats.Remove(Index);
    }

    void CopyStats(Node stats)
    {
        foreach (Node node in stats.GetChildren())
        {
            if (node is BaseStat stat)
            {
                var statCopy = new BaseStat(stat);
                AddChild(statCopy);
            }
        }
    }

    public void AddStatModifier(int statID, StatModifier modifier)
    {
        Stats[statID].AddModifier(modifier);
    }

    public void RemoveStatModifier(int statID, StatModifier modifier)
    {
        Stats[statID].RemoveModifier(modifier);
    }

    public BaseStat GetStat(int index)
    {
        return Stats[index];
    }

    public float GetCurrentValue(int index)
    {
        return GetStat(index).CurrentValue;
    }

    public override void _Ready()
    {
        int i = 0;
        foreach (var item in GetChildren())
        {
            if (item is BaseStat stat)
            {
                stat.Index = i;
                Stats.Add(i, stat);
                i++;
            }
        }
    }

    public string PrintStats()
    {
        var str = new StringBuilder();
        foreach (var stat in Stats.Values)
            str.Append(stat.Name+" "+stat.CurrentValue);
        return str.ToString();
    }
}