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

    private List<Building> _worldBuildings = new List<Building>();
    public List<Building> WorldBuildings
    {
        get { return _worldBuildings; }
    }
    
    private Dictionary<string, Resource> _wolrdResources = new Dictionary<string, Resource>();
    public Dictionary<string, Resource> WorldResources
    {
        get { return _wolrdResources; }
    }
    

    public Dictionary<string, int> WorldGenParameters = new Dictionary<string, int>();

    List<int> PlayerIDs = new List<int>();

    [Export]
    int PlayerNumber = 3;

    void _on_ShowBattlePanel(SpaceBattle battle){
        _UI.BattlePan.Visible = true;
    }

    public void ConnectTo_OpenPlanetInterface(Node node){
        node.Connect("OpenPlanetInterface",this,nameof(_on_OpenPlanetInterface));
    }

    void _on_OpenPlanetInterface(Planet planet){
        _UI.PInterface.Visible = true;
        _UI.PInterface.UpdatePlanetInterface(planet, WorldBuildings);
    }

    void _on_CameraLookAt(Vector3 position){
        Camera.LookAt(position);
    }

    void _on_LookAtObject(Node node){
        var obj = _Player.GetMapObjectByName(node.Name);
        if(obj is Planet p){
            GD.Print(node.Name);
            if(p.System != null){
                p.System.OpenStarSystem();
            }else{
                Galaxy.ViewGalaxy();
            }
            Camera.LookAt(p.GlobalTransform.origin);
        }else if(obj is Ship s){
            if(s.System != null){
                s.System.OpenStarSystem();
            }else{
                Galaxy.ViewGalaxy();
            }
            Camera.LookAt(s.GlobalTransform.origin);
        }
    }

    void _on_SelectObjectInOrbit(Planet planet, Node node){
        if(planet != null && node != null){
            var label = (Label)node;
            PhysicsBody obj = (PhysicsBody)node.GetMeta(label.Text); // 4 w/e reason node's name gets corrupted in overviewPanel connection method, but text is ok
            GD.Print(node.Name);
            if(obj != null){
                _wcc._SelectUnit(obj);
            }
        }
    }

    void GetNodes(){
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
        _map.ConnectToShowBattlePanel(this, nameof(_on_ShowBattlePanel));
    }

    public void ConnectToSelectUnit(Node node){
        node.Connect("SelectUnit", this, nameof(_on_SelectUnit));
    }

    void _on_SelectUnit(PhysicsBody body){
        WCC._SelectUnit(body);
        _UI.UInfo.Visible = true;
        _UI.UInfo.UpdatePanel(body);
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
                if(player == _Player){
                    planet.Vision = true;
                    GD.Print(planet.PlanetName);
                }else{
                    planet.Vision = false;
                }
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
                        ship.ShipOwner = player;
                        ship.ID_Owner = player.GetIndex();
                        ship.Name = planet.Name +" "+1;
                        ship.System = planet.System;
                        ship.System.AddMapObject(ship);
                        player.MapObjects.Add(ship);
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

    void InitStartResources(){
        
    }

    void InitWorld(){
        InitRand();
        InitPlayers();
        InitGalaxy();
        UpdateGround();
        InitStartPlanets();
        InitStartFleets();
        InitStartResources();
    }

    public override void _Ready()
    {
        GetNodes();
        WCC.camera = Camera.GetNode<Camera>("InnerGimbal/Camera");
        if(_Player != null){
            WCC.LocalPlayerID = _Player.PlayerID;
        }
        ConnectSignals();
        InitWorld();

        for(int i =0; i<5; i++){
            var building = new Building();
            building.Name = "Building "+i;
            WorldBuildings.Add(building);
        }
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
