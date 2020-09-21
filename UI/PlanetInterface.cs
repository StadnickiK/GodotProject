using Godot;
using System;

public class PlanetInterface : TextureRect
{


    Button CloseButton = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CloseButton = GetNode<Button>("VBoxContainer/Header/XButton");
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

}
