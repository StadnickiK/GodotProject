using Godot;
using System;

public class MainMenu : Control
{
    
    CanvasItem _menuNode = null;

    CanvasItem _newGameNode = null;

    Label _TitleLabel = null;

    [Signal]
    delegate void QuickGame();

    [Export]
    string Title = "Title";

    void GetNodes(){
        _menuNode = GetNode<CanvasItem>("Menu");
        _newGameNode = GetNode<CanvasItem>("NewGame");
        _TitleLabel = GetNode<Label>("Menu/Label");
        _TitleLabel.Text = Title;
    }

    public override void _Ready()
    {
        GetNodes();
        _newGameNode.Visible = false;
    }

    void _on_NewGame_button_up(){
        //_menuNode.Visible = false;
        _newGameNode.Visible = true;
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
