using Godot;
using System;

public partial class ResourceLabel : HBoxContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    

    private Label _resourceName = null;
    public Label ResourceName
    {
        get { return _resourceName; }
    }

    private Label _value;
    public Label Value
    {
        get { return _value; }
    }

    private Label _income;
    public Label Income
    {
        get { return _income; }
    }
    
    void GetNodes(){
        _value = GetNode<Label>("Value");
        _income = GetNode<Label>("Income");
        _resourceName = GetNode<Label>("Name");
    }

    public void Update(string name, int value, int income){
        SetResourceName(name);
        SetValue(value);
        SetIncome(income);
    }

    public void Update(int value, int income){
        SetValue(value);
        SetIncome(income);
    }

    public void SetResourceName(string name){
        ResourceName.Text = name;
    }

    public void SetValue(int value){
        Value.Text = value.ToString();
    }

    public void SetIncome(int income){
        Value.Text = income.ToString();
    }

     public void SetValueTooltip(string tip){
         Value.TooltipText = tip;
     }

     public void SetTip(string tip){
         TooltipText = tip;
     }

     public void SetLabelTheme(Theme theme){
        _value.Theme = theme;
        _resourceName.Theme = theme;
     }

    // public override Control _MakeCustomTooltip(string forText)
    // {
    //     var tooltip = new RichTextLabel();
    //     tooltip.Text = TooltipText;
    //     return tooltip;
    // }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
