using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

public partial class Stat<T> : Node, IStat<T>
{
    public new StringName Name { get; set; }

    public int Index { get; set; }

    public T BaseValue { get; set; }

    public T MinValue { get; set; }
    public T MaxValue { get; set; }

    public bool HasMinValue { get; set; }
    public bool HasMaxValue { get; set; }

    public Type ValueType => typeof(T);

    public event Action<IStat> StatChanged;

    public IStatOperations<T> Operations { get; set; }

    public object GetValue()
    {
        return CurrentValue;
    }

    public List<StatModifier<T>> Modifiers { get; set; } = new List<StatModifier<T>>();

    [Export]
    public IStat.StatVisibility Visibility { get; set; } = IStat.StatVisibility.Visible;

    private T _currentValue;

    public T CurrentValue
    {
        get { return _currentValue; }
        set
        {
            _currentValue = value;
            if (EqualityComparer<T>.Default.Equals(_currentValue, value)) // setting same value doesnt trigger event
                return;
            StatChanged?.Invoke(this);
        }
    }

    public Node GetAsNode
    {
        get { return this; }
    }

    public override string ToString()
    {
        return CurrentValue.ToString();
    }

    public Stat(){}

    public Stat(string name, T baseValue, IStatOperations<T> statOperations)
    {
        Name = name;
        BaseValue = baseValue;
        CurrentValue = baseValue;
        Operations = statOperations;
    }

    public Stat(string name, T baseValue, T max, IStatOperations<T> statOperations)
    {
        Name = name;
        BaseValue = baseValue;
        CurrentValue = baseValue;
        MaxValue = max;
        HasMaxValue = true;
        Operations = statOperations;
    }

    public Stat(string name, T baseValue, T min, T max, IStatOperations<T> statOperations)
    {
        Name = name;
        BaseValue = baseValue;
        CurrentValue = baseValue;
        MaxValue = max;
        HasMaxValue = true;
        MinValue = min;
        HasMinValue = true;
        Operations = statOperations;
    }

    private event IStat.StatChangedEventHandler _statChanged;

    event IStat.StatChangedEventHandler IStat.StatChanged
    {
        add => _statChanged += value;
        remove => _statChanged -= value;
    }

    public IStat<T> CloneStat()
    {
        return new Stat<T>()
        {
            Name = Name,
            CurrentValue = CurrentValue,
            BaseValue = BaseValue,
            MinValue = MinValue,
            MaxValue = MaxValue,
            HasMinValue = HasMinValue,
            HasMaxValue = HasMaxValue,
            Operations = Operations
        };
    }

    IStat IStat.CloneStat()
    {
        return CloneStat();
    }

    public void Copy(IStat<T> source)
    {
        Name = source.Name;
        CurrentValue = source.CurrentValue;
        BaseValue = source.BaseValue;
        MinValue = source.MinValue;
        MaxValue = source.MaxValue;
        HasMinValue = source.HasMinValue;
        HasMaxValue = source.HasMaxValue;
        Operations = source.Operations;
    }

    public void Copy(IStat source)
    {
        if(source is not IStat<T> typedSource)
            throw new ArgumentException($"Cannot copy {source.Name} of type {source.ValueType} into {Name} of type Stat<{typeof(T).Name}>.");
        Copy(typedSource);
    }

    public void AddModifier(StatModifier<T> statModifier)
    {
        Modifiers.Add(statModifier);
        switch (statModifier.ValueType)
        {
            case StatModifierValueType.Percent:
                T newValue = Operations.Multiply(CurrentValue, statModifier.Value);
                statModifier.ValueChange = Operations.Subtract(newValue, CurrentValue);
                break;
            case StatModifierValueType.Flat:
                statModifier.ValueChange = statModifier.Value;
                break;
            default:
                break;
        }
        
        CurrentValue = Operations.Add(CurrentValue, statModifier.ValueChange);
    }

    public void RemoveModifier(StatModifier<T> statModifier)
    {
        Modifiers.Remove(statModifier);
        CurrentValue = Operations.Subtract(CurrentValue, statModifier.ValueChange);
    }

    public override void _Ready()
    {
        CurrentValue = BaseValue;
        MaxValue = BaseValue;
    }

    public void AddModifier(IStatModifier modifier)
    {
        throw new NotImplementedException();
    }

    public void RemoveModifier(IStatModifier modifier)
    {
        throw new NotImplementedException();
    }
}