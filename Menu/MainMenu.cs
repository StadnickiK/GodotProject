using Godot;
using System;

public partial class MainMenu : Control
{
    
    CanvasItem _menuNode = null;

    MenuPanel _newGameNode = null;

    Label _TitleLabel = null;

    [Signal]
    public delegate void QuickGameEventHandler();

    [Export]
    string Title = "Title";

    public MenuPanel NewGameNode { get => _newGameNode; set => _newGameNode = value; }

    void GetNodes(){
        _menuNode = GetNode<CanvasItem>("Menu");
        NewGameNode = GetNode<MenuPanel>("NewGame");
        _TitleLabel = GetNode<Label>("Menu/Label");
        _TitleLabel.Text = Title;
    }

    public override void _Ready()
    {
        GetNodes();
        NewGameNode.Visible = false;
    }

    void _on_NewGame_button_up(){
        //_menuNode.Visible = false;
        NewGameNode.Visible = true;
    }

    void _on_Play_button_up(){
        EmitSignal(nameof(QuickGame));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
