using Godot;
using System;


public class StatModifier<T> : IStatModifier
{
    public T Value { get; protected set; }
    public T ValueChange { get; set; }
    public int Index { get; set; }
    public StringName Name { get; set; }
    public double Duration { get; protected set; }
    public Node Source { get; set; }
    public StatModifierValueType ValueType { get; protected set; }

    public StatModifier(T value, StatModifierValueType type, Node source = null)
    {
        Value = value;
        ValueType = type;
        Source = source;
    }

}