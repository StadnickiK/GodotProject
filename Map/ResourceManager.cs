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
}
