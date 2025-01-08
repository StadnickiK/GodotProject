using Godot;
using System;

public partial class MainMenuButton : Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    public delegate void OpenMainMenuEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var game = (Game)GetNode("/root/Game");
        game.ConnectToOpenMainMenu(this);
    }

    public void _on_MainMenu_button_up(){
        EmitSignal("OpenMainMenu");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
