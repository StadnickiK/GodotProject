using Godot;
using System;
using System.Collections.Generic;

public class Planet : StaticBody, IEnterMapObject, IExitMapObject, IMapObjectControllerChanger, IVisible, IResourceManager
{

    [Export]
    int Size = 10;

    [Export]
    int Wealth = 5;

    [Export]
    Gradient gradient = null;

    [Export]
    public int TimeStep { get; set; } = 1;

    public bool Vision { get; set; } = false;

    PackedScene TileScene = null;

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    [Signal]
    public delegate void OpenCmdPanel(Planet planet);

    [Signal]
    public delegate void CreateShip(Planet planet, Unit unit);
   
    public Player Controller { get; set; } = null;

    public List<Tile> Tiles { get; set; } = new List<Tile>();

    public Random Rand { get; set; } = new Random();

    public string PlanetName { get; set; } = "PlanetName";

    public Text3 MapObjectName3 { get; set; } = null;

    public Icon3D IcoOrbit { get; set; } = null;

    public StarSystem System { get; set; } = null;

    private Orbit _orbit = null;
    public Orbit Orbit
    {
        get { return _orbit; }
    }

    public Status PlanetStatus { get; set; } = Status.None;

    public enum Status
    {
        None,
        Blockade,
        Siege,
        Occupied
    }

    private Unit _unit;
    public Unit CurrentUnit
    {
        get { return _unit; }
    }
    

    public BuildingManager BuildingsManager { get; } = new BuildingManager();

    public bool BuildingsChanged { get; set; } = false;

    private MeshInstance _mesh = null;
    public MeshInstance Mesh
    {
        get { return _mesh; }
    }

    float _time = 0;
    public ResourceManager ResourcesManager { get; set; } = new ResourceManager();

    [Signal]
    public delegate void OpenPlanetInterface(Planet planet);

    [Signal]
    public delegate void GameAlert(World.GameAlert alert);

    public enum Type
    {
        Balanced,
        Aggro,
        MegaCity,
        Colony,
        Mine,
        Industrial,
        HeavyIndustrial,
        HellIndustrial
    }

    void Generate(){
        Name = PlanetName;
        Size = Rand.Next(5,15);
        Scale *= Size/10;
        Size = Size%2==0 ? Size : ++Size;
        for(int i = 0; i<Size;i++){
            Tile t =  (Tile)TileScene.Instance();
            t.Rand = Rand;
            Tiles.Add(t);
        }
    }

    void GenerateMesh(){
        ShaderMaterial material = (ShaderMaterial)Mesh.GetSurfaceMaterial(0);
        material.ResourceLocalToScene = true;
        NoiseTexture noise = new NoiseTexture();
        noise = (NoiseTexture)material.GetShaderParam("noise");//new OpenSimplexNoise();
        var tempGradient = (GradientTexture)material.GetShaderParam("gradient");
        if(gradient != null){
            tempGradient.Gradient = gradient;
        }else{
            gradient = new Gradient();
            for(float i = 1; i<4;i++){
                gradient.AddPoint(i*0.3f, new Color( (float)Rand.NextDouble(), (float)Rand.NextDouble(), (float)Rand.NextDouble()));
            }
            gradient.RemovePoint(0);
            tempGradient.Gradient = gradient;
        }
        noise.Noise.Seed = Rand.Next(-1000,1000);
        material.SetShaderParam("noise", noise);
        material.SetShaderParam("gradient", tempGradient);
        Mesh.SetSurfaceMaterial(0, material);
    }

    void _on_Planet_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(inputEvent is InputEventMouseButton eventMouseButton){
            switch((ButtonList)eventMouseButton.ButtonIndex){
            case ButtonList.Left:
                EmitSignal(nameof(OpenPlanetInterface), this);
                break;
            case ButtonList.Right:
                // EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
                EmitSignal(nameof(OpenCmdPanel), this);
                break;
        }
      } 
    }

    public void EnterMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state){
        if(node is Ship ship)
            if(GetParent() == ship.GetParent())
                if(!Orbit.GetChildren().Contains(ship) && ship.MapObject != this){
                    AddToOrbit(ship);
                    ship.PlanetPos = (Transform.origin - ship.GlobalTransform.origin);
                    var transform = state.Transform;
                    transform.origin = GlobalTransform.origin;
                    state.Transform = transform;
                    CheckOrbit(ship);
                    // if(!ship.IsConnected("LeavePlanet", this, nameof(_on_Ship_LeavePlanet))){
                    //     ship.ConnectToLeavePlanet(this, nameof(_on_Ship_LeavePlanet));
                    // }
                    ship.MapObject = this;
                    ship.targetManager.ClearTargets();
                }
    }

    public void ExitMapObject(Node node, Vector3 exitVec, PhysicsDirectBodyState state){
        var transform = Transform;
        transform.origin = GlobalTransform.origin;
        if(node != null)
            if(node is Ship ship){
                if(ship.MapObject == this){
                    if(((Transform.origin - ship.GlobalTransform.origin) - ship.PlanetPos).Length()>2){
                        ship.MapObject = null; 
                        RemoveFromOrbit(ship);
                        ship.Visible = true; 
                        ship.targetManager.ClearTargets();
                    }
                }

            }
        state.Transform = transform;
    }

    void GetNodes(){
        _mesh = GetNode<MeshInstance>("MeshInstance");
        _orbit = GetNode<Orbit>("Orbit");
        MapObjectName3 = GetNode<Text3>("Text3");
        IcoOrbit = GetNode<Icon3D>("IcoOrbit");
    }

    public void StartConstruction(Unit unit){
        if(Controller.ResManager.PayCost(unit.BuildCost)){
            _unit = new Unit(unit);
        }else{
            EmitSignal(nameof(GameAlert), this);
        }
        return;
    }

    public void StartConstruction(IBuilding building){
        if(Controller.ResManager.PayCost(building.BuildCost)){
            if(building is Unit unit){
                _unit = new Unit(unit);
            }else{
                if(building is Building b)
                BuildingsManager.ConstructBuilding(b);
            }
        }else{
            EmitSignal(nameof(GameAlert), this);
        }
        return;
    }

    public void ConstructUnit(Unit unit){
        if(Controller.ResManager.PayCost(unit.BuildCost)){
            var ship = GetLocalShip();
            if(ship != null){
                ship.Units.Add(unit);
            }else{
                EmitSignal(nameof(CreateShip), this, unit);
                // unit.CurrentTime = 0;
            }
        }else{
            return;
        }
    }

    public void CheckOrbit(Node node){
        // to do change it to multiple enemies
        if(node is Ship ship){
            foreach(Node orbitNode in Orbit.GetChildren()){
                if(ship != orbitNode && orbitNode is Ship orbitShip){
                    if(ship.Controller != orbitShip.Controller){
                        ship.EmitSignal("EnterCombat", ship, orbitShip, Orbit);
                        if(ship.IsLocal)
                            Vision = ship.IsLocal;
                    }else{
                        //ChangeController(ship.ShipOwner);
                    }
                }else{
                    //ChangeController(ship.ShipOwner);
                }
            }
        }
    }

    public void ChangeVision(){
        var orbit = Orbit.GetChildren();
        if(Vision){
            Vision = false;
        }else{
            Vision = true;
        }
    }

    public new bool IsVisible(){
        return Vision;
    }

    public void AddToOrbit(Node ship){
        if(ship.GetParent() != null){
            ship.GetParent().RemoveChild(ship);
        }
        Orbit.AddNode(ship);
        Orbit.OrbitChanged = true;
        if(IsVisible())
            IcoOrbit.Visible =  true;
            if(Orbit.GetChildren().Count == 1)
                IcoOrbit.SetGreen();
    }

    public void RemoveFromOrbit(Ship ship){
        if(ship != null){
            if(Orbit.GetChildren().Contains(ship)){
                Orbit.RemoveChild(ship);
                if(GetParent().GetParent() is StarSystem system){
                    system.AddMapObject(ship);
                    ship.MapObject = system;
                }else{
                    GetParent().AddChild(ship);
                }
                Orbit.OrbitChanged = true;
            }
            if(Orbit.GetChildren().Count <= 0)
                IcoOrbit.Visible = false;
        }
    }

    public Ship GetLocalShip(){
        return Orbit.GetLocal();
    }

    public void Siege(Node node){
        if(node is Ship ship){
            
        }
    }

    public void _on_Planet_TakeOver(Node node){
        if(node is Ship ship){
            ChangeController(ship.Controller);
        }
        if(node is Player player){
            ChangeController(player);
        }
    }

    public void ChangeController(Player player){
            if(player != Controller){
                if(Controller != null){
                    Controller.RemoveMapObject(this);
                }
                Controller = player;
                if(player != null){
                    player.AddMapObject(this);
                }
            }
    }

    public override void _Ready()
    {
        TileScene = (PackedScene)GD.Load("res://Map/Tile.tscn");
        GetNodes();
        GenerateMesh();
        World w = GetNode<World>("/root/Game/World");
        w.ConnectTo_OpenPlanetInterface(this);
        Connect(nameof(CreateShip), w, "_on_CreateShip");
        var arr = new Godot.Collections.Array();
        arr.Add(Orbit);
        IcoOrbit.Connect("mouse_entered", w.UInterface,"_on_OrbitIconFocus", arr);
        var arr2 = new Godot.Collections.Array();
        arr2.Add(this);
        Connect(nameof(OpenCmdPanel), w, "_on_OpenPlanetCmdPanel");
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
        WCC.ConnectToSelectTarget(this);    
        Name = PlanetName;
        MapObjectName3.UpdateText(Name);

        AddChild(BuildingsManager);
        AddChild(ResourcesManager);
        //Generate();
        // for(int i = 0; i<1; i++){
        //     var building = new Building();
        //     building.Name = "Building "+i;
        //     var resource = new Resource();
        //     resource.Name = "resource "+i;
        //     resource.Quantity = 20;
        //     building.Products.Add(resource.Name, resource.Quantity);
        //     building.ResourceLimit = 500;
        //     BuildingsManager.Buildings.Add(building);
        //     ResourcesManager.Resources.Add(resource.Name, resource);
        // }
        // foreach(Building building in BuildingsManager.Buildings)
        //     ResourcesManager.UpdateResourceLimit(building);
    }

    public override void _Process(float delta){
        _time += delta;
        // if(_time >= TimeStep){
        //     ResourcesManager.UpdateResources(BuildingsManager.Buildings);
        //     _time = 0;
        // }
        if(Orbit.OrbitChanged){
            if(_orbit.HasLocal()){
                Vision = true;
            }
        }
        if(BuildingsManager.BuildingsChanged){
            // ResourcesManager.UpdateResourceLimit(BuildingsManager.GetLastBuilding());
            BuildingsManager.BuildingsChanged = false;
            BuildingsChanged = true;
            // Controller.UpdateResourceLimit(this);
        }
        if(_unit != null){
            if(_time > TimeStep){
                _time = 0;
                if(_unit.CurrentTime < _unit.BuildTime){
                    _unit.CurrentTime++;
                }else{
                    var ship = GetLocalShip();
                    if(ship != null){
                        ship.Units.Add(_unit);
                    }else{
                        EmitSignal(nameof(CreateShip), this, _unit);
                    }
                    _unit = null;
                }
            }
        }
    }
}
