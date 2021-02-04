using Godot;
using System;

public class ValueSlider : HBoxContainer
{
    

    Label _nameLabel = null;
    Label _valueLabel = null;

    Slider _slider = null;

    [Export]
    public string Text { get; set; } = "Value Name";

    [Export]
    public string ValueName = "ValueName";

    [Export]
    int _minValue = 0;

    [Export]
    int _maxValue = 100;

    [Export]
    string _hint = "";

    public int CurrentValue { get; set; }

    void GetNodes(){
        _nameLabel = GetNode<Label>("ValueName");
        _valueLabel = GetNode<Label>("CurrentValue");
        _slider = GetNode<Slider>("Value");
        
    }

    void _on_Value_changed(float Value){
        CurrentValue = (int)_slider.Value;
        _valueLabel.Text = CurrentValue.ToString();
    }

    void UpdateSlider(){
        _slider.MinValue = _minValue;
        _slider.Value = _minValue;
        _slider.MaxValue = _maxValue;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        CurrentValue = (int)_slider.Value;
        _valueLabel.Text = CurrentValue.ToString();
        _nameLabel.Text = Text; 
        HintTooltip = _hint;
        UpdateSlider();
    }

    public void SetValueName(string name){
        _nameLabel.Text = name;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
