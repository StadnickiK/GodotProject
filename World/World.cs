using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class World : Spatial
{


	public enum GameAlert
	{
		NoResource
	}

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

private Data _data = null;

	Galaxy Galaxy = null;
	CameraGimbal Camera = null;

	private UI _UI = null;
	public UI UInterface
	{
		get { return _UI; }
	}
	
	PackedScene _PlayerScene = (PackedScene)ResourceLoader.Load("res://World/Player.tscn");

	PackedScene _GalaxyScene = (PackedScene)ResourceLoader.Load("res://Map/Galaxy.tscn");

	PackedScene _ShipScene = (PackedScene)ResourceLoader.Load("res://Units/Base/Ship.tscn");

	int Seed = 0;
	public Random Rand { get; set; }

	void InitRand(){
		Seed = Guid.NewGuid().GetHashCode();
		Rand = new Random(Seed);
;   }

	private Player _Player = null;
	public Player Host
	{
		get { return _Player; }
	}

	Node Players = null;

	List<Player> PlayersList = new List<Player>();

	public Dictionary<string, int> WorldGenParameters = new Dictionary<string, int>();

	List<int> PlayerIDs = new List<int>();

	[Export]
	int PlayerNumber = 1;

	void _on_ShowBattlePanel(SpaceBattle battle){
		_UI.BattlePan.Visible = true;
		_UI.BattlePan.UpdatePanel(battle);
	}

	public void ConnectTo_OpenPlanetInterface(Node node){
		node.Connect("OpenPlanetInterface",this,nameof(_on_OpenPlanetInterface));
	}

	void _on_OpenPlanetInterface(Planet planet){
		_UI.PInterface.Visible = true;
		_UI.PInterface.UpdatePlanetInterface(planet, _data.WorldBuildings, _data.WorldUnits);
	}

	void _on_CreateShip(Planet planet, Unit unit){
		CreateShip(planet, unit);
	}

	void _on_CameraLookAt(Vector3 position){
		Camera.LookAt(position);
	}

	void _on_LookAtObject(Node node){
		var obj = _Player.GetMapObjectByName(node.Name);
		Galaxy.ViewGalaxy();
		if(obj.GetParent().GetParent() is StarSystem system){
			system.OpenStarSystem();
		}
		if(obj is Spatial spatial){
			Camera.LookAt(spatial.GlobalTransform.origin);
		}
	}

	void _on_LookAtStarSystem(StarSystem system){
		if(system != null)
			Camera.LookAt(system.GlobalTransform.origin);
		
	}

	void _on_SelectObjectInOrbit(Planet planet, Node node){
		if(planet != null && node != null){
			var label = (Label)node;
			PhysicsBody obj = (PhysicsBody)node.GetMeta(label.Text); // 4 w/e reason node's name gets corrupted in overviewPanel connection method, but text is ok
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
		Camera = GetNode<CameraGimbal>("UI/CameraGimbal");
		Players = GetNode("Players");
		_wcc = GetNode<WorldCursorControl>("WorldCursorControl");
		_UI = GetNode<UI>("UI");
	}

	void ConnectSignals(){
		_UI.RPanel.ConnectToLookAt(this, nameof(_on_LookAtObject));
		_UI.PInterface.ConnectToSelectObjectInOrbit(this, nameof(_on_SelectObjectInOrbit));
		_UI.UInfo.ConnectToChangeStance(_map, nameof(_map._on_UInfo_ChangeStance));
		_UI.OrbitList.Connect("SelectObject", this, nameof(_on_SelectUnit));
		_UI.CommandPanel.Connect("ShipCommand", this, nameof(_on_ShipCommand));
		_map.ConnectToShowBattlePanel(this, nameof(_on_ShowBattlePanel));
	}

	public void ConnectToSelectUnit(Node node){
		node.Connect("SelectUnit", this, nameof(_on_SelectUnit));
	}

	void _on_SelectUnit(PhysicsBody body){
		_wcc._SelectUnit(body);
		_UI.UInfo.Visible = true;
		_UI.UInfo.UpdatePanel(body);
	}

	void _on_Deselect(){
		_UI.UInfo.Visible = false;
	}

	void InitPlayers(){
		if(WorldGenParameters != null){
			if(WorldGenParameters.ContainsKey("Players")){
				PlayerNumber = WorldGenParameters["Players"];
			}
		}
		for(int i = 0; i<PlayerNumber;i++){
			var player = (Player)_PlayerScene.Instance();
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

	void InitGalaxy(){
		Galaxy = (Galaxy)_GalaxyScene.Instance();
		if(WorldGenParameters != null){
			if(WorldGenParameters.ContainsKey("Systems")){
				Galaxy.StarSystemNumber = WorldGenParameters["Systems"];
			}
		}
		Galaxy.Rand = Rand;
		_map.AddChild(Galaxy);
		_map.galaxy = Galaxy;
		Galaxy.Connect("CameraLookAt",this, nameof(_on_CameraLookAt));
		Galaxy.Connect("LookAtStarSystem", this, nameof(_on_LookAtStarSystem));
	}

	void UpdateGround(){
		var ground = GetNode<StaticBody>("Ground");
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
				if(player == _Player){
					planet.Vision = true;
				}else{
					planet.Vision = false;
				}
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
				if(player == _Player){
					planet.Vision = true;
				}else{
					planet.Vision = false;
				}
			}
		}
	}

	void InitStartFleets(){
		foreach(Node node in Players.GetChildren()){
			if(node is Player player){
				int maxFleets = 1;
				var ship = (Ship)_ShipScene.Instance();
				foreach(PhysicsBody body in player.MapObjects.ToArray()){ // ToArray is needed because MapObjects list is modified inside foreach loop which raises exception
					if(body is Planet planet && maxFleets>0){
						var transform = ship.Transform;
						transform.origin = planet.Transform.origin;
						transform.origin += new Vector3(3,0,3);
						ship.Transform = transform;
						ship.Controller = player;
						ship.ID_Owner = player.GetIndex();
						ship.Name = planet.Name +" "+1;
						ship.MapObject = planet.System;
						planet.System.AddMapObject(ship);
						player.MapObjects.Add(ship);
						for(int i = 0;i<5;i++){
							var unit = new Unit();
							ship.Units.Add(unit);
							// ship.Power.CurrentValue += new Unit().Stats["HitPoints"].CurrentValue;
							// ship.ResourcesManager.TotalResourceLimit += unit.Stats["Storage"].BaseValue;
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

	public Ship CreateShip(Unit unit){
		var ship = (Ship)_ShipScene.Instance();
		ship.Units.Add(unit);
		return ship;
	}

	public Ship CreateShip(Planet planet, Unit unit){
		var ship = (Ship)_ShipScene.Instance();
		var transform = ship.Transform;
		transform.origin = planet.Transform.origin;
		transform.origin += new Vector3(3,0,3);
		ship.MapObject = planet;
		ship.Transform = transform;
		ship.Controller = planet.Controller;
		ship.IsLocal = planet.Vision;
		ship.ID_Owner = ship.Controller.PlayerID;
		ship.Name = planet.Name +" "+Rand.Next(0,1000);
		ship.Units.Add(unit);
		planet.System.AddMapObject(ship);
		planet.Controller.AddMapObject(ship);
		//planet.AddToOrbit(ship);
		return ship;
	}

	void InitStartResources(){
		foreach(StarSystem system in _map.galaxy.StarSystems){
			foreach(Node node in system.StarSysObjects.GetChildren()){
				if(node is Planet planet){
					foreach(var resource in _data.WorldResources.Values){
						if(resource.IsStarter == true && 
						planet.Controller != null && 
						(resource.ResourceType == Resource.Type.Ore)){
							planet.ResourcesManager.Resources.Add(resource.Name, resource.Quantity);
						}else if((resource.ResourceType == Resource.Type.Ore)){
							if(Rand.Next(0,100)>(100 - resource.Rarity))
								planet.ResourcesManager.Resources.Add(resource.Name, resource.Quantity);
						}
						// if(resource is Godot.Collections.Dictionary)
							// GD.Print(resource);
					
					// if(planet.Controller != null){
						// var resource = new Resource("resource 0");
						// if(!(planet.ResourcesManager.Resources.ContainsKey(resource.Name)))
						// 	planet.ResourcesManager.Resources.Add(resource.Name, resource);
						// do{
						// 	resource = new Resource("resource "+ Rand.Next(1,5));
						// }while(planet.ResourcesManager.Resources.ContainsKey(resource.Name));
						// planet.ResourcesManager.Resources.Add(resource.Name, resource);
					// }else{
						// int amount = Rand.Next(1,5);
						// while(amount>0){
						// 	Resource resource = null;
						// 	do{
						// 		resource = new Resource("resource "+ Rand.Next(0,5));
						// 	}while(planet.ResourcesManager.Resources.ContainsKey(resource.Name));
						// 	planet.ResourcesManager.Resources.Add(resource.Name, resource);
						// 	amount--;
						// }
					// }
					}
				}
			}
		}
	}

	void InitResistance(){
		foreach(StarSystem system in _map.galaxy.StarSystems){
			foreach(Node node in system.StarSysObjects.GetChildren()){
				if(node is Planet planet){
					if(planet.Controller == null){
						int amount = Rand.Next(10, 50);
						var unit = new Unit();
						var ship = CreateShip(unit);
						for(int i = 0; i < amount; i++){
							ship.Units.Add(new Unit());
						}
						planet.AddToOrbit(ship);
						//var transform = ship.Transform;
					}
				}
			}
		}
	}

	void InitWorldBuildings(){
		var startBuildings = new List<Building>();
		foreach(Building building in _data.WorldBuildings.Where(x => x.IsStarter == true)){
			startBuildings.Add(building);			
		}
		foreach(Player player in Players.GetChildren()){
			foreach(Planet planet in player.MapObjects.Where(x => x is Planet)){
				planet.BuildingsManager.Buildings.AddRange(startBuildings);
			}
			player.InitResourceLimit();
		}
	}	

	void InitWorld(){
		InitRand();
		InitPlayers();
		InitGalaxy();
		UpdateGround();
		InitStartPlanets();
		InitStartFleets();
		InitStartResources();
		InitResistance();
		InitWorldBuildings();
		ConnectPlanets();
	}

	void _on_Alert(World.GameAlert alert){
		switch(alert){
			case GameAlert.NoResource:
				_UI.ABox.Visible = true;
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
				GD.Print("Conquer "+planet.Name);
				break;
		}
		_UI.CommandPanel.Visible = false;
	}


	void ConnectPlanets(){
		foreach(StarSystem system in Galaxy.StarSystems){
			foreach(Planet planet in system.Planets){
				planet.Connect("GameAlert", this, nameof(_on_Alert));
			}
		}
	}

	public override void _Ready()
	{
		GetNodes();
		_wcc.camera = Camera.GetNode<Camera>("InnerGimbal/Camera");
		if(_Player != null){
			_wcc.LocalPlayerID = _Player.PlayerID;
		}
		_wcc.Connect("Deselect", this, nameof(_on_Deselect));
		ConnectSignals();
		InitWorld();
		if(_Player != null)
			_UI.PInterface.LocalPlayerID = _Player.PlayerID;

	}
	public override void _Process(float delta)
	{
		if(_Player != null){
			_UI.UpdateUI(_Player);
		}
		if(Input.IsActionJustReleased("ui_cancel")){
			_UI.WorldMenu.Visible = true;
		}
	}
}
