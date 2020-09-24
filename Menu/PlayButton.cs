using Godot;
using System;

public class PlayButton : Button
{
    Control Menu;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Menu = (Control)GetParent().GetParent().GetParent();//(Control)GetNode("World/Menu");
        //Menu = (Control)GetNode("Menu");
    }

    public void _on_PlayButton_button_up(){
        Menu.Visible = false;
    }
}
