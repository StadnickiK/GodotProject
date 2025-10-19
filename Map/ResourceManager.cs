using Godot;
using System;
using System.Collections.Generic;

public partial class ResourceManager : Node
{

    private Dictionary<int, int> _resources = new Dictionary<int, int>();
    public Dictionary<int, int> Resources
    {
        get { return _resources; }
        set { _resources = value; }
    }

    public UpkeepComponent ResourceLimits { get; set; }

    public UpkeepComponent ProdCost { get; set; }

    public UpkeepComponent Production { get; set; }

    public UpkeepComponent Upkeep { get; set; }

    public UpkeepComponent TotalProduction { get; set; }

    public delegate void ResourcesChangedEventHandler(ResourceManager resourceManager);

    public event ResourcesChangedEventHandler ProdChanged;

    public event ResourcesChangedEventHandler ProdCostChanged;

    public event ResourcesChangedEventHandler ResourceLimitsChanged;

    public event ResourcesChangedEventHandler UpkeepChanged;

    public event ResourcesChangedEventHandler PlayerResourcesChanged;

    public int TotalResourceLimit { get; set; } = -1; // used for dynamaic resource limit allocation, -1 if not used

    public override void _Ready()
    {
        ResourceLimits = GetNodeOrNull<UpkeepComponent>("ResourceLimits");
        Production = GetNodeOrNull<UpkeepComponent>("Production");
        ProdCost = GetNodeOrNull<UpkeepComponent>("ProdCost");
        Upkeep = GetNodeOrNull<UpkeepComponent>("Upkeep");
        TotalProduction = GetNodeOrNull<UpkeepComponent>("TotalProduction");
    }

    public bool TransferResources(IResourceManager target, int resourceName, int quantity)
    {
        if (target.ResourcesManager.HasLimit(resourceName, quantity))
            if (PayCost(resourceName, quantity))
            {
                target.ResourcesManager.AddResource(resourceName, quantity);
                return true;
            }
        return false;
    }

    public bool TransferResources(ResourceManager target, int resourceName, int quantity)
    {
        if (target.HasLimit(resourceName, quantity))
            if (PayCost(resourceName, quantity))
            {
                target.AddResource(resourceName, quantity);
                PlayerResourcesChanged?.Invoke(this);
                return true;
            }
            
        return false;
    }

    public bool PayCost(Dictionary<int, int> BuildCost)
    {
        if (!CanPayCost(BuildCost))
            return false;
        foreach (var resName in BuildCost.Keys)
        {
            if (BuildCost[resName] > 0)
                Resources[resName] -= BuildCost[resName];
        }
        PlayerResourcesChanged?.Invoke(this);
        return true;
    }

    public bool PayUpkeep(Dictionary<int, int> upkeep)
    {
        bool payed = true;
        foreach (var resName in upkeep.Keys)
        {
            if (Resources.ContainsKey(resName))
            {
                if ((Resources[resName] - upkeep[resName]) >= 0)
                {
                    Resources[resName] -= upkeep[resName];
                }
                else
                {
                    Resources[resName] = 0;
                    payed = false;
                }
            }
            else
            {
                payed = false;
            }
        }
        return payed;
    }



    public bool CanPayCost(Dictionary<int, int> BuildCost)
    {
        foreach (var resName in BuildCost.Keys)
        {
            if (BuildCost[resName] > 0)
                if (Resources.ContainsKey(resName))
                {
                    if (Resources[resName] < BuildCost[resName])
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
        }
        return true;
    }

    public bool PayCost(Resource resource)
    {
        if (Resources.ContainsKey(resource.Index))
        {
            if (Resources[resource.Index] < resource.Quantity)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        Resources[resource.Index] -= resource.Quantity;
        PlayerResourcesChanged?.Invoke(this);
        return true;
    }

    public bool PayCost(int resourceName, int quantity)
    {
        if (Resources.ContainsKey(resourceName))
        {
            if (Resources[resourceName] < quantity)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        Resources[resourceName] -= quantity;
        PlayerResourcesChanged?.Invoke(this);
        return true;
    }

    public bool HasResource(int resourceName)
    {
        return Resources.ContainsKey(resourceName);
    }

    public bool HasResource(int resourceName, int quantity)
    {
        if (HasResource(resourceName))
            if (Resources[resourceName] >= quantity)
                return true;
        return false;
    }

    public bool HasResource(Dictionary<int, int> resources)
    {
        foreach (var name in resources.Keys)
        {
            if (!HasResource(name))
                return false;
            if (Resources[name] < resources[name])
                return false;
        }
        return true;
    }

    public bool HasGivenResource(Dictionary<int, int> resources)
    {
        foreach (var name in resources.Keys)
        {
            if (!HasResource(name))
                return false;
            if (Resources[name] < resources[name])
                return false;
        }
        return true;
    }

    public bool HasLimit(int resourceName, int quantity)
    {
        if (TotalResourceLimit < 0)
        {
            if (ResourceLimits.Upkeep.ContainsKey(resourceName))
                if (HasResource(resourceName))
                {
                    if ((ResourceLimits.Upkeep[resourceName] - Resources[resourceName]) > quantity)
                        return true;
                }
                else
                {
                    if (ResourceLimits.Upkeep[resourceName] > quantity)
                        return true;
                }
        }
        else
        {
            return CheckDynamicLimit(resourceName, quantity);
        }
        return false;
    }

    public bool CheckDynamicLimit(int resourceName, int quantity)
    {
        if (ResourceLimits.Upkeep.ContainsKey(resourceName))
        {
            if (HasResource(resourceName))
            {
                if ((ResourceLimits.Upkeep[resourceName] - Resources[resourceName]) > quantity)
                    return true;
            }
            else
            {
                if (ResourceLimits.Upkeep[resourceName] > quantity)
                    return true;
            }
        }
        else
        {
            if (quantity <= TotalResourceLimit)
            {
                ResourceLimits.Upkeep.Add(resourceName, quantity);
                return true;
            }
        }
        return false;
    }

    public int GetResourceFillPercent(int resName)
    {
        if (ResourceLimits.Upkeep.ContainsKey(resName) && Resources.ContainsKey(resName))
        {
            return Resources[resName] / ResourceLimits.Upkeep[resName];
        }
        return 0;
    }

    public void AddResource(Resource resource)
    {
        if (Resources[resource.Index] + resource.Quantity < ResourceLimits.Upkeep[resource.Index])
        {
            if (Resources.ContainsKey(resource.Index))
            {
                Resources[resource.Index] += resource.Quantity;
            }
            else
            {
                Resources.Add(resource.Index, resource.Quantity);
            }

        }
        else
        {
            if (Resources.ContainsKey(resource.Index))
            {
                Resources[resource.Index] = ResourceLimits.Upkeep[resource.Index];
            }
            else
            {
                Resources.Add(resource.Index, resource.Quantity);
            }
        }
        PlayerResourcesChanged?.Invoke(this);
    }

    public void AddResource(int resourceName, int quantity)
    {
        if (Resources.ContainsKey(resourceName))
        {
            if (ResourceLimits.Upkeep.ContainsKey(resourceName))
                if (Resources[resourceName] + quantity <= ResourceLimits.Upkeep[resourceName])
                {
                    Resources[resourceName] += quantity;
                }
                else
                {
                    Resources[resourceName] = ResourceLimits.Upkeep[resourceName];
                }
        }
        else
        {
            if (ResourceLimits.Upkeep.ContainsKey(resourceName))
            {
                if (quantity <= ResourceLimits.Upkeep[resourceName])
                {
                    Resources.Add(resourceName, quantity);
                }
            }
            else
            {
                Resources.Add(resourceName, 0);
            }
        }
        PlayerResourcesChanged?.Invoke(this);
    }

    public void AddResource(Dictionary<int, int> resources)
    {
        foreach (var resource in resources)
            if (Resources.ContainsKey(resource.Key))
            {
                if (ResourceLimits.Upkeep.ContainsKey(resource.Key))
                    if (Resources[resource.Key] + resource.Value <= ResourceLimits.Upkeep[resource.Key])
                    {
                        Resources[resource.Key] += resource.Value;
                    }
                    else
                    {
                        Resources[resource.Key] = ResourceLimits.Upkeep[resource.Key];
                    }
            }
            else
            {
                if (ResourceLimits.Upkeep.ContainsKey(resource.Key))
                {
                    if (resource.Value <= ResourceLimits.Upkeep[resource.Key])
                    {
                        Resources.Add(resource.Key, resource.Value);
                    }
                }
                else
                {
                    Resources.Add(resource.Key, 0);
                }
            }
        PlayerResourcesChanged?.Invoke(this);
    }

    public void UpdateUpkeep(System.Collections.Generic.Dictionary<int, int> resources){
        Upkeep.UpdateUpkeep(resources);
        TotalProduction.RemoveUpkeep(resources);
        UpkeepChanged?.Invoke(this);
    }

    public void RemoveUpkeep(System.Collections.Generic.Dictionary<int, int> upkeep){
        Upkeep.RemoveUpkeep(upkeep);
        TotalProduction.UpdateUpkeep(upkeep);
        UpkeepChanged?.Invoke(this);
    }

    public void AddProduction(System.Collections.Generic.Dictionary<int, int> prod){
        Production.UpdateUpkeep(prod);
        TotalProduction?.UpdateUpkeep(prod);
        ProdChanged?.Invoke(this); 
    }

    public void RemoveProduction(System.Collections.Generic.Dictionary<int, int> prod){
        Production.RemoveUpkeep(prod);
        TotalProduction.RemoveUpkeep(prod); 
        ProdChanged?.Invoke(this); 
    }

    public void AddProductionCost(System.Collections.Generic.Dictionary<int, int> prod){
        ProdCost.UpdateUpkeep(prod);
        TotalProduction?.RemoveUpkeep(prod); 
        ProdCostChanged?.Invoke(this); 
    }

    public void RemoveProductionCost(System.Collections.Generic.Dictionary<int, int> prod)
    {
        ProdCost.RemoveUpkeep(prod);
        TotalProduction.UpdateUpkeep(prod);
        ProdCostChanged?.Invoke(this);
    }
    
    public void AddResourceLimit(System.Collections.Generic.Dictionary<int, int> prod){
        ResourceLimits.UpdateUpkeep(prod);
        ResourceLimitsChanged?.Invoke(this); 
    }

    public void RemoveResourceLimit(System.Collections.Generic.Dictionary<int, int> prod){
        ResourceLimits.RemoveUpkeep(prod);
        ResourceLimitsChanged?.Invoke(this); 
    }
}
