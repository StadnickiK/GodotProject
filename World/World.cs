using Godot;
using System;
using System.Collections.Generic;

public class World : Spatial
{

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

    Galaxy Galaxy = null;
    CameraGimbal Camera = null;

    UI _UI = null;

    PackedScene _PlayerScene = (PackedScene)ResourceLoader.Load("res://World/Player.tscn");

    PackedScene _GalaxyScene = (PackedScene)ResourceLoader.Load("res://Map/Galaxy.tscn");

    int Seed = 0;
    public Random Rand { get; set; }

    void InitRand(){
        Seed = Guid.NewGuid().GetHashCode();
        Rand = new Random(Seed);
;   }

    Player _Player = null;

    Node Players = null;

    List<Player> PlayersList = new List<Player>();

    public Dictionary<string, int> WorldGenParameters = new Dictionary<string, int>();

    List<int> PlayerIDs = new List<int>();

    int PlayerNumber = 1;

    void _on_ShowBattlePanel(SpaceBattle battle){
        _UI.BattlePan.Visible = true;
    }

    public void ConnectTo_OpenPlanetInterface(Node node){
        node.Connect("OpenPlanetInterface",this,nameof(_on_OpenPlanetInterface));
    }

    void _on_OpenPlanetInterface(Planet planet){
        _UI.PInterface.Visible = true;
        _UI.PInterface.SetTitle(planet.PlanetName);
    }

    void _on_CameraLookAt(Vector3 position){
        Camera.LookAt(position);
    }

    void _on_LookAtObject(Node node){
        var planet = _Player.GetPlanetByName(node.Name);
        GD.Print(node.Name);
        if(planet is Planet p){
            p.System.OpenStarSystem();
            Camera.LookAt(p.Transform.origin);
        }
    }

    void GetNodes(){
        _map = GetNode<Map>("Map");
        Camera = GetNode<CameraGimbal>("UI/CameraGimbal");
        Players = GetNode("Players");
        _wcc = GetNode<WorldCursorControl>("WorldCursorControl");
        _UI = GetNode<UI>("UI");
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
                planet.PlanetOwner = player;
                player.MapObjects.Add(planet);
                tempStarSystems.Remove(system);
            }
        }else{
            var usedPlanetList = new List<Planet>();
            foreach(int id in PlayerIDs){
                var system = tempStarSystems[Rand.Next(0,count)];
                var planetList = new List<Planet>(system.Planets);
                var planet = planetList[Rand.Next(0,planetList.Count)];
                var player = (Player)Players.GetChild(id);
                while(usedPlanetList.Contains(planet)){
                    planet = system.Planets[Rand.Next(0,planetList.Count)];
                }
                planet.PlanetOwner = player;
                player.MapObjects.Add(planet);
                usedPlanetList.Add(planet);
            }
        }
    }

    void InitStartResources(){
        
    }

    public override void _Ready()
    {
        GetNodes();
        InitRand();
        InitPlayers();
        WCC.camera = Camera.GetNode<Camera>("InnerGimbal/Camera");
        if(_Player != null){
            WCC.LocalPlayerID = _Player.PlayerID;
        }
        InitGalaxy();
        UpdateGround();
        InitStartPlanets();
        _UI.RPanel.ConnectToLookAt(this, nameof(_on_LookAtObject));
        _map.ConnectToShowBattlePanel(this, nameof(_on_ShowBattlePanel));
    }
    public override void _Process(float delta)
    {
        _UI.UpdateUI(_Player);
        if(Input.IsActionJustReleased("ui_cancel")){
            _UI.WorldMenu.Visible = true;
        }
    }
}
