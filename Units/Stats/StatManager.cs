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

    public void AddStat(IStat stat)
    {
        AddChild(stat.GetAsNode);
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
            if (node is IStat stat)
            {
                var statCopy = stat.CloneStat();
                stat.StatChanged += StatChangedInvoke;
                Stats.Add(statCopy.Name, statCopy);
                AddChild(statCopy.GetAsNode);
            }
        }
    }

    public void CloneStats(IStatManager statManager)
    {
        CloneStats(statManager.StatManager);
    }

    public void CloneStats(StatManager statManager)
    {
        foreach (var item in statManager.Stats)
            if (!Stats.ContainsKey(item.Key))
            {
                var stat = item.Value.CloneStat();
                AddStat(stat);
            }else
                 AddStat(item.Value.CloneStat());
    }

    public void AddStatModifier(string statName, IStatModifier modifier)
    {
        Stats[statName].AddModifier(modifier);
    }

    public void RemoveStatModifier(string statName, IStatModifier modifier)
    {
        Stats[statName].RemoveModifier(modifier);
    }

    // public IStat GetStat(string statName)
    // {
    //     return Stats[statName];
    // }

    public IStat<T> GetStat<T>(StringName name)
    {
        if (!Stats.TryGetValue(name, out IStat stat))
            throw new KeyNotFoundException($"Stat '{name}' not found.");

        if (stat is not IStat<T> typedStat)
        {
            throw new InvalidCastException(
                $"Stat '{name}' is {stat.ValueType.Name}, " +
                $"not {typeof(T).Name}.");
        }

        return typedStat;
    }

    public T GetStatCurrentValue<T>(string statName)
    {
        return GetStat<T>(statName).CurrentValue;
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
            if (item is IStat stat)
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
            str.Append(stat.Name + " " + stat.ToString() + "\n");
        return str.ToString();
    }
}