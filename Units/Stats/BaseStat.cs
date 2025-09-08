using Godot;
using System;
using System.Collections.Generic;


public interface IStat
{
    public delegate void StatChangedEventHandler(IStat stat);
    public event StatChangedEventHandler StatChanged;

    public float CurrentValue { get; set; }

    public float MaxValue { get; set; }

    public bool HasMaxValue { get; set; }

    public float MinValue { get; set; }

    public bool HasMinValue { get; set; }

    public float BaseValue { get; set; }

    public StringName Name { get; set; }

    public List<StatModifier> Modifiers { get; set; } 

    public void AddModifier(StatModifier statModifier);

    public void RemoveModifier(StatModifier statModifier);

    public IStat CloneStat(IStat stat);
}

public partial class BaseStat : Node, IStat
{

    public event IStat.StatChangedEventHandler StatChanged;

    public BaseStat() { }

    public List<StatModifier> Modifiers { get; set; } = new List<StatModifier>();

    private float _currentValue = 0;
    public float CurrentValue
    {
        get { return _currentValue; }
        set { _currentValue = value; StatChanged?.Invoke(this); }
    }

    public BaseStat(BaseStat stat)
    {
        CloneStat(stat);
    }

    public BaseStat(string name)
    {
        Name = name;
    }

    public BaseStat(string name, int baseValue)
    {
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
    }

    public BaseStat(string name, int baseValue, int max)
    {
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
    }

    public BaseStat(string name, int baseValue, int min, int max)
    {
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
        _minValue = min;
        HasMinValue = true;
    }

    public IStat CloneStat(IStat stat)
    {
        Name = stat.Name;
        _baseValue = stat.BaseValue;
        _currentValue = stat.CurrentValue;
        _maxValue = stat.MaxValue;
        HasMaxValue = stat.HasMaxValue;
        _minValue = stat.MinValue;
        HasMinValue = stat.HasMinValue;
        Modifiers = new List<StatModifier>(stat.Modifiers);
        return this;
    }

    public void AddModifier(StatModifier statModifier)
    {
        Modifiers.Add(statModifier);
        switch (statModifier.ValueType)
        {
            case StatModifierValueType.Percent:
                float newValue = _currentValue * statModifier.Value;
                statModifier.ValueChange = newValue - CurrentValue;
                break;
            case StatModifierValueType.Flat:
                statModifier.ValueChange = statModifier.Value;
                break;
            default:
                break;
        }
        _currentValue += statModifier.ValueChange;
    }

    public void RemoveModifier(StatModifier statModifier)
    {
        Modifiers.Remove(statModifier);
        _currentValue -= statModifier.ValueChange;
    }

    public int Index { get; set; }

    [Export]
    private float _baseValue = 0;
    public float BaseValue
    {
        get { return _baseValue; }
        set{ _baseValue = value; }
    }
    [Export]
    public bool HasMaxValue { get; set; } = false;

    [Export]
    private float _maxValue;
    public float MaxValue
    {
        get { return _maxValue; }
        set { _maxValue = value; }
    }
    [Export]
    public bool HasMinValue { get; set; } = false;
    [Export]
    private float _minValue = 0;
    public float MinValue
    {
        get { return _minValue; }
        set { _minValue = value; }
    }

    public override void _Ready()
    {
        CurrentValue = BaseValue;
        _maxValue = BaseValue;
    }
}