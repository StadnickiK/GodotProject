using Godot;
using System;

public partial class MainMenu : Control
{
    
    CanvasItem _menuNode = null;

    MenuPanel _newGameNode = null;

    Label _TitleLabel = null;

    [Export]
    string Title = "Title";

    public MenuPanel NewGameNode { get => _newGameNode; set => _newGameNode = value; }

    public Button Skirmish { get; set; }

    public Button QuickGame { get; set; }

    public CustomBatttleMenu CustomBatttleMenu { get; set; }

    void GetNodes(){
        _menuNode = GetNode<CanvasItem>("Menu");
        NewGameNode = GetNode<MenuPanel>("NewGame");
        CustomBatttleMenu =  GetNode<CustomBatttleMenu>("CustomBatttleMenu");    
        NewGameNode.CloseButton.ButtonUp += _on_ShowMainMenu;
        CustomBatttleMenu.CloseButton.ButtonUp += _on_ShowMainMenu;
        Skirmish = _menuNode.GetNode<Button>("Skirmish");
        Skirmish.ButtonUp += _on_CustomBattle_button_up;
        QuickGame = _menuNode.GetNode<Button>("QuickGame");
        _TitleLabel = GetNode<Label>("Menu/Label");
        _TitleLabel.Text = Title;
    }

    public override void _Ready()
    {
        GetNodes();
        NewGameNode.Visible = false;
    }

    void _on_NewGame_button_up(){
        _menuNode.Hide();
        NewGameNode.Show();
    }

    void _on_CustomBattle_button_up(){
        _menuNode.Hide();
        CustomBatttleMenu.Show();
    }

    void _on_ShowMainMenu()
    {
        _menuNode.Show();
    }

    public void InitializeMainMenu(Game game, Data data)
    {
        ConnectMainMenu(game);
        CustomBatttleMenu.Initialize(data);
        
    }

    void ConnectMainMenu(Game game)
    {
        NewGameNode.StartNewGame += game._on_StartNewGame;
		CustomBatttleMenu.PlayButton.ButtonUp += game._on_Skirmish_ButtonUp;
        QuickGame.ButtonUp += game._on_QuickGame;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
