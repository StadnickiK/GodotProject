using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class World : Node3D, IEndTurnListener
{

	[Export]
	public int UnitCount { get; private set; } = 4;

	public enum GameAlert
	{
		NoResource
	}

	public delegate void SplitShipEventHandler(ShipModel shipStruct);

	public delegate void FreeShipEventHandler(Ship ship);

	public delegate void TransferUnitsEventHandler(IPlanetInterface host, IEnterCombat target);

	private Map _map = null;

	public Map GetMap
	{
		get { return _map; }
	}

	ManualBattleScene ManualBattleScene;

	private WorldCursorControl _wcc = null;
	public WorldCursorControl WCC
	{
		get { return _wcc; }
	}

	Ground Ground;

	public MapArmyManager MapArmyManager { get; set; }

	private Data _data = null;

	public Data Data { get => _data; set => _data = value; }

	Galaxy Galaxy = null;
	CameraGimbal Camera3D = null;

	private UI _UI = null;
	public UI UInterface
	{
		get { return _UI; }
	}

	PackedScene _PlayerScene = (PackedScene)ResourceLoader.Load("res://World/Player.tscn");

	PackedScene _GalaxyScene = (PackedScene)ResourceLoader.Load("res://Map/Galaxy.tscn");

	[Export]
	int Seed = -1991794247;
	public Random Rand { get; set; }

	void InitRand()
	{
		if (Seed == -1313)
			Seed = Guid.NewGuid().GetHashCode();
		Rand = new Random(Seed);
	}

	private Player _Player = null;
	public Player Host
	{
		get { return _Player; }
	}
	
	private static World _instance;
    public static World Instance
    {
        get
        {
            return _instance;
        }
        set { _instance = value; }
    }   

    Node Players = null;

	List<Player> PlayersList = new List<Player>();

	public WorldGenParams WorldGenParameters;

	List<int> PlayerIDs = new List<int>();

	[Export]
	int PlayerNumber = 1;

	void _on_ShowBattlePanel(SpaceBattle battle)
	{
		_UI.BattlePan.Show();
		_UI.BattlePan.UpdatePanel(battle);
		_UI.RightPanel.Hide();
		_on_Deselect();
	}

	void _on_Battle_Retreat(Ship ship)
	{
		ManualBattleScene.ClearCombat();
		if (ship.Controller.IsLocal)
			_on_SelectUnit(ship);
		_UI.BattlePan.Hide();
		_UI.RightPanel.Show();
	}

	void _on_EndBattle()
	{
		//Ground.ConnectToInputEvent(WCC.OnGroundInputCallable);
		Ground.Show();
		_UI.BattlePan.Hide();
		_UI.RightPanel.Show();
		ManualBattleScene.EndCombat();
	}

	void _on_ResetBattle()
	{
		//Ground.ConnectToInputEvent(WCC.OnGroundInputCallable);
		_map.Hide();
		ManualBattleScene.Show();
		_UI.BattlePan.Hide();
		ManualBattleScene.ResetCombat();
	}

	void _on_ManualBattle()
	{
		//Ground.DisconnectInputEvent(WCC.OnGroundInputCallable);
		Ground.Hide();
		_UI.UpdateBattleUI();
		_map.Hide();
		ManualBattleScene.UpdateBattle();
	}
	void _on_EndManualBattle()
	{
		//_UI.UpdateBattleUI();
		_map.Show();
		ManualBattleScene.Hide();
	}

	public void Select(ISelection planet)
	{
		_on_Deselect();
		_wcc._SelectUnit(planet);
		if (planet is IPlanetInterface planetInterface)
		{
			_UI.PInterface.Show();
			_UI.PInterface.UpdatePlanetInterface(planetInterface);
		}
	}

	void _on_CreateShip(Planet planet, Unit unit)
	{
		CreateShip(planet, unit);
	}

	void _on_CameraLookAt(Vector3 position)
	{
		Camera3D.LookAt(position);
	}

	void _on_LookAtStarSystem(StarSystem system)
	{
		if (system != null)
			Camera3D.LookAt(system.GlobalTransform.Origin);

	}

	void _on_SelectObjectInOrbit(Planet planet, Node node)
	{
		if (planet != null && node != null)
		{
			var label = (Label)node;
			PhysicsBody3D obj = (PhysicsBody3D)node.GetMeta(label.Text); // 4 w/e reason node's name gets corrupted in overviewPanel connection method, but text is ok
			if (obj != null)
			{
				if (obj is ISelectMapObject selectMapObject)
				{
					selectMapObject.SelectMapObject();
				}
			}
		}
	}

	public void GetNodes()
	{
		_map = GetNode<Map>("Map");
		ManualBattleScene = GetNode<ManualBattleScene>("ManualBattleScene");
		Players = GetNode("Players");
		_wcc = GetNode<WorldCursorControl>("WorldCursorControl");
		_UI = GetNode<UI>("CanvasLayer/UI");
		Ground = GetNode<Ground>("Ground");
		Camera3D = _UI.GetNode<CameraGimbal>("CameraGimbal");
		MapArmyManager = GetNode<MapArmyManager>("MapArmyManager");
	}

	void ConnectSignals()
	{
		//_UI.RightPanel.ConnectToLookAt(this, nameof(_on_LookAtObject));
		_UI.RightPanel.WorldCamera = Camera3D;
		_UI.PInterface.ConnectToSelectObjectInOrbit(this, nameof(_on_SelectObjectInOrbit));
		_UI.ResourcePanel.InitResourcePanel(Data.Resources);
		_UI.PInterface._data = Data;
		_UI.PInterface.InitBuildingsPanel();
		_UI.PInterface.InitRecruitmentPanel();
		
		//_UI.UInfo.ConnectToChangeStance(_map, nameof(_map._on_UInfo_ChangeStance));
		_UI.OrbitList.Connect("SelectObject", new Callable(this, nameof(_on_SelectUnit)));
		_UI.CommandPanel.Connect("ShipCommand", new Callable(this, nameof(_on_ShipCommand)));
		//_map.Combat.SpaceBattle.OpenBattlePanel += _on_ShowBattlePanel;
		ManualBattleScene.OpenBattlePanel += _on_ShowBattlePanel;
		_UI.BattlePan.Center.Retreat.ButtonUp += () => _on_Battle_Retreat(ManualBattleScene.GetLocalAttakcerOrNull());
		_UI.BattlePan.Center.EndFight.ButtonUp += _on_EndBattle;
		_UI.BattlePan.Center.RepeatFight.ButtonUp += _on_ResetBattle;
		_UI.BattlePan.Center.Fight.ButtonUp += _on_ManualBattle;
		ManualBattleScene.CameraLookAt += Camera3D.LookAt;
		ManualBattleScene.EndBattle += _on_EndManualBattle;
		//_map.ConnectToShowBattlePanel(this, nameof(_on_ShowBattlePanel));
	}

	void ConnectLocalPlayer(Player player)
	{
		player.ArmiesChanged += _UI.RightPanel.UpdateRightPanel;
		player.ResManager.ProdChanged += _UI.ResourcePanel.UpdatePanel;
		player.ResManager.UpkeepChanged += _UI.ResourcePanel.UpdatePanel;
		player.ResManager.ProdCostChanged += _UI.ResourcePanel.UpdatePanel;
		player.ResManager.PlayerResourcesChanged += _UI.ResourcePanel.UpdatePanel;
		
	}

	void _on_SelectUnit(ISelection body)
	{
		_UI.PInterface.Hide();
		_wcc._SelectUnit(body);
	}

	void _on_Deselect()
	{
		_UI.PInterface.Hide();
		WCC.ClearSelection();
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
			Players.AddChild(player);
			player.PlayerID = player.GetIndex();
			if (i == 0)
			{
				_Player = player;
				player.IsLocal = true;
			}
			PlayerIDs.Add(player.PlayerID);
			PlayersList.Add(player);
		}
	}

	void InitAIPlayers()
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
			player.SetMap(_map);
			Players.AddChild(player);
			player.PlayerID = player.GetIndex();
			if (i == 0)
			{
				_Player = player;
				player.IsLocal = true;
			}
			PlayerIDs.Add(player.PlayerID);
			PlayersList.Add(player);
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

	void InitGalaxy()
	{
		var generator = new Generator();
		generator.InitGenerator(this, Rand, WorldGenParameters.WorldGenParameters);
		Galaxy = generator.GenerateGalaxy();
		generator.QueueFree();
		_map.AddChild(Galaxy);
		_map.galaxy = Galaxy;
		Galaxy.Connect("CameraLookAt", new Callable(this, nameof(_on_CameraLookAt)));
		Galaxy.Connect("LookAtStarSystem", new Callable(this, nameof(_on_LookAtStarSystem)));
	}

	void UpdateGround()
	{
		Ground.Scale = new Vector3(2 * Galaxy.Radius, 1, 2 * Galaxy.Radius);
	}

	void InitStartPlanets()
	{
		List<StarSystem> tempStarSystems = new List<StarSystem>(Galaxy.StarSystems);
		int count = tempStarSystems.Count;
		if (count > PlayerNumber)
		{
			foreach (int id in PlayerIDs)
			{
				var system = tempStarSystems[Rand.Next(0, count)];
				var planetList = system.Planets;
				var player = (Player)Players.GetChild(id);
				var planet = planetList[Rand.Next(0, planetList.Count)];
				planet.ChangeController(player);
				planet.VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct() { Visible = true, Visibility = VisibilityConroller.VisibilityState.Visible }, player.PlayerID);

			}
		}
		else
		{
			var usedPlanetList = new List<Planet>();
			foreach (int id in PlayerIDs)
			{
				var system = tempStarSystems[Rand.Next(0, count - 1)];
				var planetList = new List<Planet>(system.Planets);
				var planet = planetList[Rand.Next(0, planetList.Count)];
				var player = (Player)Players.GetChild(id);
				while (usedPlanetList.Contains(planet))
				{
					planet = system.Planets[Rand.Next(0, planetList.Count)];
				}
				planet.ChangeController(player);
				usedPlanetList.Add(planet);
				planet.VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct() { Visible = true, Visibility = VisibilityConroller.VisibilityState.Visible }, player.PlayerID);
			}
		}
	}

	void InitStartFleets()
	{
		foreach (Node node in Players.GetChildren())
		{
			if (node is Player player)
			{
				int maxFleets = 1;
				foreach (CollisionObject3D body in player.MapObjects.ToArray())
				{ // ToArray is needed because MapObjects list is modified inside foreach loop which raises exception
					if (body is Planet planet && maxFleets > 0)
					{
						var ship = MapArmyManager.CreateShip(planet, planet.Transform.Origin + new Vector3(3, 0, 3), planet.Name + " " + 1);
						ConnectShip(ship);
						for (int i = 0; i < UnitCount; i++)
						{
							var unit = Data.GetUnit(0);
							ship.UnitController.AddUnit(unit);
							// ship.Power.CurrentValue += new Unit().Stats["HitPoints"].CurrentValue;
						}
						ship.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct() { Visibility = VisibilityConroller.VisibilityState.Unexplored, Visible = _Player == player }, player.PlayerID, _Player == player);
						if (_Player == player)
						{
							ship.IsLocal = true;
						}
					}
				}
			}
		}
	}

	void CreateShip(ShipModel shipStruct)
	{
		var s = MapArmyManager.CreateShip(shipStruct);
		ConnectShip(s);
	}

	public Ship CreateShip(Planet planet, Unit unit)
	{
		var ship = MapArmyManager.CreateShip(planet, unit);
		ConnectShip(ship);
		//planet.Controller.AddMapObject(ship);
		//  planet.AddToOrbit(ship);
		return ship;
	}

	public void SplitShip(ShipModel shipStruct)
	{
		var s = MapArmyManager.CreateShip(shipStruct);
		ConnectShip(s);
		_on_Deselect();
		_on_SelectUnit(s);
	}

	void FreeShip(Ship ship)
	{
		MapArmyManager.FreeShip(ship);
	}

	void ConnectShip(Ship ship)
	{
		//WCC.ConnectToSelectTarget(ship);
		ManualBattleScene.ConnectToEnterCombat(ship);
		//_map.ConnectToEnterMapObject(ship);
		_map.ConnectToExitMapObject(ship);
		if (ship.Controller == _Player)
		{
			ship.OpenTransferPanel -= _UI.PInterface.UpdatePlanetInterface;
			ship.OpenTransferPanel += _UI.PInterface.UpdatePlanetInterface;
		}
		ship.FreeShip -= FreeShip;
		ship.FreeShip += FreeShip;
	}

	void InitStartResources()
	{
		foreach (StarSystem system in _map.galaxy.StarSystems)
		{
			foreach (Node node in system.StarSysObjects.GetChildren())
			{
				if (node is Planet planet)
				{
					// foreach(var resource in _data.WorldResources.Values){
					// 	if(resource.IsStarter == true && 
					// 	planet.Controller != null && 
					// 	(resource.ResourceType == Resource.Type.Ore)){
					// 		planet.ResourcesManager.Resources.Add(resource.Name, resource.Quantity);
					// 	}else if((resource.ResourceType == Resource.Type.Ore)){
					// 		if(Rand.Next(0,100)>(100 - resource.Rarity))
					// 			planet.ResourcesManager.Resources.Add(resource.Name, resource.Quantity);
					// 	}
					// }
					foreach (Resource resource in Data.Resources)
					{
						if (resource.IsStarter == true &&
							planet.Controller != null &&
							(resource.ResourceType == Resource.Type.Ore))
						{
							planet.ResourcesManager.Resources.Add(resource.Index, resource.Quantity);
							// GD.Print("1 ", rNode.Name);
							//planet.Resources.Add(resource.Index, resource.Quantity);
						}
						else if (resource.ResourceType == Resource.Type.Ore)
						{
							// GD.Print("2 ", rNode.Name);
							if (Rand.Next(0, 100) > (100 - resource.Rarity))
							{
								planet.ResourcesManager.Resources.Add(resource.Index, resource.Quantity);
								//planet.Resources.Add(resource.Index, resource.Quantity);
							}

						}
					}
					planet.InfoLabel.InitResources(planet, Data.Resources);
				}
			}
		}
	}

	void InitPlayerStartResources()
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

	void InitResistance()
	{
		foreach (StarSystem system in _map.galaxy.StarSystems)
		{
			foreach (Node node in system.StarSysObjects.GetChildren())
			{
				if (node is Planet planet)
				{
					if (planet.Controller == null)
					{
						// int amount = 2;//Rand.Next(10, 20);
						// var unitFileName = _data.GetNode<Unit>("Units/Unit 1").SceneFilePath;
						//var ship = CreateShip((Unit)((PackedScene)GD.Load(unitFileName)).Instantiate());
						// for(int i = 0;i<amount;i++){
						// 	var unit = ((PackedScene)GD.Load(unitFileName)).Instantiate();
						// 	var stat = unit.GetNode<BaseStat>("Stats/Attack");
						// 	ship.Units.AddChild(unit);
						// }
						// planet.AddToOrbit(ship);
						//var transform = ship.Transform;
					}
				}
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
		InitRand();
		InitPlayers();
		InitGalaxy();
		UpdateGround();
		InitPlayerStartResources();
		InitStartPlanets();
		InitStartResources();
		InitStartFleets();
		//InitResistance(); needs work
		InitWorldBuildings();
		ConnectPlanets();
		ConnectPlayers();
	}

	void _on_Alert(World.GameAlert alert)
	{
		switch (alert)
		{
			case GameAlert.NoResource:
				//_UI.ABox.Visible = true;
				break;
		}
	}

	void _on_ShipCommand(CmdPanel.CmdPanelOption option, Planet planet)
	{
		switch (option)
		{
			case CmdPanel.CmdPanelOption.MoveTo:
				WCC._SelectTarget(planet);
				break;
			case CmdPanel.CmdPanelOption.Conquer:
				WCC.SetTask(planet, option);
				// GD.Print("Conquer "+planet.Name);
				break;
		}
		_UI.CommandPanel.Visible = false;
	}


	void ConnectPlanets()
	{
		foreach (StarSystem system in Galaxy.StarSystems)
		{
			foreach (Planet planet in system.Planets)
			{
				planet.Connect("GameAlert", new Callable(this, nameof(_on_Alert)));
			}
		}
	}

	public override void _Ready()
	{
		GetNodes();
		ManualBattleScene.LoadUI(UInterface);
		_wcc.camera = Camera3D.GetNode<Camera3D>("InnerGimbal/Camera3D");
		_wcc.Connect("Deselect", new Callable(this, nameof(_on_Deselect)));
		Ground.ConnectToInputEvent(WCC.OnGroundInputCallable);
		GD.Print("World: " + GetInstanceId());
		ConnectSignals();
		MapArmyManager.ModelLoader = Data.ModelLoader;
		InitWorld();
		MapArmyManager.Rand = Rand;
		ManualBattleScene.InitializeBattle(Rand, Data.ModelLoader, _wcc);
		GD.Print("World: " + GetInstanceId());
		EndTurnEmitter.Instance.EndTurn += _on_EndTurn;
		if (_Player != null)
		{
			_wcc.LocalPlayerID = _Player.PlayerID;
			_UI.PInterface.LocalPlayerID = _Player.PlayerID;
			_UI.TopLeft._Player = _Player;
			_UI.TopLeft.WorldTechnology = Data.GetNode("Technology");
			_UI.ResourcePanel.UpdatePanel(_Player.ResManager);
			ConnectLocalPlayer(_Player);
			_UI.UpdateUI(_Player);
		}

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
			_UI.WorldMenu.Visible = !_UI.WorldMenu.Visible;
			GetTree().Paused = _UI.WorldMenu.Visible;
		}
    }

    public void _on_EndTurn(int TurnNumber)
    {
		_on_Deselect();
    }

}
