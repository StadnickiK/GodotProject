using Godot;
using System;

public class MainMenuButton : Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    delegate void OpenMainMenu();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var game = (Game)GetNode("/root/Game");
        game.ConnectToOpenMainMenu(this);
    }

    public void _on_MainMenu_button_up(){
        EmitSignal(nameof(OpenMainMenu));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
