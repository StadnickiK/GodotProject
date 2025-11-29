using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public interface IGameScene
{
    public static IGameScene Instance { get; set; }

	public void InitializeScene(IWorld world);

	public void Select(ISelection planet);

	public void _on_Deselect();

	public void SplitShip(ShipModel shipStruct);

	public Node GetAsNode { get; }
}


public partial class CampaignScene : Node3D, IEndTurnListener, IGameScene
{
    [Export]
    PackedScene _GalaxyScene = (PackedScene)ResourceLoader.Load("res://Map/Galaxy.tscn");

    public Map Map { get; set; }

    public UI UI { get; set; } 

    public MapArmyManager MapArmyManager { get; set; }

    public ManualBattleScene ManualBattleScene { get; set; }

    public Random Rand { get; set; }

    WorldCursorControl WCC;

    Ground Ground;

    public Player Host { get; set; }

	public Node GetAsNode { get { return this; } }

    public override void _Ready()
    {
        Map = GetNode<Map>("Map");
        UI = GetNode<UI>("CanvasLayer/UI");
        MapArmyManager = GetNode<MapArmyManager>("MapArmyManager");
        ManualBattleScene = GetNode<ManualBattleScene>("ManualBattleScene");
        ManualBattleScene.LoadUI(UI);
        EndTurnEmitter.Instance.EndTurn += _on_EndTurn;
    }


    public void InitializeScene(IWorld world)
    {
        WCC = world.WCC;
        Ground = world.Ground;
        Host = world.Host;
        WCC.Connect("Deselect", new Callable(this, nameof(_on_Deselect)));
        ConnectSignals(world);
		MapArmyManager.ModelLoader = world.Data.ModelLoader;
        MapArmyManager.Rand = Rand = world.Rand;
        ManualBattleScene.InitializeScene(world);
		InitCampaign(world);
        if (Host != null)
		{
			WCC.LocalPlayerID = Host.PlayerID;
			UI.PInterface.LocalPlayerID = Host.PlayerID;
			UI.TopLeft._Player = Host;
			UI.TopLeft.WorldTechnology = world.Data.GetNode("Technology");
			UI.ResourcePanel.UpdatePanel(Host.ResManager);
			ConnectLocalPlayer(Host);
			UI.UpdateUI(Host);
		}
    }

    void _on_ShowBattlePanel(SpaceBattle battle)
	{
		UI.BattlePan.Show();
		UI.BattlePan.UpdatePanel(battle);
		UI.RightPanel.Hide();
		_on_Deselect();
	}

    void _on_Battle_Retreat(Ship ship)
	{
		ManualBattleScene.ClearCombat();
		if (ship.Controller.IsLocal)
			_on_SelectUnit(ship);
		UI.BattlePan.Hide();
		UI.RightPanel.Show();
	}

    void _on_EndBattle()
	{
		//Ground.ConnectToInputEvent(WCC.OnGroundInputCallable);
		Ground.Show();
		UI.BattlePan.Hide();
		UI.RightPanel.Show();
		ManualBattleScene.EndCombat();
	}

	void _on_ResetBattle()
	{
		//Ground.ConnectToInputEvent(WCC.OnGroundInputCallable);
		Map.Hide();
		ManualBattleScene.Show();
		UI.BattlePan.Hide();
		ManualBattleScene.ResetCombat();
	}

	void _on_ManualBattle()
	{
		//Ground.DisconnectInputEvent(WCC.OnGroundInputCallable);
		Ground.Hide();
		UI.UpdateBattleUI();
		Map.Hide();
		ManualBattleScene.UpdateBattle();
	}
	void _on_EndManualBattle()
	{
		//UI.UpdateBattleUI();
		Map.Show();
		ManualBattleScene.Hide();
	}

    public void Select(ISelection planet)
	{
		_on_Deselect();
		WCC._SelectUnit(planet);
		if (planet is IPlanetInterface planetInterface)
		{
			UI.PInterface.Show();
			UI.PInterface.UpdatePlanetInterface(planetInterface);
		}
	}

	void _on_CreateShip(Planet planet, Unit unit)
	{
		CreateShip(planet, unit);
	}

    void ConnectSignals(IWorld world)
	{
		//UI.RightPanel.ConnectToLookAt(this, nameof(_on_LookAtObject));
		UI.RightPanel.WorldCamera = world.Camera3D;
		UI.ResourcePanel.InitResourcePanel(world.Data.Resources);
		UI.PInterface._data = world.Data;
		UI.PInterface.InitBuildingsPanel();
		UI.PInterface.InitRecruitmentPanel();
		ManualBattleScene.OpenBattlePanel += _on_ShowBattlePanel;
		UI.BattlePan.Center.Retreat.ButtonUp += () => _on_Battle_Retreat(ManualBattleScene.GetLocalAttakcerOrNull());
		UI.BattlePan.Center.EndFight.ButtonUp += _on_EndBattle;
		UI.BattlePan.Center.RepeatFight.ButtonUp += _on_ResetBattle;
		UI.BattlePan.Center.Fight.ButtonUp += _on_ManualBattle;
		ManualBattleScene.CameraLookAt += world.Camera3D.LookAt;
		ManualBattleScene.EndBattle += _on_EndManualBattle;
		//_map.ConnectToShowBattlePanel(this, nameof(_on_ShowBattlePanel));
	}

    void _on_SelectUnit(ISelection body)
	{
		UI.PInterface.Hide();
		WCC._SelectUnit(body);
	}

	public void _on_Deselect()
	{
		UI.PInterface.Hide();
		WCC.ClearSelection();
	}

	void InitCampaign(IWorld world)
	{	
		Map.InitMap(world, this);
		InitStartPlanets(world);
		InitStartResources(world);
		InitStartFleets(world);
		//InitResistance(); needs work
		world.Ground.UpdateGround(Map.galaxy.Radius);
	}

    void InitStartPlanets(IWorld world)
	{
		var tempStarSystems = new List<StarSystem>(Map.galaxy.StarSystems);
		int count = tempStarSystems.Count;
		if (count > world.PlayerNumber)
		{
			foreach (var player in world.PlayersList)
			{
				var system = tempStarSystems[Rand.Next(0, count)];
				var planetList = system.Planets;
				var planet = planetList[Rand.Next(0, planetList.Count)];
				planet.ChangeController(player);
				planet.VisibilityConroller.UpdateVisibility(new VisibilityConroller.VisibilityStruct() { Visible = true, Visibility = VisibilityConroller.VisibilityState.Visible }, player.PlayerID);

			}
		}
		else
		{
			var usedPlanetList = new List<Planet>();
			foreach (var player in world.PlayersList)
			{
				var system = tempStarSystems[Rand.Next(0, count - 1)];
				var planetList = new List<Planet>(system.Planets);
				var planet = planetList[Rand.Next(0, planetList.Count)];
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

    void InitStartFleets(IWorld world)
	{
		foreach (var player in world.PlayersList)
		{
				int maxFleets = 1;
				foreach (CollisionObject3D body in player.MapObjects.ToArray())
				{ // ToArray is needed because MapObjects list is modified inside foreach loop which raises exception
					if (body is Planet planet && maxFleets > 0)
					{
						var ship = MapArmyManager.CreateShip(planet, planet.Transform.Origin + new Vector3(3, 0, 3), planet.Name + " " + 1);
						ConnectShip(ship);
						for (int i = 0; i < world.UnitCount; i++)
						{
							var unit = world.Data.GetUnit(0);
							ship.UnitController.AddUnit(unit);
							// ship.Power.CurrentValue += new Unit().Stats["HitPoints"].CurrentValue;
						}
						ship.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct() { Visibility = VisibilityConroller.VisibilityState.Unexplored, Visible = world.Host == player }, player.PlayerID, world.Host == player);
						if (world.Host == player)
						{
							ship.IsLocal = true;
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
		Map.ConnectToExitMapObject(ship);
		if (ship.Controller == Host)
		{
			ship.OpenTransferPanel -= UI.PInterface.UpdatePlanetInterface;
			ship.OpenTransferPanel += UI.PInterface.UpdatePlanetInterface;
		}
		ship.FreeShip -= FreeShip;
		ship.FreeShip += FreeShip;
	}

    void InitStartResources(IWorld world)
	{
		foreach (StarSystem system in Map.galaxy.StarSystems)
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
					foreach (Resource resource in world.Data.Resources)
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
					planet.InfoLabel.InitResources(planet, world.Data.Resources);
				}
			}
		}
	}

    public void ConnectLocalPlayer(Player player)
	{
		player.ArmiesChanged += UI.RightPanel.UpdateShips;
		player.PlanetsChanged += UI.RightPanel.UpdatePlanets;
		player.ResManager.ProdChanged += UI.ResourcePanel.UpdatePanel;
		player.ResManager.UpkeepChanged += UI.ResourcePanel.UpdatePanel;
		player.ResManager.ProdCostChanged += UI.ResourcePanel.UpdatePanel;
		player.ResManager.PlayerResourcesChanged += UI.ResourcePanel.UpdatePanel;
	}

	public void DisonnectLocalPlayer(Player player)
	{
		player.ArmiesChanged -= UI.RightPanel.UpdateShips;
		player.PlanetsChanged -= UI.RightPanel.UpdatePlanets;
		player.ResManager.ProdChanged -= UI.ResourcePanel.UpdatePanel;
		player.ResManager.UpkeepChanged -= UI.ResourcePanel.UpdatePanel;
		player.ResManager.ProdCostChanged -= UI.ResourcePanel.UpdatePanel;
		player.ResManager.PlayerResourcesChanged -= UI.ResourcePanel.UpdatePanel;
	}

	public void _on_EndTurn(int TurnNumber)
    {
		_on_Deselect();
    }
}
