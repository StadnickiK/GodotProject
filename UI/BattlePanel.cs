using Godot;
using System;

public class BattlePanel : Panel
{
    
    Button _closeButton = null;

    void GetNodes(){
        _closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
    }

    public override void _Ready()
    {
        GetNodes();
        _closeButton.Connect("button_up", this, nameof(_on_XButton_button_up));
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
