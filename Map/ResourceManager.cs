using Godot;
using System;
using System.Collections.Generic;

public class ResourceManager : Node
{

    private Dictionary<string, int> _resources = new Dictionary<string, int>();
    public Dictionary<string, int> Resources
    {
        get { return _resources; }
    }

    private Dictionary<string, int> _resourceLimits = new Dictionary<string, int>();
    public Dictionary<string, int> ResourceLimits
    {
        get { return _resourceLimits; }
    }

    public Dictionary<string, int> Upkeep { get; set; } = new Dictionary<string, int>();

    public bool UpkeepChanged { get; set; } = false;

    public Dictionary<string, int> ProdCost { get; set; } = new Dictionary<string, int>();

    public bool ProdCostChanged { get; set; } = false;

    public Dictionary<string, int> Production { get; set; } = new Dictionary<string, int>();

    public bool ProductionChanged { get; set; } = false;

    public int TotalResourceLimit { get; set; } = -1; // used for dynamaic resource limit allocation, -1 if not used

    public bool ResourceLimitChanged { get; set; } = false;

    public bool ResourcesChanged { get; set; } = false;

    public override void _Ready()
    {
        
    }

    public bool TransferResources(IResourceManager target, string resourceName, int quantity){
        if(target.ResourcesManager.HasLimit(resourceName, quantity))
            if(PayCost(resourceName, quantity)){
                target.ResourcesManager.AddResource(resourceName, quantity);
                return true;
            }
        return false;
    }

    public bool TransferResources(ResourceManager target, string resourceName, int quantity){
        if(target.HasLimit(resourceName, quantity))
            if(PayCost(resourceName, quantity)){
                target.AddResource(resourceName, quantity);
                return true;
            }
        return false;
    }

    public void UpdateUpkeep(List<Building> buildings){
        foreach(var building in buildings)
            UpdateUpkeep(building);
    }

    public void UpdateUpkeep(string resName , int quantity){
        if(Upkeep.ContainsKey(resName)){
            Upkeep[resName] += quantity;
        }else
        {
            Upkeep.Add(resName, quantity);
        }
    }

    public void UpdateUpkeep(IUpkeep upkeep){
        foreach(var pair in upkeep.Upkeep){
            UpdateUpkeep(pair.Key, pair.Value);
        }
    }

    public void RemoveUpkeep(IUpkeep upkeep){
        foreach(var pair in upkeep.Upkeep){
            RemoveUpkeep(pair.Key, pair.Value);
        }
    }

    public void RemoveUpkeep(string resName , int quantity){
        if(Upkeep.ContainsKey(resName)){
            if(Upkeep[resName] - quantity > 0){
                Upkeep[resName] -= quantity;
            }else
            {
                Upkeep[resName] = 0;
            }
        }else
        {
            Upkeep.Add(resName, 0);
        }
    }

    public void UpdateResourceLimit(List<Building> buildings){  
        foreach(Building building in buildings){
            if(building.ResourceLimits != null)
                UpdateResourceLimit(building);
        }
    }

    public void UpdateResourceLimit(Building building){
        if(building.ResourceLimits != null)
            foreach(var resourceName in building.ResourceLimits.Keys){
                if(ResourceLimits.ContainsKey(resourceName)){
                    ResourceLimits[resourceName] += building.ResourceLimits[resourceName];
                    ResourceLimitChanged = true;   
                }else{
                    ResourceLimits.Add(resourceName, building.ResourceLimits[resourceName]);
                    ResourceLimitChanged = true;   
                }
            }
    }

    public void RemoveResourceLimit(Building building){
        if(building.ResourceLimits != null)
            foreach(var resourceName in building.ResourceLimits.Keys){
                if(ResourceLimits.ContainsKey(resourceName)){
                    ResourceLimits[resourceName] -= building.ResourceLimits[resourceName];
                    ResourceLimitChanged = true;   
                }
            }
    }

    public void UpdateResourceLimit(Building newBuilding, Building oldBuilding){
        RemoveResourceLimit(oldBuilding);
        UpdateResourceLimit(newBuilding);
    }

    public void UpdateResourceLimit(string resourceName, int quantity){
                if(ResourceLimits.ContainsKey(resourceName)){
                    ResourceLimits[resourceName] += quantity;
                    ResourceLimitChanged = true;   
                }else{
                    ResourceLimits.Add(resourceName, quantity);
                    ResourceLimitChanged = true;   
                }
    }

    public bool PayCost(Godot.Collections.Dictionary<string, int> BuildCost){
        if(!CanPayCost(BuildCost)) 
            return false;
        foreach(var resName in BuildCost.Keys){
            if(BuildCost[resName] > 0)
                Resources[resName] -= BuildCost[resName];
        }
        return true;
    }

    public bool PayUpkeep(Dictionary<string, int> upkeep){
        bool payed = true;
        foreach(var resName in upkeep.Keys){
            if(Resources.ContainsKey(resName)){
                if((Resources[resName] - upkeep[resName])  >= 0){
                    Resources[resName] -= upkeep[resName];
                }else{
                    Resources[resName] = 0;
                    payed = false;
                }
            }else{
                payed = false;
            }
        }
        return payed;
    }

    

    public bool CanPayCost(Godot.Collections.Dictionary<string, int> BuildCost){
        foreach(var resName in BuildCost.Keys){
            if(BuildCost[resName] > 0)
                if(Resources.ContainsKey(resName)){
                    if(Resources[resName] < BuildCost[resName]){
                        return false;
                    }
                }else{
                    return false;
            }
        }
        return true;
    }

    public bool PayCost(Resource resource){
            if(Resources.ContainsKey(resource.Name)){
                if(Resources[resource.Name] < resource.Quantity){
                    return false;
                }
            }else{
                return false;
            }
            Resources[resource.Name] -= resource.Quantity;
        return true;
    }

    public bool PayCost(string resourceName, int quantity){
            if(Resources.ContainsKey(resourceName)){
                if(Resources[resourceName] < quantity){
                    return false;
                }
            }else{
                return false;
            }
            Resources[resourceName] -= quantity;
        return true;
    }

    public bool HasResource(string resourceName){
        return (Resources.ContainsKey(resourceName));
    }

    public bool HasResource(string resourceName, int quantity){
        if(HasResource(resourceName))
            if(Resources[resourceName] >= quantity)
                return true;
        return false;  
    }

    public bool HasResource(Dictionary<string, int> resources){
        foreach(var name in resources.Keys){
            if(!HasResource(name))
                return false;
            if(Resources[name] < resources[name])
                return false;
        }
        return true;
    }

    public bool HasResource(Godot.Collections.Dictionary<string, int> resources){
        foreach(var name in resources.Keys){
            if(!HasResource(name))
                return false;
            if(Resources[name] < resources[name])
                return false;
        }
        return true;
    }

    public bool HasLimit(string resourceName, int quantity){
        if(TotalResourceLimit < 0){
            if(ResourceLimits.ContainsKey(resourceName))
                if(HasResource(resourceName)){
                    if((ResourceLimits[resourceName] - Resources[resourceName]) > quantity)
                        return true;
                }else{
                    if(ResourceLimits[resourceName] > quantity)
                        return true;
                }
        }else{
            return CheckDynamicLimit(resourceName, quantity);
        }
        return false;  
    }

    public bool CheckDynamicLimit(string resourceName, int quantity){
        if(ResourceLimits.ContainsKey(resourceName)){
            if(HasResource(resourceName)){
                if((ResourceLimits[resourceName] - Resources[resourceName]) > quantity)
                    return true;
            }else{
                if(ResourceLimits[resourceName] > quantity)
                    return true;
            }
        }else{
            if(quantity <= TotalResourceLimit){
                ResourceLimits.Add(resourceName, quantity);
                return true;
            }
        }
        return false;
    }

    public int GetResourceFillPercent(string resName){
        if(ResourceLimits.ContainsKey(resName) && Resources.ContainsKey(resName)){
            return Resources[resName] / ResourceLimits[resName];
        }
        return 0;
    }

    public void UpdateResources(List<Building> buildings){
            foreach(Building building in buildings){
                foreach(string productName in building.Products.Keys){
                    if(!Resources.ContainsKey(productName)){
                        var quantity = building.Products[productName];
                        if(ResourceLimits.ContainsKey(productName))
                            if(quantity<ResourceLimits[productName]){  // case for no resource limit may be required
                                if(PayCost(building.ProductCost)){
                                    Resources.Add(productName, quantity);
                                    ResourcesChanged = true;
                                }
                            }
                    }else{
                        var quantity = building.Products[productName];
                        if(ResourceLimits.ContainsKey(productName))
                            if(Resources[productName] + quantity<ResourceLimits[productName]){
                                if(PayCost(building.ProductCost)){
                                    Resources[productName] += quantity;
                                    ResourcesChanged = true;
                                }
                        }else{
                            if(PayCost(building.ProductCost)){
                                Resources[productName] = ResourceLimits[productName];
                                ResourcesChanged = true;
                            }
                        }
                    }
                }
            }
    }

    public void UpdateResources(Planet planet ){
            foreach(Building building in planet.BuildingsManager.Buildings){
                
                foreach(string productName in building.Products.Keys){
                    if(!Resources.ContainsKey(productName)){
                        var quantity = building.Products[productName];
                        if(ResourceLimits.ContainsKey(productName))
                            if(quantity<ResourceLimits[productName]){  // case for no resource limit may be required
                                if(PayCost(building.ProductCost)){
                                    Resources.Add(productName, quantity);
                                    ResourcesChanged = true;
                                }
                            }
                    }else{
                        var quantity = building.Products[productName];
                        if(ResourceLimits.ContainsKey(productName))
                            if(Resources[productName] + quantity<ResourceLimits[productName]){
                                if(PayCost(building.ProductCost)){
                                    Resources[productName] += quantity;
                                    ResourcesChanged = true;
                                }
                        }else{
                            if(PayCost(building.ProductCost)){
                                Resources[productName] = ResourceLimits[productName];
                                ResourcesChanged = true;
                            }
                        }
                    }
                }
            }
    }

        public void AddResource(Resource resource){
                    if(Resources[resource.Name] + resource.Quantity<ResourceLimits[resource.Name]){
                            if(Resources.ContainsKey(resource.Name)){
                                Resources[resource.Name] += resource.Quantity;
                            }else{
                                Resources.Add(resource.Name, resource.Quantity);
                            }
                            ResourcesChanged = true;
                        
                    }else{
                            if(Resources.ContainsKey(resource.Name)){
                                Resources[resource.Name] = ResourceLimits[resource.Name];
                            }else{
                                Resources.Add(resource.Name, resource.Quantity);
                            }
                            ResourcesChanged = true;
                        
                    }
        }

    public void AddResource(string resourceName, int quantity){
        if(Resources.ContainsKey(resourceName)){
            if(ResourceLimits.ContainsKey(resourceName))
                if(Resources[resourceName] + quantity <= ResourceLimits[resourceName]){
                    Resources[resourceName] += quantity;
                }else{
                    Resources[resourceName] = ResourceLimits[resourceName];
                }
        }else{
            if(ResourceLimits.ContainsKey(resourceName)){
                if(quantity <= ResourceLimits[resourceName]){
                    Resources.Add(resourceName, quantity);
                }
            }else{
                Resources.Add(resourceName, 0);
            }
        }
        ResourcesChanged = true;  
    }
}
