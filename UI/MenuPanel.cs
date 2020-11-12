using Godot;
using System;

public class MenuPanel : Panel
{
    Button _closeButton = null;

    Label _titleLabel = null;

    [Export]
    public string Title { get; set; } = "Title";
    // Called when the node enters the scene tree for the first time.

    void GetNodes(){
        _closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
        _titleLabel = GetNode<Label>("VBoxContainer/Header/Title");
    }

    public override void _Ready()
    {
        GetNodes();
        _closeButton.Connect("button_up", this, nameof(_on_XButton_button_up));
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

}

