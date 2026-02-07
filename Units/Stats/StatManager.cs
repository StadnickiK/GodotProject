using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class StatManager : Node, IStatChangedNotifier
{

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportStats { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public Dictionary<string, IStat> Stats { get; set; } = new Dictionary<string, IStat>();

    public event IStat.StatChangedEventHandler StatChanged;

    public void AddStat(BaseStat stat)
    {
        AddChild(stat);
        Stats.Add(stat.Name, stat);
        stat.StatChanged += StatChangedInvoke;
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
        CloneStats(statManager.StatManager);
    }

    public void CloneStats(StatManager statManager)
    {
        for (int i = Stats.Count; i < statManager.Stats.Count; i++)
        {
            var stat = new BaseStat();
            stat.CloneStat(statManager.Stats.ElementAt(i).Value);
            AddStat(stat);
        }
        for (var i = 0; i < Stats.Count; i++)
        {
            Stats.ElementAt(i).Value.CloneStat(statManager.Stats.ElementAt(i).Value);
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

    public float GetStatCurrentValue(string statName)
    {
        return GetStat(statName).CurrentValue;
    }

    public bool HasStat(string statName)
    {
        return Stats.ContainsKey(statName);
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
        foreach (var child in parent.GetChildren())
        {
            if (child is IUpdateStat updateStat)
            {
                StatChanged -= updateStat.UpdateStat;
                StatChanged += updateStat.UpdateStat;
            }
            
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
}