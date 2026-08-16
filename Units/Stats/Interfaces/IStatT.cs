using Godot;
using System;
using System.Collections.Generic;

public interface IStat<T> : IStat
{

    public T CurrentValue { get; set; }

    public T MaxValue { get; set; }

    public bool HasMaxValue { get; set; }

    public T MinValue { get; set; }

    public bool HasMinValue { get; set; }

    public T BaseValue { get; set; }

    public IStatOperations<T> Operations { get; set; }


    public List<StatModifier<T>> Modifiers { get; set; } 

    public void AddModifier(StatModifier<T> StatModifier);

    public void RemoveModifier(StatModifier<T> StatModifier);

    new IStat<T> CloneStat();

    void Copy(IStat<T> source);
}
