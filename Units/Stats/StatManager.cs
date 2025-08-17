using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class StatManager : Node
{

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportStats { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

    public event BaseStat.StatChangedEventHandler StatChanged;

    public void AddStat(BaseStat stat)
    {
        Stats.Add(stat.Name, stat);
        stat.StatChanged += StatChangedInvoke;
        AddChild(stat);
    }

    void StatChangedInvoke(float value, string statName = null)
    {
        StatChanged?.Invoke(value, statName);
    }

    void CopyStats(Node stats)
    {
        foreach (Node node in stats.GetChildren())
        {
            if (node is BaseStat stat)
            {
                var statCopy = new BaseStat(stat);
                stat.StatChanged += StatChangedInvoke;
                Stats.Add(statCopy.Name, statCopy);
                AddChild(statCopy);
            }
        }
    }

    public void CloneStats(IStatManager statManager)
    {
        while (Stats.Count < statManager.StatManager.Stats.Count)
            AddStat(new BaseStat());

        for (int i = 0; i < Stats.Count; i++)
        {
            Stats.ElementAt(i).Value.CloneStat(statManager.StatManager.Stats.ElementAt(i).Value);
        }
    }



    public void AddStatModifier(string statName, StatModifier modifier)
    {
        Stats[statName].AddModifier(modifier);
    }

    public void RemoveStatModifier(string statName, StatModifier modifier)
    {
        Stats[statName].RemoveModifier(modifier);
    }

    public BaseStat GetStat(string statName)
    {
        return Stats[statName];
    }

    public float GetCurrentValue(string statName)
    {
        return GetStat(statName).CurrentValue;
    }

    public override void _Ready()
    {
        LoadStats();
        ConnectStatListeners(GetParent());
        UpdateListeners();
    }

    public void UpdateListeners()
    {
        foreach (var item in Stats.Values)
        {
            StatChangedInvoke(item.CurrentValue, item.Name);
        }
    }

    void LoadStats() {
        int i = 0;
        foreach (var item in GetChildren())
        {
            if (item is BaseStat stat)
            {
                stat.Index = i;
                stat.StatChanged += StatChangedInvoke;
                Stats.Add(stat.Name, stat);
                i++;
            }
        }
    }

    public void ConnectStatListeners(Node parent)
    {
        foreach (var child in parent.GetChildren())
            if (child is IUpdateStat updateStat)
            {
                StatChanged += updateStat.UpdateStat;
            }
    }

    public void DisonnectStatListeners(Node parent)
    {
        foreach (var child in parent.GetChildren())
            if (child is IUpdateStat updateStat)
            {
                StatChanged -= updateStat.UpdateStat;
            }
    }

    public string PrintStats()
    {
        var str = new StringBuilder();
        foreach (var stat in Stats.Values)
            str.Append(stat.Name + " " + stat.CurrentValue + "\n");
        return str.ToString();
    }
}