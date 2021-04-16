using Godot;
using System;
using System.Collections.Generic;

public class ResourceManager : Node
{

    private Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
    public Dictionary<string, Resource> Resources
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

    public void UpdateResourceLimit(Building building){
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

    public void UpdateResourceLimit(string resourceName, int quantity){
                if(ResourceLimits.ContainsKey(resourceName)){
                    ResourceLimits[resourceName] += quantity;
                    ResourceLimitChanged = true;   
                }else{
                    ResourceLimits.Add(resourceName, quantity);
                    ResourceLimitChanged = true;   
                }
    }

    public bool PayCost(List<Resource> BuildCost){
        foreach(Resource resource in BuildCost){
            if(Resources.ContainsKey(resource.Name)){
                if(Resources[resource.Name].Value < resource.Quantity){
                    return false;
                }
            }else{
                return false;
            }
        }
        foreach(Resource resource in BuildCost){
            Resources[resource.Name].Value -= resource.Quantity;
        }
        return true;
    }

    public bool PayCost(Resource resource){
            if(Resources.ContainsKey(resource.Name)){
                if(Resources[resource.Name].Value < resource.Quantity){
                    return false;
                }
            }else{
                return false;
            }
            Resources[resource.Name].Value -= resource.Quantity;
        return true;
    }

    public bool PayCost(string resourceName, int quantity){
            if(Resources.ContainsKey(resourceName)){
                if(Resources[resourceName].Value < quantity){
                    return false;
                }
            }else{
                return false;
            }
            Resources[resourceName].Value -= quantity;
        return true;
    }

    public bool HasResource(string resourceName){
        return (Resources.ContainsKey(resourceName));
    }

    public bool HasResource(string resourceName, int quantity){
        if(HasResource(resourceName))
            if(Resources[resourceName].Value >= quantity)
                return true;
        return false;  
    }

    public bool HasLimit(string resourceName, int quantity){
        if(ResourceLimits.ContainsKey(resourceName))
            if(HasResource(resourceName)){
                if((ResourceLimits[resourceName] - Resources[resourceName].Value) > quantity)
                    return true;
            }else{
                if(ResourceLimits[resourceName] > quantity)
                    return true;
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
                foreach(Resource product in building.Products){
                    if(Resources[product.Name].Value + product.Quantity<ResourceLimits[product.Name]){
                        if(PayCost(building.ProductCost)){
                            if(Resources.ContainsKey(product.Name)){
                                //int temp = product.Quantity;
                                //Resources[product.Name].Quantity = Resources[product.Name].Quantity + product.Quantity;
                                Resources[product.Name].Value += product.Quantity;
                            }else{
                                Resources.Add(product.Name, product);
                            }
                            ResourcesChanged = true;
                        }
                    }else{
                        if(PayCost(building.ProductCost)){
                            if(Resources.ContainsKey(product.Name)){
                                //int temp = product.Quantity;
                                //Resources[product.Name].Quantity = Resources[product.Name].Quantity + product.Quantity;
                                Resources[product.Name].Value = ResourceLimits[product.Name];
                            }else{
                                Resources.Add(product.Name, product);
                            }
                            ResourcesChanged = true;
                        }
                    }
                }
            }
    }

        public void AddResource(Resource resource){
                    if(Resources[resource.Name].Value + resource.Quantity<ResourceLimits[resource.Name]){
                            if(Resources.ContainsKey(resource.Name)){
                                Resources[resource.Name].Value += resource.Quantity;
                            }else{
                                Resources.Add(resource.Name, resource);
                            }
                            ResourcesChanged = true;
                        
                    }else{
                            if(Resources.ContainsKey(resource.Name)){
                                Resources[resource.Name].Value = ResourceLimits[resource.Name];
                            }else{
                                Resources.Add(resource.Name, resource);
                            }
                            ResourcesChanged = true;
                        
                    }
        }

    public void AddResource(string resourceName, int quantity){
                    if(Resources[resourceName].Value + quantity<ResourceLimits[resourceName]){
                            if(Resources.ContainsKey(resourceName)){
                                Resources[resourceName].Value += quantity;
                            }else{
                                var resource = new Resource();
                                resource.Value = quantity;
                                Resources.Add(resource.Name, resource);
                            }
                            ResourcesChanged = true;
                        
                    }else{
                            if(Resources.ContainsKey(resourceName)){
                                Resources[resourceName].Value = ResourceLimits[resourceName];
                            }else{
                                                                var resource = new Resource();
                                resource.Value = quantity;
                                Resources.Add(resource.Name, resource);
                            }
                            ResourcesChanged = true;
                        
                    }
        
    }
}
