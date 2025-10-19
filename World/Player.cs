using Godot;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;
using System;

public partial class Player : Node, IEquatable<Player>, IEndTurnListener
{
    public int PlayerID { get; set; }

    public string PlayerName { get; set; }

    public delegate void UnitssChangedEventHandler(Ship mapArmy);

    public event UnitssChangedEventHandler UnitsChanged;

    public delegate void PlayerDataChanged(Player caller);

    public event PlayerDataChanged ArmiesChanged;

    [Export]
    public Color PlayerColor { get; set; }

    public bool IsLocal { get; set; } = false;

    [Export]
    public int TimeStep { get; set; } = 1;

    [Export]
    public Array<Technology> ExportTechnologies { get; set; } = new Array<Technology>();

    public List<int> Technologies { get; set; } = new List<int>();

    public List<Ship> Ships { get; set; } = new List<Ship>();

    public ConstructionManager Research { get; set; } //= new ConstructionManager();

    double _time = 0;

    public List<Planet> Planets { get; set; } = new List<Planet>();

    private Array<CollisionObject3D> _MapObejcts = new Array<CollisionObject3D>();
    public Array<CollisionObject3D> MapObjects
    {
        get { return _MapObejcts; }
    }

    // private Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
    // public Dictionary<string, Resource> Resources
    // {
    //     get { return _resources; }
    // }

    private ResourceManager _resourceManager; // = new ResourceManager();

    public ResourceManager ResManager 
    { 
        get { return _resourceManager; } 
    }

    // private Dictionary<string, int> _resourceLimits = new Dictionary<string, int>();
    // public Dictionary<string, int> ResourceLimits
    // {
    //     get { return _resourceLimits; }
    // }

    // public bool PayCost(Array<Resource> BuildCost){
    //             foreach(Resource resource in BuildCost){
    //                 if(Resources.ContainsKey(resource.Name)){
    //                     if(Resources[resource.Name].Value < resource.Quantity){
    //                         return false;
    //                     }
    //                 }else{
    //                     return false;
    //                 }
    //             }
    //     foreach(Resource resource in BuildCost){
    //         Resources[resource.Name].Value -= resource.Quantity;
    //     }
    //     return true;
    // }

    public PhysicsBody3D GetMapObjectByName(string name){
        foreach(PhysicsBody3D body in _MapObejcts){
            if(body.Name == name){
                return body;
            }
        }
        return null;
    }   

    public void AddMapObject(CollisionObject3D mapObject){
        MapObjects.Add(mapObject);
        if(mapObject is Ship ship){
            Ships.Add(ship);
            ship.Controller = this;
            //ship.Units.AddUpkeep -= UpdateUpkeep;
            ship.UnitController.AddUpkeep += ResManager.UpdateUpkeep;
            ship.UnitController.RemoveUpkeep += ResManager.RemoveUpkeep;
            UnitsChanged?.Invoke(ship);
            ArmiesChanged?.Invoke(this);
        }
        if(mapObject is Planet planet){
            Planets.Add(planet);
            ResManager.AddProduction(planet.ResourcesManager.Production.Upkeep);
            ResManager.AddProductionCost(planet.ResourcesManager.ProdCost.Upkeep);
        }
        
    }

    public void RemoveMapObject(CollisionObject3D mapObject){
        MapObjects.Remove(mapObject);
        if(mapObject is Ship ship){
            Ships.Remove(ship);
            ship.UnitController.AddUpkeep -= ResManager.UpdateUpkeep;
            ship.UnitController.RemoveUpkeep -= ResManager.RemoveUpkeep;
            ArmiesChanged?.Invoke(this);
        }
        if(mapObject is Planet planet){
            Planets.Remove(planet);
            ResManager.RemoveProduction(planet.ResourcesManager.Production.Upkeep);
            ResManager.RemoveProductionCost(planet.ResourcesManager.ProdCost.Upkeep);
            ResManager.RemoveResourceLimit(planet.ResourcesManager.ResourceLimits.Upkeep);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerID = GetIndex();
        PlayerName = "Player "+ PlayerID;
        Name = PlayerName;
        EndTurnEmitter.Instance.EndTurn += _on_EndTurn;
        _resourceManager = GetNode<ResourceManager>("ResourceManager");
        Research = GetNode<ConstructionManager>("TechManager");
        // AddChild(ResManager);
        // AddChild(Research);
        // for(int i = 0; i<5; i++){
        //     var resource = new Resource();
        //     resource.Name = "resource "+i;
        //     resource.Value = i * 10;
        //     Resources.Add(resource.Name, resource);
        //     ResourcesChanged = true;
        // }
    }

    // void UpdateTempResources(){
    //         Resources.Clear();
    //         foreach(IResourceManager resManager in MapObjects.Where( x => x is IResourceManager )){
    //                 if(resManager.ResourcesManager.ResourcesChanged){
    //                     foreach(Resource resource in resManager.ResourcesManager.Resources.Values){
    //                         if(Resources.ContainsKey(resource.Name)){
    //                             Resources[resource.Name].Value += resource.Value;
    //                         }else{
    //                             Resources.Add(resource.Name,resource);
    //                         }
    //                     }
    //                     resManager.ResourcesManager.ResourcesChanged = false;
    //                 }
    //         }
    //         ResourcesChanged = true;
    // }

    // protected void UpdatePlayerResources(){
    //     foreach(Planet planet in MapObjects.Where( x => x is Planet )){
    //         UpdateResourceLimit(planet);
    //         _resourceManager.AddResource("Credits", (int)(0.01f*planet.Pops.TotalQuantity));
    //         _resourceManager.UpdateResources(planet.BuildingManager.Buildings);
            
    //         ResourcesChanged = true;
    //     }
    // }

    protected void InitPlayerResources(List<Resource> resources){
        for(int i = 0; i < resources.Count; i++){
            if(!ResManager.Resources.ContainsKey(resources[i].Index))
                ResManager.Resources.Add(resources[i].Index, 0);
        }
        //_resourceManager.PayUpkeep(_resourceManager.Upkeep);
    }

    public void InitResourceLimit(){
        foreach(Planet planet in MapObjects.Where( x => x is Planet )){
            _resourceManager.ResourceLimits.UpdateUpkeep(planet.BuildingManager.Buildings.ToList<IUpkeep>());
        }
    }

    List<Technology> IConstructToTechnology(List<IConstruct> IConstructs){
        var Array = new List<Technology>();
        foreach(var IConstruct in IConstructs){
            if(IConstruct is Technology technology)
                Array.Add(technology);
        }
        return Array;
    }

    // public override void _Process(double delta){
    //     _time += delta;
    //     if(_time >= TimeStep){
    //         if(Research != null)
    //             if(Research.HasConstruct)
    //                 Technologies.AddRange(IConstructToTechnology(Research.UpdateConstruction()).Select(x => x.Index));
    //         _time = 0;
    //     }
    // }

    // public static bool operator== (Player player1, Player player2)
    // {
    //     if(player1 == null || player2 == null) return false;
    //     return player1.PlayerID == player2.PlayerID;
    // }

    // this is second one '!='
    // public static bool operator!= (Player player1, Player player2)
    // {
    //     if(player1 == null || player2 == null) return true;
    //     return player1.PlayerID != player2.PlayerID;
    // }

/// <summary>
/// Same as ControllerA.PlayerID == ControllerB.PlayerID;
/// </summary>
/// <param name="other"></param>
/// <returns></returns>
    public bool Equals(Player other)
    {
        if (other == null)
            return false;
        return PlayerID == other.PlayerID;
    }

    public override bool Equals(object obj)
    {
        if(obj is Player player) return Equals(player);
        return false;
    }

    public override int GetHashCode()
    {
        return PlayerID.GetHashCode();
    }

    public void _on_EndTurn(int TurnNumber)
    {
        ResManager.AddResource(ResManager.TotalProduction.Upkeep);
    }
}
