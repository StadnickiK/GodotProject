using Godot;
using System;
using System.Collections.Generic;

public class StatManager : Node{

    public Dictionary<string, BaseStat> Stats { get; set; } = new Dictionary<string, BaseStat>();

    public void AddStat(BaseStat stat){
        Stats.Add(stat.Name, stat);
    }

    public void RemoveStat(string statName){
        Stats.Remove(statName);
    }

    public override void _Ready()
    {

    }
}