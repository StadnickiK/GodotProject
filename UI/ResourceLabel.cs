using Godot;
using System;

public class ResourceLabel : HBoxContainer
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
    
    void GetNodes(){
        _value = GetNode<Label>("Value");
        _resourceName = GetNode<Label>("Name");
    }

    public void SetResourceName(string name){
        ResourceName.Text = name;
    }

    public void SetValue(int value){
        Value.Text = value.ToString();
    }

     public void SetValueTooltip(string tip){
         Value.HintTooltip = tip;
     }

     public void SetResourceNameTip(string tip){
         ResourceName.HintTooltip = tip;
     }

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
