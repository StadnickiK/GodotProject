using Godot;
using System;
using System.Collections.Generic;

public class Planet : StaticBody, IEnterMapObject, IExitMapObject, IMapObjectController
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

    public bool LoacalInOrbit { get; set; } = false;

    PackedScene TileScene = null;

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    [Signal]
    public delegate void CreateShip(Planet planet, Unit unit);
   
    public Player Controller { get; set; } = null;

    public List<Tile> Tiles { get; set; } = new List<Tile>();

    public Random Rand { get; set; } = new Random();

    public string PlanetName { get; set; } = "PlanetName";

    public Text3 MapObjectName3 { get; set; } = null;

    public StarSystem System { get; set; } = null;

    private Orbit _orbit = null;
    public Orbit Orbit
    {
        get { return _orbit; }
    }

    private Timer _timer = null;
    public Timer PlanetTimer
    {
        get { return _timer; }
    }

    public Status PlanetStatus { get; set; } = Status.None;

    public enum Status
    {
        None,
        Blockade,
        Siege,
        Occupied
    }

    //public TargetManager<Building> Construction { get; set; } = new TargetManager<Building>(); TO DO: create construction list, multiple simultanous constructions

    public Building Construction { get; set; } = null;

    private MeshInstance _mesh = null;
    public MeshInstance Mesh
    {
        get { return _mesh; }
    }
    

    float _time = 0;

    private Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
    public Dictionary<string, Resource> PlayerResources
    {
        get { return _resources; }
    }

    private Dictionary<string, int> _resourceLimits = new Dictionary<string, int>();
    public Dictionary<string, int> ResourceLimits
    {
        get { return _resourceLimits; }
    }

    public bool ResourceLimitChanged { get; set; } = false;

    public bool ResourcesChanged { get; set; } = false;

    private Dictionary<string, Resource> _naturalResources = new Dictionary<string, Resource>();
    public Dictionary<string, Resource> NaturalResources
    {
        get { return _naturalResources; }
    }

    private List<Building> _buildings = new List<Building>();
    public List<Building> Buildings
    {
        get { return _buildings; }
    }

    public bool BuildingsChanged { get; set; } = false;

    [Signal]
    public delegate void OpenPlanetInterface(Planet planet);

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
                EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
                break;
        }
      } 
    }

    void _on_Timer_timeout(){
        if(Construction != null){
            Construction.CurrentTime++;
            BuildingsChanged = true;
            if(Construction.CurrentTime >= Construction.BuildTime){
                _buildings.Add(Construction);
                UpdateResourceLimit(Construction);
                Construction = null;
            }else{
                _timer.Start(1);
            }
        }
    }

    public void EnterMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state){
        if(node is Ship ship)
            if(GetParent() == ship.GetParent())
                if(!Orbit.GetChildren().Contains(ship) && ship._Planet != this){
                    AddToOrbit(ship);
                    ship._Planet = this;
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
                        ship._Planet = null;
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
        _timer = GetNode<Timer>("Timer");
        MapObjectName3 = GetNode<Text3>("Text3");
    }

    public void ConstructBuilding(Building building){
        if(Construction == null){
            if(Controller.PayCost(building.BuildCost)){
                Construction = building;
                _timer.Start(1);
            }else{
                return;
            }
        }else{
            _timer.Start(1);
        }
    }

    void UpdateResourceLimit(){  
        foreach(Building building in Buildings){
            if(building.ResourceLimit >0 && building.ResourceLimit != default(int))
                foreach(Resource resource in building.Products){
                    if(ResourceLimits.ContainsKey(resource.Name)){
                        ResourceLimits[resource.Name] += building.ResourceLimit;
                        ResourceLimitChanged = true;   
                    }else{
                        ResourceLimits.Add(resource.Name, building.ResourceLimit);
                        ResourceLimitChanged = true;   
                    }
                }
        }
    }

    void UpdateResourceLimit(Building building){
        if(building.ResourceLimit >0 && building.ResourceLimit != default(int))
            foreach(Resource resource in building.Products){
                if(ResourceLimits.ContainsKey(resource.Name)){
                    ResourceLimits[resource.Name] += building.ResourceLimit;
                    ResourceLimitChanged = true;   
                }else{
                    ResourceLimits.Add(resource.Name, building.ResourceLimit);
                    ResourceLimitChanged = true;   
                }
            }
    }

    public void ConstructUnit(Unit unit){
        if(Controller.PayCost(unit.BuildCost)){
            var ship = GetLocalShip();
            if(ship != null){
                ship.Units.Add(unit);
            }else{
                EmitSignal(nameof(CreateShip), this, unit);
            }
        }else{
            return;
        }
    }

    bool PayCost(List<Resource> BuildCost){
        foreach(Resource resource in BuildCost){
            if(PlayerResources.ContainsKey(resource.Name)){
                if(PlayerResources[resource.Name].Value < resource.Quantity){
                    return false;
                }
            }else{
                return false;
            }
        }
        foreach(Resource resource in BuildCost){
            PlayerResources[resource.Name].Value -= resource.Quantity;
        }
        return true;
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

    public void AddToOrbit(Node ship){
        if(ship.GetParent() != null){
            ship.GetParent().RemoveChild(ship);
        }
        Orbit.AddNode(ship);
        Orbit.OrbitChanged = true;
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
        
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
        WCC.ConnectToSelectTarget(this);    
        Name = PlanetName;
        MapObjectName3.UpdateText(Name);
        //Generate();
        for(int i = 0; i<1; i++){
            var building = new Building();
            building.Name = "Building "+i;
            var resource = new Resource();
            resource.Name = "resource "+i;
            resource.Quantity = 20;
            building.Products.Add(resource);
            building.ResourceLimit = 500;
            Buildings.Add(building);
            _resources.Add(resource.Name, resource);
        }
        UpdateResourceLimit();
    }

    void UpdatePlanetResources(){
            foreach(Building building in _buildings){
                // foreach(Resource resource in building.ProductCost){  TO DO: product cost, linq?
                //     var Quantity = Resources[resource.Name].Quantity; 
                //     if(0 >=(Quantity-resource.Quantity)){
                        
                //     }
                // }
                foreach(Resource product in building.Products){
                    if(PlayerResources[product.Name].Value + product.Quantity<ResourceLimits[product.Name]){
                        if(PayCost(building.ProductCost)){
                            if(PlayerResources.ContainsKey(product.Name)){
                                //int temp = product.Quantity;
                                //Resources[product.Name].Quantity = Resources[product.Name].Quantity + product.Quantity;
                                PlayerResources[product.Name].Value += product.Quantity;
                            }else{
                                PlayerResources.Add(product.Name, product);
                            }
                            ResourcesChanged = true;
                        }
                    }else{
                        if(PayCost(building.ProductCost)){
                            if(PlayerResources.ContainsKey(product.Name)){
                                //int temp = product.Quantity;
                                //Resources[product.Name].Quantity = Resources[product.Name].Quantity + product.Quantity;
                                PlayerResources[product.Name].Value = ResourceLimits[product.Name];
                            }else{
                                PlayerResources.Add(product.Name, product);
                            }
                            ResourcesChanged = true;
                        }
                    }
                }
            }
    }

    public override void _Process(float delta){
        _time += delta;
        if(_time >= TimeStep){
            //UpdatePlanetResources();
            _time = 0;
        }
        if(Orbit.OrbitChanged){
            if(_orbit.HasLocal()){
                Vision = true;
            }
        }
    }
}
