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

    public void UpdateResources(List<Building> buildings){
            foreach(Building building in buildings){
                // foreach(Resource resource in building.ProductCost){  TO DO: product cost, linq?
                //     var Quantity = Resources[resource.Name].Quantity; 
                //     if(0 >=(Quantity-resource.Quantity)){
                        
                //     }
                // }
                foreach(string productName in building.Products.Keys){
                    if(!Resources.ContainsKey(productName)){
                        var quantity = building.Products[productName];
                        if(productName == "Resource 1")
                            GD.Print();
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
                    // if(Resources[product.Name].Value + product.Quantity<ResourceLimits[product.Name]){
                    //     if(PayCost(building.ProductCost)){
                    //         if(Resources.ContainsKey(product.Name)){
                    //             //int temp = product.Quantity;
                    //             //Resources[product.Name].Quantity = Resources[product.Name].Quantity + product.Quantity;
                    //             Resources[product.Name].Value += product.Quantity;
                    //         }else{
                    //             Resources.Add(product.Name, product);
                    //         }
                    //         ResourcesChanged = true;
                    //     }
                    // }else{
                    //     if(PayCost(building.ProductCost)){
                    //         if(Resources.ContainsKey(product.Name)){
                    //             //int temp = product.Quantity;
                    //             //Resources[product.Name].Quantity = Resources[product.Name].Quantity + product.Quantity;
                    //             Resources[product.Name].Value = ResourceLimits[product.Name];
                    //         }else{
                    //             Resources.Add(product.Name, product);
                    //         }
                    //         ResourcesChanged = true;
                    //     }
                    // }
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
            if(Resources[resourceName] + quantity < ResourceLimits[resourceName]){
                Resources[resourceName] += quantity;
            }else{
                Resources[resourceName] = quantity;
            }
        }else{
            if(quantity <= ResourceLimits[resourceName]){
                // var resource = new Resource();
                // resource.Value = quantity;
                // resource.Name =resourceName;
                Resources.Add(resourceName, quantity);
            }
        }
        ResourcesChanged = true;  
    }
}
