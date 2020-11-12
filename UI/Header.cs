using Godot;
using System;

public class Header : HBoxContainer
{
    Button button = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
    }

    void GetNodes(){
        button = GetNode<Button>("XButton");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
