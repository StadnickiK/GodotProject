using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node3D
{
	PackedScene _worldScene = (PackedScene)ResourceLoader.Load("res://World/World.tscn");
	PackedScene _mainMenuScene = (PackedScene)ResourceLoader.Load("res://Menu/MainMenu.tscn");

	World _curerentWorld = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void ConnectToStartNewGame(Node node){
		node.Connect("StartNewGame", new Callable(this, nameof(_on_StartNewGame)));
	}

	public void _on_StartNewGame(Dictionary<string, int> WorldGenParameters){
	   _curerentWorld = (World)_worldScene.Instantiate();
	   GetNode("MainMenu").QueueFree();
	   _curerentWorld.WorldGenParameters = WorldGenParameters;
	   AddChild(_curerentWorld);
	}

	public void ConnectToQuickGame(Node node){
		node.Connect("QuickGame", new Callable(this, nameof(_on_QuickGame)));
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
