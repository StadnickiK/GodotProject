using Godot;
using System;

public class TransferLabel : HBoxContainer
{

    Label LeftText;
    Label RightText;

    public SpinBox NumInput { get; set; }
    Button FastLeft;
    Button Left;

    public int LeftValue { get; set; } = 0;

    public int RightValue { get; set; } = 0;
    Button Right;
    Button FastRight;

    public bool IsRight { get; set; } = true;

    void GetNodes(){
        LeftText = GetNode<Label>("LeftText");
        RightText = GetNode<Label>("RightText");
        Right = GetNode<Button>("Right");
        Left = GetNode<Button>("Left");
        NumInput = GetNode<SpinBox>("Control/NumInput");
    }

    void _on_FastLeft_button_up(){

    }

    void _on_Left_button_up(){
        IsRight = false;
        Right.Disabled = false;
        Left.Disabled = true;
    }

    void _on_Right_button_up(){
        IsRight = true;
        Right.Disabled = true;
        Left.Disabled = false;
    }

    void _on_FastRight_button_up(){

    }

    public override void _Ready()
    {
        GetNodes();
        if(IsRight){
            Right.Disabled = true;
            Left.Disabled = false;
        }else{
            Right.Disabled = false;
            Left.Disabled = true;
        }
    }

    public void UpdateLabel(string resourceName, int left, int right){
        Name = resourceName;
        LeftValue = left;
        RightValue = right;
        if(LeftText != null || RightText != null){
            LeftText.Text = resourceName +": "+left;
            RightText.Text = resourceName + ": "+right;
        }else{
            GetNodes();
            UpdateLabel(resourceName, left, right);
        }
    }
}
