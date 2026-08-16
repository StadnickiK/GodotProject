using Godot;
using System;

public enum StatModifierValueType
{
    Flat,       // Direct addition (e.g., +5)
    Percent     // Percentage of base value (e.g., +10%)
}

public interface IStatModifier
{
    public int Index { get; set; }

    public StringName Name { get; set; }

    public StatModifierValueType ValueType { get; }

    public double Duration { get; }

    public Node Source { get; }
}
