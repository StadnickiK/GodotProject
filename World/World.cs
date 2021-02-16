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
        _UI.PInterface.UpdatePlanetInterface(planet, WorldBuildings);
    }

    void _on_CreateShip(Planet planet, Unit unit){
        CreateShip(planet, unit);
    }

    void _on_CameraLookAt(Vector3 position){
        Camera.LookAt(position);
    }

    void _on_LookAtObject(Node node){
        var obj = _Player.GetMapObjectByName(node.Name);
        if(obj is Planet planet){
            Galaxy.ViewGalaxy();
            if(planet.System != null){
                planet.System.OpenStarSystem();
            }
            Camera.LookAt(planet.GlobalTransform.origin);
        }else if(obj is Ship ship){
            Galaxy.ViewGalaxy();
            if(ship.System != null){
                ship.System.OpenStarSystem();
            }
            Camera.LookAt(ship.GlobalTransform.origin);
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
                if(obj is Ship){
                    _wcc._SelectUnit(obj);
                    _UI.UInfo.Visible = true;
                    _UI.UInfo.UpdatePanel(obj);
                }else{
                    if(obj is SpaceBattle battle){
                        _on_ShowBattlePanel(battle);
                    }
                }

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
                planet.ChangePlanetOwner(player);
                tempStarSystems.Remove(system);
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
                planet.ChangePlanetOwner(player);
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
                        ship.ShipOwner = player;
                        ship.ID_Owner = player.GetIndex();
                        ship.Name = planet.Name +" "+1;
                        ship.System = planet.System;
                        ship.MapObject = planet.System;
                        ship.System.AddMapObject(ship);
                        player.MapObjects.Add(ship);
                        for(int i = 0;i<5;i++){
                            ship.Units.Add(new Unit());
                            ship.Power.CurrentValue += new Unit().Stats["HitPoints"].CurrentValue;
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
        ship.System = planet.System;
        ship.Transform = transform;
        ship.ShipOwner = planet.PlanetOwner;
        ship.IsLocal = planet.Vision;
        ship.ID_Owner = ship.ShipOwner.PlayerID;
        ship.Name = planet.Name +" "+Rand.Next(0,1000);
        ship.Units.Add(unit);
        planet.System.AddMapObject(ship);
        //planet.AddToOrbit(ship);
        return ship;
    }

    void InitStartResources(){
        foreach(StarSystem system in _map.galaxy.StarSystems){
            foreach(Node node in system.StarSysObjects.GetChildren()){
                if(node is Planet planet){
                    if(planet.PlanetOwner != null){
                        var resource = new Resource("resource 0");
                        if(!(planet.PlayerResources.ContainsKey(resource.Name)))
                            planet.PlayerResources.Add(resource.Name, resource);
                        do{
                            resource = new Resource("resource "+ Rand.Next(1,5));
                        }while(planet.PlayerResources.ContainsKey(resource.Name));
                        planet.PlayerResources.Add(resource.Name, resource);
                    }else{
                        int amount = Rand.Next(1,5);
                        while(amount>0){
                            Resource resource = null;
                            do{
                                resource = new Resource("resource "+ Rand.Next(0,5));
                            }while(planet.PlayerResources.ContainsKey(resource.Name));
                            planet.PlayerResources.Add(resource.Name, resource);
                            amount--;
                        }
                    }
                }
            }
        }
    }

    void InitResistance(){
        foreach(StarSystem system in _map.galaxy.StarSystems){
            foreach(Node node in system.StarSysObjects.GetChildren()){
                if(node is Planet planet){
                    if(planet.PlanetOwner == null){
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
        for(int i = 1; i<6; i++){
            var building = new Building();
            building.Name = "Building "+i;
            
            Resource res = new Resource();
            res.Name = "resource "+i;
            building.BuildTime = 5+i;
            
            for(int j = 0; j<i;j++){
                res = new Resource();
                res.Name = "resource "+(j);
                res.Quantity = i * 100;
                building.BuildCost.Add(res);
            }

            res = new Resource();
            res.Quantity = (int)(25f/i);
            res.Name = "resource "+i;
            building.Products.Add(res);

            res = new Resource();
            res.Name = "resource "+(i-1);
            res.Quantity = (int)(10f/i);
            building.ProductCost.Add(res);

            building.ResourceLimit = 200;

            building.BuildTime = (i+1) * 5;
            WorldBuildings.Add(building);
        }

        for(int i = 0; i<5; i++){
            var building = new Building();
            building.Name = "Storage "+i;
            
            Resource res = new Resource();
            res.Name = "resource "+i;
            
            res = new Resource();
            res.Name = "resource 0";
            res.Quantity = 100+i * 100;
            building.BuildCost.Add(res);

            res = new Resource();
            res.Name = "resource "+i;
            building.Products.Add(res);

            building.ResourceLimit = 1000;

            building.BuildTime = (i+5) * 5;
            WorldBuildings.Add(building);
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
