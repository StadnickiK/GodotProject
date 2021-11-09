using Godot;
using System;

public class AlertBox : Panel
{
    
    private Label _label;
    public Label Text
    {
        get { return _label; }
    }

    Button _button;
    
    void GetNodes(){
        _label = GetNode<Label>("Label");
        _button = GetNode<Button>("Button");
    }

    public override void _Ready()
    {
        GetNodes();
    }

    void _on_Button_button_up(){
        Visible = false;
    }

}
