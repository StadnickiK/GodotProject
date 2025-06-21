using Godot;
using System;
// using System.Collections.Generic;
using Godot.Collections;

public partial class Game : Node3D
{
	PackedScene _worldScene = (PackedScene)ResourceLoader.Load("res://World/World.tscn");
	PackedScene _mainMenuScene = (PackedScene)ResourceLoader.Load("res://Menu/MainMenu.tscn");
	GameLogger gameLogger = GameLogger.Instance;

	World _curerentWorld = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameLogger.LogInfo("Game node ready");
	}

	public void ConnectToStartNewGame(Node node){
		node.Connect("StartNewGame", new Callable(this, nameof(_on_StartNewGame)));
		GD.Print(node.GetSignalConnectionList("StartNewGame"));
		
	}

	public void _on_StartNewGame(Dictionary<string, int> WorldGenParameters){
		GD.Print(GetSignalConnectionList("QuickGame"));
	   _curerentWorld = (World)_worldScene.Instantiate();
	   GetNode("MainMenu").QueueFree();
	   _curerentWorld.WorldGenParameters = WorldGenParameters;
	   AddChild(_curerentWorld);
	}

	public void ConnectToQuickGame(Node node){
		node.Connect("QuickGame", new Callable(this, nameof(_on_QuickGame)));
		GD.Print(node.GetSignalConnectionList("QuickGame"));
		GD.Print(GetSignalConnectionList("QuickGame"));
	}

	public void _on_QuickGame(){
		_curerentWorld = (World)_worldScene.Instantiate();
	   var menu = GetNode("MainMenu");
	   menu.QueueFree();
	   AddChild(_curerentWorld);
	}

	public void ConnectToOpenMainMenu(Node node){
		node.Connect("OpenMainMenu", new Callable(this, nameof(_on_OpenMainMenu)));
	}

	public void _on_OpenMainMenu(){
		var menu = (MainMenu)_mainMenuScene.Instantiate();
	   	GetNode("World").QueueFree();
	   	AddChild(menu);
	   	ConnectToQuickGame(menu);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
