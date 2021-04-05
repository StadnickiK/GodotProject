using Godot;
using System;

public class TransferLabel : HBoxContainer
{

    Label LeftText;
    Label RightText;
    SpinBox NumInput;
    Button FastLeft;
    Button Left;
    Button Right;
    Button FastRight;


    void GetNodes(){
        LeftText = GetNode<Label>("LeftText");
        RightText = GetNode<Label>("RightText");
        NumInput = GetNode<SpinBox>("Control/NumInput");
    }

    void _on_FastLeft_button_up(){

    }

    void _on_Left_button_up(){

    }

    void _on_Right_button_up(){

    }

    void _on_FastRight_button_up(){

    }

    public override void _Ready()
    {
        GetNodes();
    }

}
