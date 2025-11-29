using Godot;
using System;
// using System.Collections.Generic;
using Godot.Collections;

public partial class Game : Node3D
{
	PackedScene _worldScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.WorldScene);
	PackedScene _mainMenuScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.MainMenuScene);
	PackedScene _manualBattleScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.ManualBattleScene);

	PackedScene _campaignScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.CampaignScene);

	GameLogger gameLogger = GameLogger.Instance;

	MainMenu mainMenu;

	Data Data;

	World _curerentWorld = null;

	ManualBattleScene manualBattleScene = null;

	private static IGameScene _instance;
    public static IGameScene Instance
    {
        get
        {
            return _instance;
        }
        set { _instance = value; }
    }  

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameLogger.LogInfo("Game node ready");
		Data = GetNode<Data>("Data");
		mainMenu = GetNode<MainMenu>("MainMenu");
		mainMenu.NewGameNode.LoadGameResources(Data.Resources);
		mainMenu.ConnecMainMenu(this);
	}

	public void _on_StartNewGame(Dictionary<string, int> WorldGenParameters){
		GD.Print(GetSignalConnectionList("QuickGame"));
		var WorldGenParams = new WorldGenParams()
		{
			WorldGenParameters = WorldGenParameters,
			ResourceSliders = mainMenu.NewGameNode.ResourceValueSliders
		};
		var _campaign = _campaignScene.Instantiate<CampaignScene>();
		LoadWorld(_campaign, WorldGenParams);
		FreeMainMenu();
	}

	void LoadWorld(IGameScene scene, WorldGenParams worldGenParams)
    {
		_instance = scene;
        _curerentWorld = _worldScene.Instantiate<World>();
		_curerentWorld.WorldGenParameters = worldGenParams;
		_curerentWorld.Data = Data;
		_curerentWorld.GetNodes();
		AddChild(_curerentWorld);
		_curerentWorld.InitScene(scene);
    }

	public void _on_Skirmish_ButtonUp()
    {

        manualBattleScene = _manualBattleScene.Instantiate<ManualBattleScene>();
		LoadWorld(manualBattleScene, null);
		FreeMainMenu();
    } 

	void FreeMainMenu()
    {
        mainMenu.NewGameNode.StartNewGame -= _on_StartNewGame;
		mainMenu.QueueFree();
    }

	public void _on_QuickGame(){
		_curerentWorld = (World)_worldScene.Instantiate();
		var WorldGenParams = new WorldGenParams()
		{
			WorldGenParameters = new Dictionary<string, int>() { { "Players", 2 } },
			ResourceSliders = mainMenu.NewGameNode.ResourceValueSliders
		};
		var _campaign = _campaignScene.Instantiate<CampaignScene>();
		LoadWorld(_campaign, WorldGenParams);
		FreeMainMenu();
	}

	public void ConnectToOpenMainMenu(Node node){
		node.Connect("OpenMainMenu", new Callable(this, nameof(_on_OpenMainMenu)));
	}

	public void _on_OpenMainMenu()
	{
		mainMenu = (MainMenu)_mainMenuScene.Instantiate();
		if(_curerentWorld != null)
        {
            _curerentWorld.QueueFree();
        }
        else
        {
            manualBattleScene.QueueFree();
        }
		AddChild(mainMenu);
		mainMenu.ConnecMainMenu(this);
		mainMenu.NewGameNode.LoadGameResources(Data.Resources);
		GetTree().Paused = false;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
