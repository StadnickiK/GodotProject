using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public interface IWorld
{
    public WorldCursorControl WCC { get; }

	public Player Host { set; get; }

	public Ground Ground { get; set; }

	public Data Data { get; set; }

	public Random Rand { get; set; }

	public CameraGimbal Camera3D { get; set;}

	public List<Player> PlayersList { get; set; }

	public int PlayerNumber { get; set; }

	public int UnitCount { get; set; }

	public WorldGenParams WorldGenParameters { get; set; }

	public Node GetAsNode { get; }

	public void AddPlayer(Player player);
}

public partial class World : Node3D, IWorld
{

	[Export]
	public int UnitCount { get; set; } = 4;

	public enum GameAlert
	{
		NoResource
	}

	public delegate void SplitShipEventHandler(ShipModel shipStruct);

	public delegate void FreeShipEventHandler(Ship ship);

	public delegate void TransferUnitsEventHandler(IPlanetInterface host, IEnterCombat target);

	private WorldCursorControl _wcc = null;
	public WorldCursorControl WCC
	{
		get { return _wcc; }
	}

	public Node GetAsNode { get { return this; } }

	public Ground Ground { get; set;}

	private Data _data = null;

	public Data Data { get => _data; set => _data = value; }
	public CameraGimbal Camera3D { get; set;}

	Control WorldMenu { get; set; }

	PackedScene _PlayerScene = (PackedScene)ResourceLoader.Load(ScenePaths.Instance.PlayerScene);

	public Random Rand { get; set; }

	void InitRand(int seed)
	{
		// if (seed == -1313)
		// 	seed = Guid.NewGuid().GetHashCode();
		Rand = new Random(seed);
	}

	private Player _Player = null;
	public Player Host
	{
		set {_Player = value; } get { return _Player; }
	} 

    Node Players = null;

	public List<Player> PlayersList { get; set; } = new List<Player>();

	public WorldGenParams WorldGenParameters { get; set; }

	[Export]
	public int PlayerNumber { get; set; } = 1;

	public void _on_LookAtStarSystem(StarSystem system)
	{
		if (system != null)
			Camera3D.LookAt(system.GlobalTransform.Origin);

	}

	public void GetNodes()
	{
		InitRand(WorldGenParameters.WorldGenParameters["Seed"]);
		WorldMenu = GetNode<Control>("Menu");
		Players = GetNode("Players");
		_wcc = GetNode<WorldCursorControl>("WorldCursorControl");
		Ground = GetNode<Ground>("Ground");
		Camera3D = GetNode<CameraGimbal>("CameraGimbal");
	}

	public void InitScene(IGameScene scene)
    {
        AddChild(scene.GetAsNode);
		scene.InitializeScene(this);
    }

	public void AddPlayer(Player player)
    {
		if(player.GetParent() == null) Players.AddChild(player);
		player.PlayerID = PlayersList.Count;
		PlayersList.Add(player);
    }

	void InitPlayers()
	{
		if (WorldGenParameters != null)
		{
			if (WorldGenParameters.WorldGenParameters.ContainsKey("Players"))
				PlayerNumber = WorldGenParameters.WorldGenParameters["Players"];
			
		}
		GD.Print("World 1: " + GetInstanceId());
		for (int i = 0; i < PlayerNumber; i++)
		{
			// var player = new AIPlayer(_data);//(Player)_PlayerScene.Instance();
			var player = (Player)_PlayerScene.Instantiate();
			// player.SetMap(_map);
			AddPlayer(player);
			if (i == 0)
			{
				_Player = player;
				player.IsLocal = true;
			}
		}
	}

	void InitAIPlayers(Map map)
	{
		if (WorldGenParameters != null)
		{
			if (WorldGenParameters.WorldGenParameters.ContainsKey("Players"))
			{
				PlayerNumber = WorldGenParameters.WorldGenParameters["Players"];
			}
		}
		for (int i = 0; i < PlayerNumber; i++)
		{
			var player = new AIPlayer(Data);  //(Player)_PlayerScene.Instance();
			player.SetMap(map);
			AddPlayer(player);
			if (i == 0)
			{
				_Player = player;
				player.IsLocal = true;
			}
		}
	}

	void ConnectPlayers()
	{
		foreach (var player in PlayersList)
		{
			if (player is AIPlayer ai)
			{
				ai.ConnectSignals();
			}
		}
	}

	void InitPlayerStartResources()
	{
		if(WorldGenParameters.ResourceSliders != null)
        {
			var resources = Data.GetResourcesIndexList();
			var starResources = Data.GetStartResources();
			foreach (var item in WorldGenParameters.ResourceSliders)
			{
				if (starResources.ContainsKey(item.Key)){
					starResources[item.Key] = item.Value.CurrentValue;
				} else {
					starResources.Add(item.Key, item.Value.CurrentValue);
				}
			}
			foreach (var player in PlayersList)
			{
				// without new dictionary the game would use one for all of those instead of separate ones
				player.ResManager.Resources = new Dictionary<int, int>(starResources);

				player.ResManager.Production.Upkeep = new Dictionary<int, int>(resources);
				player.ResManager.ResourceLimits.Upkeep = new Dictionary<int, int>(resources);
				player.ResManager.ProdCost.Upkeep = new Dictionary<int, int>(resources);
				player.ResManager.Upkeep.Upkeep = new Dictionary<int, int>(resources);
				player.ResManager.TotalProduction.Upkeep = new Dictionary<int, int>(resources);
			}
        }
	}

	bool CheckBuildingResources(Planet planet, Building building)
	{
		if (building.Type == Building.Category.Mine)
			foreach (var resName in building.Products.Keys)
			{
				if (!planet.ResourcesManager.Resources.ContainsKey(resName))
				{
					return false;
				}
			}
		return true;
	}

	void InitAvaiableBuildings(Planet planet)
	{ // todo: Separate construction and building list into separate nodes for better organization
		var construction = planet.BuildingManager.CurrentConstruction();
		foreach (var building in Data.Buildings)
			if (building.Requirements.Count == 0)
				if (CheckBuildingResources(planet, building))
					if (planet.BuildingManager.Buildings.Find(x => x.Name == building.Name) == null)
						planet.BuildingManager.AvaiableBuildings.Add(building);
	}

	void InitWorldBuildings()
	{
		var startBuildings = new List<Building>();
		foreach (var building in Data.Buildings)
		{
			if (building.IsStarter == true)
				startBuildings.Add(building);
		}
		foreach (Player player in Players.GetChildren())
		{
			foreach (Planet planet in player.MapObjects.Where(x => x is Planet))
			{
				planet.BuildingManager.AddBuildings(startBuildings);
				InitAvaiableBuildings(planet);
			}
			player.InitResourceLimit();
		}
	}

	void ChangePlayer(Player player)
	{

	}

	void InitWorld()
	{
		InitPlayers();
		// InitGalaxy();
		// UpdateGround();
		InitPlayerStartResources();
		// InitStartPlanets();
		// InitStartResources();
		// InitStartFleets();
		//InitResistance(); needs work
		InitWorldBuildings();
		// ConnectPlanets();
		ConnectPlayers();
	}

	public override void _Ready()
	{
		GetNodes();
		
		_wcc.camera = Camera3D.GetNode<Camera3D>("InnerGimbal/Camera3D");
		Ground.ConnectToInputEvent(WCC.OnGroundInputCallable);
		GD.Print("World: " + GetInstanceId());
		InitWorld();
		GD.Print("World: " + GetInstanceId());
	}
	// public override void _Process(double delta)
	// {
	// 	// if(_Player != null){
	// 	// 	_UI.UpdateUI(_Player);
	// 	// }
	// 	// if (Input.IsActionJustReleased("ui_cancel"))
	// 	// {
	// 	// 	_UI.WorldMenu.Visible = true;
	// 	// }
	// }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
		if (Input.IsActionJustReleased("ui_cancel"))
		{
			WorldMenu.Visible = !WorldMenu.Visible;
			GetTree().Paused = WorldMenu.Visible;
		}
    }

}
