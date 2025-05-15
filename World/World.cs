using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class World : Node3D
{


	public enum GameAlert
	{
		NoResource
	}

	public delegate void SplitShipEventHandler(ShipStruct shipStruct);

	public delegate void FreeShipEventHandler(Ship ship);

	public delegate void TransferUnitsEventHandler(Ship host, Ship target);

	private Map _map = null;
	public Map GetMap
	{
		get { return _map; }
	}

	private WorldCursorControl _wcc = null;
	public WorldCursorControl WCC
	{
		get { return _wcc; }
	}

	public MapArmyManager MapArmyManager { get; set; }

	private Data _data = null;

	Galaxy Galaxy = null;
	CameraGimbal Camera3D = null;

	private UI _UI = null;
	public UI UInterface
	{
		get { return _UI; }
	}
	
	PackedScene _PlayerScene = (PackedScene)ResourceLoader.Load("res://World/Player.tscn");

	PackedScene _GalaxyScene = (PackedScene)ResourceLoader.Load("res://Map/Galaxy.tscn");

	int Seed = 0;
	public Random Rand { get; set; }

	void InitRand(){
		Seed = -1991794247; //Guid.NewGuid().GetHashCode();
		Rand = new Random(Seed);
;   }

	private Player _Player = null;
	public Player Host
	{
		get { return _Player; }
	}

	Node Players = null;

	List<Player> PlayersList = new List<Player>();

	public Godot.Collections.Dictionary<string, int> WorldGenParameters = new Godot.Collections.Dictionary<string, int>();

	List<int> PlayerIDs = new List<int>();

	[Export]
	int PlayerNumber = 1;

	void _on_ShowBattlePanel(SpaceBattle battle){
		_UI.BattlePan.Visible = true;
		_UI.BattlePan.UpdatePanel(battle);
	}

	public void ConnectTo_OpenPlanetInterface(Node node){
		node.Connect("OpenPlanetInterface", new Callable(this, nameof(_on_OpenPlanetInterface)));
	}

	void _on_OpenPlanetInterface(Planet planet){
		_UI.PInterface.Visible = true;
		_UI.PInterface.UpdatePlanetInterface(planet);
	}

	void _on_CreateShip(Planet planet, Unit unit){
		CreateShip(planet, unit);
	}

	void _on_CameraLookAt(Vector3 position){
		Camera3D.LookAt(position);
	}

	void _on_LookAtObject(Node node){
		var obj = _Player.GetMapObjectByName(node.Name);
		Galaxy.ViewGalaxy();
		// if(obj.GetParent().GetParent() is StarSystem system){
		// 	system.OpenStarSystem();
		// }
		// if(obj.GetParent().GetParent() is Planet planet){
		// 	planet.System.OpenStarSystem();
		// }
		if(obj is Node3D spatial){
			Camera3D.LookAt(spatial.GlobalTransform.Origin);
		}
	}

	void _on_LookAtStarSystem(StarSystem system){
		if(system != null)
			Camera3D.LookAt(system.GlobalTransform.Origin);
		
	}

	void _on_SelectObjectInOrbit(Planet planet, Node node){
		if(planet != null && node != null){
			var label = (Label)node;
			PhysicsBody3D obj = (PhysicsBody3D)node.GetMeta(label.Text); // 4 w/e reason node's name gets corrupted in overviewPanel connection method, but text is ok
			if(obj != null){
				if(obj is ISelectMapObject selectMapObject){
					selectMapObject.SelectMapObject();
				}
			}
		}
	}

	void GetNodes(){
		_data = GetNode<Data>("Data");
		_map = GetNode<Map>("Map");
		Camera3D = GetNode<CameraGimbal>("UI/CameraGimbal");
		Players = GetNode("Players");
		_wcc = GetNode<WorldCursorControl>("WorldCursorControl");
		_UI = GetNode<UI>("UI");
		MapArmyManager = GetNode<MapArmyManager>("MapArmyManager");
	}

	void ConnectSignals(){
		//_UI.RPanel.ConnectToLookAt(this, nameof(_on_LookAtObject));
		_UI.RPanel.WorldCamera = Camera3D;
		_UI.PInterface.ConnectToSelectObjectInOrbit(this, nameof(_on_SelectObjectInOrbit));
		_UI.ResPanel.InitResourcePanel(_data.Resources);
		_UI.PInterface._data = _data;
		_UI.PInterface.InitBuildingsPanel();
		//_UI.UInfo.ConnectToChangeStance(_map, nameof(_map._on_UInfo_ChangeStance));
		_UI.OrbitList.Connect("SelectObject", new Callable(this, nameof(_on_SelectUnit)));
		_UI.CommandPanel.Connect("ShipCommand", new Callable(this, nameof(_on_ShipCommand)));
		_UI.ArmyInterfce._data = _data;
		_UI.ArmyInterfce.InitRecruitmentPanel();
		_UI.ArmyInterfce.Connect(ArmyInterface.SignalName.Deselect, new Callable(this, nameof(_on_Deselect)));
		_map.ConnectToShowBattlePanel(this, nameof(_on_ShowBattlePanel));
	}

	void ConnectLocalPlayer(Player player){
		player.ArmiesChanged += _UI.RPanel.UpdateRightPanel;
		player.ProdChanged += _UI.ResPanel.UpdatePanel;
		player.UpkeepChanged += _UI.ResPanel.UpdatePanel;
		player.ProdCostChanged += _UI.ResPanel.UpdatePanel;
		player.PlayerResourcesChanged += _UI.ResPanel.UpdatePanel;
	}

	public void ConnectToSelectUnit(Node node){
		node.Connect("SelectUnit", new Callable(this, nameof(_on_SelectUnit)));
	}

	void _on_SelectUnit(PhysicsBody3D body){
		_wcc._SelectUnit(body);
		
		_UI.ArmyInterfce.UpdateArmyPanel((Ship)body);
	}

	void _on_HideArmyInterface(){
		_UI.ArmyInterfce.Visible = false;
	}

	void _on_Deselect(){
		_UI.ArmyInterfce.DeselectArmy();
		WCC.ClearSelection();
	}


	void InitPlayers(){
		if(WorldGenParameters != null){
			if(WorldGenParameters.ContainsKey("Players")){
				PlayerNumber = WorldGenParameters["Players"];
			}
		}
		GD.Print("World 1: "+GetInstanceId());
		for(int i = 0; i<PlayerNumber;i++){
			// var player = new AIPlayer(_data);//(Player)_PlayerScene.Instance();
			var player = (Player)_PlayerScene.Instantiate();
			// player.SetMap(_map);
			Players.AddChild(player);
			player.PlayerID = player.GetIndex();
			if(i == 0){
				_Player = player;
				player.IsLocal = true;
			}
			PlayerIDs.Add(player.PlayerID);
			PlayersList.Add(player);
		}
	}

	void InitAIPlayers(){
		if(WorldGenParameters != null){
			if(WorldGenParameters.ContainsKey("Players")){
				PlayerNumber = WorldGenParameters["Players"];
			}
		}
		for(int i = 0; i<PlayerNumber;i++){
			var player = new AIPlayer(_data);  //(Player)_PlayerScene.Instance();
			player.SetMap(_map);
			Players.AddChild(player);
			player.PlayerID = player.GetIndex();
			if(i == 0){
				_Player = player;
				player.IsLocal = true;
			}
			PlayerIDs.Add(player.PlayerID);
			PlayersList.Add(player);
		}
	}

	void ConnectPlayers(){
		foreach(var player in PlayersList){
			if(player is AIPlayer ai){
				ai.ConnectSignals();
			}
		}
	}

	void InitGalaxy(){
		var generator = new Generator();
		generator.InitGenerator(this, Rand, WorldGenParameters);
		Galaxy = generator.GenerateGalaxy();
		generator.QueueFree();
		_map.AddChild(Galaxy);
		_map.galaxy = Galaxy;
		Galaxy.Connect("CameraLookAt", new Callable(this, nameof(_on_CameraLookAt)));
		Galaxy.Connect("LookAtStarSystem", new Callable(this, nameof(_on_LookAtStarSystem)));
	}

	void UpdateGround(){
		var ground = GetNode<Area3D>("Ground");
		ground.Scale = new Vector3(2*Galaxy.Radius,1,2*Galaxy.Radius);
	}

	void InitStartPlanets(){
		List<StarSystem> tempStarSystems = new List<StarSystem>(Galaxy.StarSystems);
		int count = tempStarSystems.Count;
		if(count > PlayerNumber){
			foreach(int id in PlayerIDs){
				var system = tempStarSystems[Rand.Next(0,count)];
				var planetList = system.Planets;
				var player = (Player)Players.GetChild(id);
				var planet = planetList[Rand.Next(0,planetList.Count)];
				planet.ChangeController(player);
				planet.VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct() { Visible = true, Visibility = VisibilityConroller.VisibilityState.Visible }, player.PlayerID);
				
			}
		}else{
			var usedPlanetList = new List<Planet>();
			foreach(int id in PlayerIDs){
				var system = tempStarSystems[Rand.Next(0,count-1)];
				var planetList = new List<Planet>(system.Planets);
				var planet = planetList[Rand.Next(0,planetList.Count)];
				var player = (Player)Players.GetChild(id);
				while(usedPlanetList.Contains(planet)){
					planet = system.Planets[Rand.Next(0,planetList.Count)];
				}
				planet.ChangeController(player);
				usedPlanetList.Add(planet);
				planet.VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct() { Visible = true, Visibility = VisibilityConroller.VisibilityState.Visible }, player.PlayerID);
			}
		}
	}

	void InitStartFleets(){
		foreach(Node node in Players.GetChildren()){
			if(node is Player player){
				int maxFleets = 1;	
				foreach(CollisionObject3D body in player.MapObjects.ToArray()){ // ToArray is needed because MapObjects list is modified inside foreach loop which raises exception
					if(body is Planet planet && maxFleets>0){
						var ship = MapArmyManager.CreateShip(planet, planet.Transform.Origin + new Vector3(3,0,3), planet.Name +" "+1);
						ConnectShip(ship);
						for(int i = 0;i<5;i++){
							var unit = _data.GetUnit(0);
							ship.Units.AddUnit(unit);
							// ship.Power.CurrentValue += new Unit().Stats["HitPoints"].CurrentValue;
						}
						if(_Player != player){
							//ship.Visible = false;
						}else{
							ship.IsLocal = true;
						}
					}
				}
			}
		}
	}

	void CreateShip(ShipStruct shipStruct){
		var s = MapArmyManager.CreateShip(shipStruct);
		ConnectShip(s);
	}

	public Ship CreateShip(Planet planet, Unit unit){
		var ship = MapArmyManager.CreateShip(planet, unit);
		ConnectShip(ship);
		//planet.Controller.AddMapObject(ship);
		//  planet.AddToOrbit(ship);
		return ship;
	}

	void SplitShip(ShipStruct shipStruct){
		var s = MapArmyManager.CreateShip(shipStruct);
		ConnectShip(s);
		_on_Deselect();
		_on_SelectUnit(s);
	}

	void FreeShip(Ship ship){
		MapArmyManager.FreeShip(ship);
	}

	void ConnectShip(Ship ship){
		ConnectToSelectUnit(ship);
        WCC.ConnectToSelectTarget(ship);
        _map.ConnectToEnterCombat(ship);
        //_map.ConnectToEnterMapObject(ship);
        _map.ConnectToExitMapObject(ship);
		if(ship.Controller == _Player){
			ship.Connect(nameof(Ship.OpenUnitTransferPanel), new Callable(_UI.UnitTransferP, "_on_OpenTransferPanel"));
			ship.OpenTransferPanel += OpenTransferPanel;
		}
		ship.SplitShip += SplitShip;
		ship.FreeShip += FreeShip;
	}

	void OpenTransferPanel(Ship host, Ship target){
		_UI.ArmyInterfce.UpdateArmyPanel(host, target);
	}

	void InitStartResources(){
		foreach(StarSystem system in _map.galaxy.StarSystems){
			foreach(Node node in system.StarSysObjects.GetChildren()){
				if(node is Planet planet){
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
					foreach(Resource resource in _data.Resources){
							if(resource.IsStarter == true && 
								planet.Controller != null && 
								(resource.ResourceType == Resource.Type.Ore))
							{
								 planet.ResourcesManager.Resources.Add(resource.Index, resource.Quantity);
								// GD.Print("1 ", rNode.Name);
								//planet.Resources.Add(resource.Index, resource.Quantity);
							}else if(resource.ResourceType == Resource.Type.Ore){
								// GD.Print("2 ", rNode.Name);
								if(Rand.Next(0,100)>(100 - resource.Rarity)){
									planet.ResourcesManager.Resources.Add(resource.Index, resource.Quantity);
									//planet.Resources.Add(resource.Index, resource.Quantity);
								}
									
							}
					}
					planet.InfoLabel.InitResources(planet, _data.Resources);
				}
			}
		}
	}

	void InitPlayerStartResources(){
		var resources = _data.GetResourcesIndexList();
		var starResources = _data.GetStartResources();

		foreach (var player in PlayersList)
		{
			// without new dictionary the game would use one for all of those instead of separate ones
			player.ResManager.Resources = new Dictionary<int, int>(starResources);
			player.ResManager.Production.Upkeep = new Dictionary<int, int>(resources);
			player.ResManager.ProdCost.Upkeep = new Dictionary<int, int>(resources);
			player.Upkeep.Upkeep = new Dictionary<int, int>(resources);
			player.TotalProduction.Upkeep = new Dictionary<int, int>(resources);
		}
	}

	void InitResistance(){
		foreach(StarSystem system in _map.galaxy.StarSystems){
			foreach(Node node in system.StarSysObjects.GetChildren()){
				if(node is Planet planet){
					if(planet.Controller == null){
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

	bool CheckBuildingResources(Planet planet, Building building){
		if(building.Type == Building.Category.Mine)
			foreach(var resName in building.Products.Keys){
				if(!planet.ResourcesManager.Resources.ContainsKey(resName)){
					return false;
				}
			}
		return true;
	}

	void InitAvaiableBuildings(Planet planet){ // todo: Separate construction and building list into separate nodes for better organization
		var construction = planet.BuildingsManager.CurrentConstruction();
		foreach(var building in _data.Buildings)
				if(building.Requirements.Count == 0)
					if(CheckBuildingResources(planet, building))
						if(planet.BuildingsManager.Buildings.Find(x => x.Name == building.Name) == null)
							planet.BuildingsManager.AvaiableBuildings.Add(building);
	}

	void InitWorldBuildings(){
		var startBuildings = new List<Building>();
		foreach(var building in _data.Buildings){
				if(building.IsStarter == true)
					startBuildings.Add(building);			
		}
		foreach(Player player in Players.GetChildren()){
			foreach(Planet planet in player.MapObjects.Where(x => x is Planet)){
				planet.BuildingsManager.AddBuildings(startBuildings);
				InitAvaiableBuildings(planet);
			}
			player.InitResourceLimit();
		}
	}	

	void ChangePlayer(Player player){
	   	
	}

	void InitWorld(){
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

	void _on_Alert(World.GameAlert alert){
		switch(alert){
			case GameAlert.NoResource:
				//_UI.ABox.Visible = true;
				break;
		}
	}

	void _on_OpenPlanetCmdPanel(Planet planet){
        if(_wcc.HasSelected()){
			_UI.CommandPanel.ShowPanel(planet);
		}
    }

	void _on_ShipCommand(CmdPanel.CmdPanelOption option, Planet planet){
		switch(option){
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


	void ConnectPlanets(){
		foreach(StarSystem system in Galaxy.StarSystems){
			foreach(Planet planet in system.Planets){
				planet.Connect("GameAlert", new Callable(this, nameof(_on_Alert)));
			}
		}
	}

	public override void _Ready()
	{
		GetNodes();
		_wcc.camera = Camera3D.GetNode<Camera3D>("InnerGimbal/Camera3D");
		_wcc.Connect("Deselect", new Callable(this, nameof(_on_HideArmyInterface)));
		GD.Print("World: "+GetInstanceId());
		ConnectSignals();
		InitWorld();
		MapArmyManager.Rand = Rand;
		GD.Print("World: "+GetInstanceId());
		if(_Player != null){
			_wcc.LocalPlayerID = _Player.PlayerID;
			_UI.PInterface.LocalPlayerID = _Player.PlayerID;
			_UI.TopLeft._Player = _Player;
			_UI.TopLeft.WorldTechnology = _data.GetNode("Technology");
			_UI.ResPanel.UpdatePanel(_Player);
			ConnectLocalPlayer(_Player);

		}

	}
	public override void _Process(double delta)
	{
		if(_Player != null){
			_UI.UpdateUI(_Player);
		}
		if(Input.IsActionJustReleased("ui_cancel")){
			_UI.WorldMenu.Visible = true;
		}
	}
}
