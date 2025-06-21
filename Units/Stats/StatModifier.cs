using Godot;
using System;

public enum StatModifierValueType
{
    Flat,       // Direct addition (e.g., +5)
    Percent     // Percentage of base value (e.g., +10%)
}


public class StatModifier
{

    public int Index { get; set; }
    public string Name { get; set; }
    public float Value { get; set; }
    public float ValueChange { get; set; } = 0;
    
    public StatModifierValueType ValueType { get; }

    public double Duration { get; set; } = -1;

    public Node Source { get; }

    public StatModifier(float value, StatModifierValueType type, Node source = null)
    {
        Value = value;
        ValueType = type;
        Source = source;
    }
}