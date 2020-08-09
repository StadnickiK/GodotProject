using Godot;
using System;

public class NewSceneButton : Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export(PropertyHint.File, "*.tscn")]
    string newScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void _on_button_up(){
        GetTree().ChangeScene(newScene);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
