using Godot;
using System;

public class BaseStat : Node{

    public BaseStat(){}

    public BaseStat(BaseStat stat){
        Name = stat.Name;
        _name = stat.Name;
        _baseValue = stat.BaseValue;
        _currentValue = _baseValue;
    }

    public BaseStat(string name){
        _name = name;
        Name = name;
    }

    public BaseStat(string name, int baseValue){
        _name = name;
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
    }

    public BaseStat(string name, int baseValue, int max){
        _name = name;
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
    }

    public BaseStat(string name, int baseValue, int min,int max){
        _name = name;
        Name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
        _minValue = min;
        HasMinValue = true;
    }

    private string _name;
    public string StatName
    {
        get { return _name; }
    }
    
    [Export]
    private int _baseValue = 0;
    public int BaseValue
    {
        get { return _baseValue; }
    }

    private int _currentValue = 0;
    public int CurrentValue
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
        if(Name.Length < 1)
            _name = Name;
    }
}