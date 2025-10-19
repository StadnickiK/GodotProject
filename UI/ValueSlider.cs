using Godot;
using System;

public partial class ValueSlider : HBoxContainer
{


    Label _nameLabel = null;
    SpinBox _SpinBox = null;

    Slider _slider = null;

    public delegate void SliderEventHandler(string name, float value);

    public SliderEventHandler SliderChanged;

    [Export]
    public string Text { get; set; } = "Value Name";

    [Export]
    public string ValueName { get; private set; } = "ValueName";

    [Export]
    int _minValue = 0;

    [Export]
    int _maxValue = 100;

    [Export]
    string _hint = "";

    public int CurrentValue { get; set; }
    public Label NameLabel { get => _nameLabel; private set => _nameLabel = value; }

    void GetNodes()
    {
        NameLabel = GetNode<Label>("ValueName");
        _SpinBox = GetNode<SpinBox>("CurrentValue");
        _slider = GetNode<Slider>("Value");

    }

    void _on_Value_changed(float Value)
    {
        CurrentValue = (int)_slider.Value;
        _SpinBox.Value = CurrentValue;
        SliderChanged?.Invoke(ValueName, Value);
    }

    /// <summary>
    /// SpinBox signal updates value range
    /// </summary>
    /// <param name="CurrentValue"></param>
    void _on_current_value_value_changed(float Value)
    {
        CurrentValue = (int)Value;
        _slider.Value = CurrentValue;
        SliderChanged?.Invoke(ValueName, Value);
    }

    void UpdateSlider()
    {
        _slider.MinValue = _minValue;
        _slider.Value = _minValue;
        _slider.MaxValue = _maxValue;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        CurrentValue = (int)_slider.Value;
        _SpinBox.Value = CurrentValue;
        NameLabel.Text = Text;
        TooltipText = _hint;
        UpdateSlider();
    }

    public void SetValueName(string name)
    {
        ValueName = name;
        NameLabel.Text = name;
    }

    public void SetMaxValue(double value)
    {
        _slider.MaxValue = value;
        _SpinBox.MaxValue = value;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
