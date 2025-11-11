using Godot;
using System;
// using System.Collections.Generic;
using Godot.Collections;

public partial class Game : Node3D
{
	PackedScene _worldScene = (PackedScene)ResourceLoader.Load("res://World/World.tscn");
	PackedScene _mainMenuScene = (PackedScene)ResourceLoader.Load("res://Menu/MainMenu.tscn");
	GameLogger gameLogger = GameLogger.Instance;

	MainMenu mainMenu;

	Data Data;

	World _curerentWorld = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameLogger.LogInfo("Game node ready");
		Data = GetNode<Data>("Data");
		mainMenu = GetNode<MainMenu>("MainMenu");
		mainMenu.NewGameNode.LoadGameResources(Data.Resources);
		mainMenu.NewGameNode.StartNewGame += _on_StartNewGame;
	}

	public void _on_StartNewGame(Dictionary<string, int> WorldGenParameters){
		GD.Print(GetSignalConnectionList("QuickGame"));
		var WorldGenParams = new WorldGenParams()
		{
			WorldGenParameters = WorldGenParameters,
			ResourceSliders = mainMenu.NewGameNode.ResourceValueSliders
		};
		_curerentWorld = (World)_worldScene.Instantiate();
		mainMenu.NewGameNode.StartNewGame -= _on_StartNewGame;
		mainMenu.QueueFree();
		_curerentWorld.WorldGenParameters = WorldGenParams;
		_curerentWorld.Data = Data;
		World.Instance = _curerentWorld;
		_curerentWorld.GetNodes();
		AddChild(_curerentWorld);
	}

	public void ConnectToQuickGame(Node node){
		node.Connect("QuickGame", new Callable(this, nameof(_on_QuickGame)));
		GD.Print(node.GetSignalConnectionList("QuickGame"));
		GD.Print(GetSignalConnectionList("QuickGame"));
	}

	public void _on_QuickGame(){
		_curerentWorld = (World)_worldScene.Instantiate();
		var WorldGenParams = new WorldGenParams()
        {
            WorldGenParameters = new Dictionary<string, int>() { { "Players", 2 } },
            ResourceSliders = mainMenu.NewGameNode.ResourceValueSliders
        };
		_curerentWorld.WorldGenParameters = WorldGenParams;
	   var menu = GetNode("MainMenu");
	   mainMenu.NewGameNode.StartNewGame -= _on_StartNewGame;
	   menu.QueueFree();
	   _curerentWorld.Data = Data;
		World.Instance = _curerentWorld;
	   _curerentWorld.GetNodes();
	   AddChild(_curerentWorld);
	}

	public void ConnectToOpenMainMenu(Node node){
		node.Connect("OpenMainMenu", new Callable(this, nameof(_on_OpenMainMenu)));
	}

	public void _on_OpenMainMenu()
	{
		mainMenu = (MainMenu)_mainMenuScene.Instantiate();
		GetNode("World").QueueFree();
		AddChild(mainMenu);
		mainMenu.NewGameNode.StartNewGame += _on_StartNewGame;
		mainMenu.NewGameNode.LoadGameResources(Data.Resources);
		ConnectToQuickGame(mainMenu);
		GetTree().Paused = false;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
