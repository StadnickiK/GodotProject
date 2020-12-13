using Godot;
using System;

public class Header : HBoxContainer
{
    Button button = null;

    Label _TitleLabel = null;

    [Export]
    public string Title { get; set; } = "Title";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        _TitleLabel.Text = Title;
    }

    public void SetTitle(string title){
        _TitleLabel.Text = title;
    }

    void GetNodes(){
        button = GetNode<Button>("XButton");
        _TitleLabel = GetNode<Label>("Title");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
