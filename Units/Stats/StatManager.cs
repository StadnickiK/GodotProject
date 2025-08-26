using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class StatManager : Node, IUpdateStat, IStatChangedNotifier
{

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportStats { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public Dictionary<string, IStat> Stats { get; set; } = new Dictionary<string, IStat>();
    public HashSet<string> StatNames { get; set; }

    public event IStat.StatChangedEventHandler StatChanged;

    public void AddStat(BaseStat stat)
    {
        Stats.Add(stat.Name, stat);
        stat.StatChanged += StatChangedInvoke;
        AddChild(stat);
    }

    void StatChangedInvoke(IStat stat)
    {
        StatChanged?.Invoke(stat);
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

    public IStat GetStat(string statName)
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
        // ConnectStatListeners(GetParent());
        // UpdateListeners();
    }

    public void UpdateListeners()
    {
        foreach (var item in Stats.Values)
        {
            StatChangedInvoke(item);
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
        foreach (var child in parent.GetChildren()) {
            if (child is IUpdateStat updateStat)
                StatChanged += updateStat.UpdateStat;
            //if (child is IStatChangedNotifier statChangedNotifier)
                //statChangedNotifier.StatChanged += UpdateStat; 
            }
    }

    public void DisonnectStatListeners(Node parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is IUpdateStat updateStat)
                StatChanged -= updateStat.UpdateStat;
            // if (child is IStatChangedNotifier statChangedNotifier)
            //     statChangedNotifier.StatChanged -= UpdateStat;  
        }
    }

    public string PrintStats()
    {
        var str = new StringBuilder();
        foreach (var stat in Stats.Values)
            str.Append(stat.Name + " " + stat.CurrentValue + "\n");
        return str.ToString();
    }

    public void UpdateStat(IStat stat)
    {
        if (Stats.ContainsKey(stat.Name))
        {
            Stats[stat.Name].CurrentValue = stat.CurrentValue;
        }else
            GD.Print(GetParent().Name + " does not contain Stat " + stat.Name);
        
    }
}