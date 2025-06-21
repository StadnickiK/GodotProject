using Godot;
using System;
using System.Collections.Generic;

public partial class BaseStat : Node{

    public BaseStat(){}
    
    private List<StatModifier> Modifiers = new List<StatModifier>();

    public BaseStat(BaseStat stat)
    {
        Name = stat.Name;
        _baseValue = stat.BaseValue;
        _currentValue = _baseValue;
    }

    public BaseStat(string name){
        Name = name;
    }

    public BaseStat(string name, int baseValue){
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
    }

    public BaseStat(string name, int baseValue, int max){
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
    }

    public BaseStat(string name, int baseValue, int min,int max){
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
        _minValue = min;
        HasMinValue = true;
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
    }

    private float _currentValue = 0;
    public float CurrentValue
    {
        get { return _currentValue; }
        set { _currentValue = value; }
    }
    [Export]
    public bool HasMaxValue { get; set; } = false;

    [Export]
    private int _maxValue;
    public int MaxValue
    {
        get { return _maxValue; }
    }
    [Export]
    public bool HasMinValue { get; set; } = false; 
    [Export]
    private int _minValue;
    public int MinValue
    {
        get { return _minValue; }
        set { _minValue = value; }
    }

    public override void _Ready()
    {
        CurrentValue = BaseValue;
    }
}