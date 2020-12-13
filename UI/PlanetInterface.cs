using Godot;
using System;

public class PlanetInterface : Panel
{


    Button _closeButton = null;

    Header _header = null;

    OverviewPanel _panel = null;

    [Export]
    public string Title { get; set; } = "Title";

    void GetNodes(){
        _closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
        _header = GetNode<Header>("VBoxContainer/Header");
        _panel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
    }

    public void SetTitle(string title){
        _header.SetTitle(title);
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
