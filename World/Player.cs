using Godot;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public partial class Player : Node
{
    public int PlayerID { get; set; }

    public string PlayerName { get; set; }

    public delegate void ResourcesChangedEventHandler(Player player);

    public event ResourcesChangedEventHandler ProdChanged;

    public event ResourcesChangedEventHandler ProdCostChanged;

    public event ResourcesChangedEventHandler UpkeepChanged;

    public event ResourcesChangedEventHandler PlayerResourcesChanged;

    [Export]
    public Color PlayerColor { get; set; }

    public bool IsLocal { get; set; } = false;

    public bool MapObjectsChanged { get; set; } = true;

    [Export]
    public int TimeStep { get; set; } = 1;

    [Export]
    public Array<Technology> ExportTechnologies { get; set; } = new Array<Technology>();

    public List<int> Technologies { get; set; } = new List<int>();

    public List<Ship> Ships { get; set; } = new List<Ship>();

    public ConstructionManager Research { get; set; } //= new ConstructionManager();

    double _time = 0;

    public List<Planet> Planets { get; set; } = new List<Planet>();

    public UpkeepComponent Upkeep { get; set; }

    public UpkeepComponent TotalProduction { get; set; }

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

    public bool ResourcesChanged { get; set; } = false;

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

    public void AddUpkeep(System.Collections.Generic.Dictionary<int, int> upkeep){
        Upkeep.UpdateUpkeep(upkeep);
        TotalProduction.RemoveUpkeep(upkeep);
        UpkeepChanged?.Invoke(this);
           
    }

    public void RemoveUpkeep(System.Collections.Generic.Dictionary<int, int> upkeep){
        Upkeep.RemoveUpkeep(upkeep);
        TotalProduction.UpdateUpkeep(upkeep);
        UpkeepChanged?.Invoke(this);
    }

    public void AddProduction(System.Collections.Generic.Dictionary<int, int> prod){
        ResManager.Production.UpdateUpkeep(prod);
        TotalProduction?.UpdateUpkeep(prod);
        ProdChanged?.Invoke(this); 
    }

    public void RemoveProduction(System.Collections.Generic.Dictionary<int, int> prod){
        ResManager.Production.RemoveUpkeep(prod);
        TotalProduction.RemoveUpkeep(prod); 
        ProdChanged?.Invoke(this); 
    }

    public void AddProductionCost(System.Collections.Generic.Dictionary<int, int> prod){
        ResManager.ProdCost.UpdateUpkeep(prod);
        TotalProduction?.RemoveUpkeep(prod); 
        ProdCostChanged?.Invoke(this); 
    }

    public void RemoveProductionCost(System.Collections.Generic.Dictionary<int, int> prod){
        ResManager.ProdCost.RemoveUpkeep(prod);
        TotalProduction.UpdateUpkeep(prod); 
        ProdCostChanged?.Invoke(this); 
    }

    public void AddMapObject(CollisionObject3D mapObject){
        MapObjects.Add(mapObject);
        if(mapObject is Ship ship){
            Ships.Add(ship);
            ship.Units.AddUpkeep += UpdateUpkeep;
            ship.Units.RemoveUpkeep += RemoveUpkeep;
        }
        if(mapObject is Planet planet){
            Planets.Add(planet);
            AddProduction(planet.ResourcesManager.Production.Upkeep);
            AddProductionCost(planet.ResourcesManager.ProdCost.Upkeep);
        }
        MapObjectsChanged = true;
    }

    public void RemoveMapObject(CollisionObject3D mapObject){
        MapObjects.Remove(mapObject);
        if(mapObject is Ship ship){
            Ships.Remove(ship);
            ship.Units.AddUpkeep -= UpdateUpkeep;
            ship.Units.RemoveUpkeep -= RemoveUpkeep;
        }
        if(mapObject is Planet planet){
            Planets.Remove(planet);
            RemoveProduction(planet.ResourcesManager.Production.Upkeep);
            RemoveProductionCost(planet.ResourcesManager.ProdCost.Upkeep);
        }
        MapObjectsChanged = true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerID = GetIndex();
        PlayerName = "Player "+ PlayerID;
        Name = PlayerName;
        
        _resourceManager = GetNode<ResourceManager>("ResourceManager");
        Research = GetNode<ConstructionManager>("TechManager");
        Upkeep = GetNodeOrNull<UpkeepComponent>("UpkeepComponent");
        TotalProduction = GetNodeOrNull<UpkeepComponent>("TotalProduction");
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
    //         _resourceManager.UpdateResources(planet.BuildingsManager.Buildings);
            
    //         ResourcesChanged = true;
    //     }
    // }

    protected void UpdatePlayerResources(){ 
        _resourceManager.AddResource(TotalProduction.Upkeep);
        PlayerResourcesChanged?.Invoke(this);
        
    }

    protected void InitPlayerResources(List<Resource> resources){
        for(int i = 0; i < resources.Count; i++){
            if(!ResManager.Resources.ContainsKey(resources[i].Index))
                ResManager.Resources.Add(resources[i].Index, 0);
        }
        //_resourceManager.PayUpkeep(_resourceManager.Upkeep);
    }

    public void InitResourceLimit(){
        foreach(Planet planet in MapObjects.Where( x => x is Planet )){
            _resourceManager.ResourceLimits.UpdateUpkeep(planet.BuildingsManager.Buildings);
        }
    }

    protected void UpdateResourceLimit(){
        foreach(Planet planet in MapObjects.Where( x => x is Planet )){
            UpdateResourceLimit(planet);
        }
    }

    public void UpdateResourceLimit(System.Collections.Generic.Dictionary<int, int> resourceLimits){
        ResManager.ResourceLimits.UpdateUpkeep(resourceLimits);
    }

    public void RemoveResourceLimit(System.Collections.Generic.Dictionary<int, int> resourceLimits){
        ResManager.ResourceLimits.RemoveUpkeep(resourceLimits);
    }

    public void UpdateUpkeep(System.Collections.Generic.Dictionary<int, int> resources){
        Upkeep.UpdateUpkeep(resources);
    }

    public void UpdateResourceLimit(Planet planet){
        if(planet.BuildingsManager.BuildingsChanged){
            var buildings = planet.BuildingsManager.LastBuilding;
            _resourceManager.ResourceLimits.UpdateUpkeep(buildings);
            Upkeep.UpdateUpkeep(buildings);
            planet.BuildingsManager.BuildingsChanged = false;
        }
    }

    List<Technology> IBuildingToTechnology(List<IBuilding> ibuildings){
        var Array = new List<Technology>();
        foreach(var ibuilding in ibuildings){
            if(ibuilding is Technology technology)
                Array.Add(technology);
        }
        return Array;
    }

    public override void _Process(double delta){
        _time += delta;
        if(_time >= TimeStep){
            //UpdatePlayerResources();
            //Technologies.AddRange(IBuildingToTechnology(Research.UpdateConstruction()));
            if(Research != null)
                if(Research.HasConstruct)
                    Technologies.AddRange(IBuildingToTechnology(Research.UpdateConstruction()).Select(x => x.Index));
            _time = 0;
        }
    }

}
