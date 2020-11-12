using Godot;
using System;

public class BaseStat : Node{

    public BaseStat(){}

    public BaseStat(string name){
        _name = name;
    }

    public BaseStat(string name, int baseValue){
        _name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
    }

    public BaseStat(string name, int baseValue, int max){
        _name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
    }

    public BaseStat(string name, int baseValue, int min,int max){
        _name = name;
        _baseValue = baseValue;
        _currentValue = baseValue;
        _maxValue = max;
        HasMaxValue = true;
        _minValue = min;
        HasMinValue = true;
    }

    private string _name;
    new public string Name
    {
        get { return _name; }
    }
    
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
    
    public bool HasMaxValue { get; set; } = false;

    private int _maxValue;
    public int MaxValue
    {
        get { return _maxValue; }
    }

    public bool HasMinValue { get; set; } = false; 

    private int _minValue;
    public int MinValue
    {
        get { return _minValue; }
        set { _minValue = value; }
    }

    public override void _Ready()
    {

    }
}