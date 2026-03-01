using Godot;
using System;
// using System.Collections.Generic;
using Godot.Collections;

public partial class Game : Node3D
{
	PackedScene _worldScene;
	PackedScene _mainMenuScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.MainMenuScene);
	PackedScene _manualBattleScene;

	PackedScene _campaignScene;

	PackedScene _PlayerScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.PlayerScene);

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
		mainMenu.InitializeMainMenu(this, Data);
	}

	public static PackedScene LoadScene(string path)
    {
        return (PackedScene)ResourceLoader.Load(path);
    }

	void LoadCampaignScene()
    {
        if(_campaignScene == null) _campaignScene = LoadScene(ScenePaths.Instance.CampaignScene);
    }

	void LoadManualBattleScene()
    {
        if(_manualBattleScene == null) _manualBattleScene = LoadScene(ScenePaths.Instance.ManualBattleScene);
    }

	void LoadWorldScene()
    {
        if(_worldScene == null) _worldScene = LoadScene(ScenePaths.Instance.WorldScene);
    }
	

	public void _on_StartNewGame(Dictionary<string, int> WorldGenParameters){
		GD.Print(GetSignalConnectionList("QuickGame"));
		LoadCampaignScene();
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
		LoadWorldScene();
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
		LoadManualBattleScene();
        manualBattleScene = _manualBattleScene.Instantiate<ManualBattleScene>();
		// var Attackers = new System.Collections.Generic.List<IEnterCombatBase>();
		// var Defenders = new System.Collections.Generic.List<IEnterCombatBase>();
		// foreach (var item in mainMenu.CustomBatttleMenu.PlayerContainerControler.CustomBattlePlayerContainers)
		// {
		// 	if(item.Index % 2 == 0)
		// 	{
		// 		Attackers.Add(item.Combatant);
		// 	}else
		// 		Defenders.Add(item.Combatant);
		// }
		var WorldGenParams = new WorldGenParams()
		{
			WorldGenParameters = new Dictionary<string, int>() { { "Players", 0 }, {"Seed", Guid.NewGuid().GetHashCode() } },
			Combatants = mainMenu.CustomBatttleMenu.PlayerContainerControler.Combatants,
			BattlefieldSettings = mainMenu.CustomBatttleMenu.MapSettings.BattlefieldSettings,
			// Attackers = Attackers,
			// Defenders = Defenders
			
		};
		LoadWorld(manualBattleScene, WorldGenParams);
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
			WorldGenParameters = new Dictionary<string, int>() { { "Players", 2 }, {"Seed", Guid.NewGuid().GetHashCode() } },
			ResourceSliders = mainMenu.NewGameNode.ResourceValueSliders
		};
		LoadCampaignScene();
		var _campaign = _campaignScene.Instantiate<CampaignScene>();
		LoadWorld(_campaign, WorldGenParams);
		FreeMainMenu();
	}

	public void ConnectToOpenMainMenu(Node node){
		node.Connect("OpenMainMenu", new Callable(this, nameof(_on_OpenMainMenu)));
	}

	public void _on_OpenMainMenu()
	{
		GetTree().Paused = false;
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
		mainMenu.NewGameNode.LoadGameResources(Data.Resources);
		mainMenu.InitializeMainMenu(this, Data);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
